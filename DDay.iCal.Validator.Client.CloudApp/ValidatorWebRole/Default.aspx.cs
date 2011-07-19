using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using System.IO;
using DDay.iCal.Serialization;
using DDay.iCal.Validator.Xml;
using DDay.iCal.Validator;
using DDay.iCal.Validator.Serialization;
using DDay.iCal.Validator.RFC5545;
using System.Text;
using System.Net;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System.Threading;

namespace ValidatorWebRole
{
	public partial class _Default : System.Web.UI.Page
	{
		public string DDayICalVersion { get; protected set; }
		public IXmlDocumentProvider DocumentProvider { get; protected set; }
		public IValidationRuleset SelectedRuleset { get; protected set; }
		public IResourceManager ResourceManager { get; protected set; }
		public Debug Debug { get; protected set; }
		public string CalendarPath { get; set; }

		Encoding encoding;

		private static CloudStorageAccount acct = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("DataConnectionString"));
		private static CloudBlobClient blob_client = acct.CreateCloudBlobClient();
		private const string results_container_name = "results";
		private CloudBlobContainer results_container = blob_client.GetContainerReference(results_container_name);

		private delegate byte[] AsyncValidator(long byte_count, TextReader tr, string id);
		private string progress_fmt = "/?mode={0}&uri={1}&id={2}&progress={3}";

		private enum ValidationMode { url, file, snippet };

		protected void Page_Load(object sender, EventArgs e)
		{

			Server.ScriptTimeout = 120;

			Assembly ddayical = Assembly.Load("DDay.iCal");
			DDayICalVersion = ddayical.GetName().Version.ToString();

			encoding = Response.Output.Encoding;

			var setup_result = Setup();
			if (setup_result.Length > 0)
			{
				SendPage(setup_result, "text/html");
				return;
			}

			if (Request["check_completion"] != null)
			{
				var blob = MakeResultsBlob(Request["check_completion"]);
				if (blob.Exists())
					SendPage(encoding.GetBytes("YES"), "text/plain");
				else
					SendPage(encoding.GetBytes("NO"), "text/plain");
			}

			if (Request["cached_id"] != null)
			{
				var blob = MakeResultsBlob(Request["cached_id"]);
				var bytes = blob.DownloadByteArray();
				SendExceptionOrResult(bytes);
			}

			if (Request["mode"] != null && Request["id"] != null && Request["progress"] != null) // in progress, invoke js progress bar / redirector
			{
				this.btnValidateUrl.Enabled = false;
				this.btnUpload.Enabled = false;
				this.btnValidateSnippet.Enabled = false;
				this.fileUpload.Enabled = false;

				switch (Request["mode"])
				{
					case "url":
						if (Request["uri"] != null)
							this.tbUrl.Text = Request["uri"];
						break;

					case "file":
						break;

					case "snippet":
						break;

					default:
						break;
				}


				ClientScriptManager script = Page.ClientScript;
				string id = Request["id"];
				var script_text = "<script type='text/javascript'>debugger; RunProgress('" + id + "');</script>";
				script.RegisterClientScriptBlock(this.GetType(), "Progress", script_text);

				return;
			}

			// Validate the URI if it was passed via query string.
			if (Request["uri"] != null)
			{
				var validation_result = ValidateUri(Request["uri"]);
				SendPage(validation_result, "text/xml");
			}
		}

		private CloudBlob MakeResultsBlob(string ticks)
		{
			var blob_name = ticks + ".xml";
			var relative_uri = string.Format("{0}/{1}", results_container_name, blob_name);
			var blob = blob_client.GetBlobReference(relative_uri);
			blob.Properties.ContentType = "text/xml";
			return blob;
		}

		private void SendPage(byte[] bytes, string content_type)
		{
			Response.ContentType = content_type;
			var page = Response.Output.Encoding.GetString(bytes);
			Response.Write(page);
			Response.End();
		}

		private void RedirectToProgressPage(ValidationMode mode, string uri, string id, string progress)
		{
			var redirect_uri = string.Format(progress_fmt, mode, uri, id, 0);
			Response.Redirect(redirect_uri);
		}

		private void RedirectToProgressPage(ValidationMode mode, string id, string progress)
		{
			var redirect_uri = string.Format(progress_fmt, mode, "", id, 0);
			Response.Redirect(redirect_uri);
		}

		#region Protected Methods

		protected byte[] Setup()
		{
			byte[] bytes = new byte[0];

			Debug = new Debug();
			Debug.Initialize();

			ISerializationSettings settings = SerializationContext.Default.GetService<ISerializationSettings>();
			settings.ParsingMode = ParsingModeType.Loose;
			settings.EnsureAccurateLineNumbers = true;
			settings.StoreExtraSerializationData = true;

			DocumentProvider = new DirectoryXmlDocumentProvider(Path.Combine(Request.PhysicalApplicationPath, "icalvalidSchema"));

			SelectedRuleset = null;
			try
			{
				ResourceManager = new ResourceManager();

				// Setup the language to use for validation/tests
				SetupLanguage();
				SelectedRuleset = LoadRuleset();
			}
			catch (ValidationRuleLoadException e)
			{
				// successfulLoad = false;
				bytes = Response.Output.Encoding.GetBytes(e.Message);
			}

			return bytes;
		}

		protected byte[] ValidateUri(string uriString)
		{
			Uri uri;
			if (Uri.TryCreate(uriString, UriKind.Absolute, out uri))
			{
				WebClient client = new WebClient();

				string
					username = null,
					password = null;

				// Determine if credentials were included in the Uri
				// NOTE: including credential information in the uri 
				// is not recommended practice for numerous security reasons;
				// however, since some people have requested the ability to
				// do so, we'll handle it.
				if (!string.IsNullOrEmpty(uri.UserInfo))
				{
					// Extract the embedded credentials from the Uri
					string[] parts = uri.UserInfo.Split(':');
					if (parts.Length == 2)
					{
						username = parts[0];
						password = parts[1];
					}
				}

				// If username/password were provided, then include these credentials in our web request!
				if (username != null && password != null)
					client.Credentials = new System.Net.NetworkCredential(username, password);

				string iCalText = client.DownloadString(uri);
				CalendarPath = uriString;
				return ValidateText(iCalText, ValidationMode.url);
			}
			else
			{
				return Response.Output.Encoding.GetBytes("EXCEPTION: Invalid URI: " + uriString);
			}
		}

		private byte[] ValidateText(string text, ValidationMode mode)
		{
			string id = DateTime.UtcNow.Ticks.ToString();
			var bytes = AsyncValidate(Encoding.UTF8.GetByteCount(text), new StringReader(text), id, mode);
			return bytes;
		}

		private byte[] ValidateStream(Stream s, ValidationMode mode)
		{
			string id = DateTime.UtcNow.Ticks.ToString();
			var bytes = AsyncValidate(s.Length, new StreamReader(s), id, mode);
			return bytes;
		}


		private byte[] AsyncValidate(long byte_count, TextReader tr, string id, ValidationMode mode)
		{
			var validator = new AsyncValidator(Validate);
			var result = validator.BeginInvoke(byte_count, tr, id, null, null);
			var seconds = 2;
			Thread.Sleep(seconds * 1000);
			if (result.IsCompleted)
			{
				var bytes = validator.EndInvoke(result);
				return bytes;
			}
			else
			{
				RedirectToProgressPage(mode, id, "0");
			}
			return new byte[0];
		}

		protected void SetupLanguage()
		{
			bool foundLanguage = ResourceManager.Initialize(DocumentProvider, false);

			if (!foundLanguage)
			{
				// Force initialization to English
				ResourceManager.Initialize(DocumentProvider, true);
			}
		}

		protected IValidationRuleset LoadRuleset()
		{
			// Load some rulesets
			XmlValidationRulesetLoader loader = new XmlValidationRulesetLoader(DocumentProvider);
			List<IValidationRuleset> rulesets = new List<IValidationRuleset>(loader.Load());

			// Determine a validation ruleset to use
			string validatorName = "Strict_2_0";
			/*if (_Arguments.Contains(_ValidatorArgument))
				validatorName = _Arguments[_ValidatorArgument].Value;*/

			// Select the ruleset
			IValidationRuleset selectedRuleset = rulesets.Find(
				delegate(IValidationRuleset rs)
				{
					return string.Equals(rs.Name, validatorName, StringComparison.CurrentCultureIgnoreCase);
				}
			);

			return selectedRuleset;
		}

		protected IValidationSerializer GetSerializer()
		{
			IValidationSerializer serializer = new XmlValidationSerializer(ResourceManager, DocumentProvider);

			if (serializer != null)
			{
				serializer.ApplicationName = "icalvalid";
				serializer.ApplicationUrl = "http://icalvalid.cloudapp.net";
				serializer.ApplicationVersion = Assembly.GetExecutingAssembly().GetName().Version;
			}

			return serializer;
		}

		/*
        protected void SetupStream(IValidationSerializer serializer, out Stream stream, out Encoding encoding)
        {
            Response.ContentType = "text/xml";
           // stream = Response.OutputStream;
			stream = new MemoryStream();
            encoding = Response.Output.Encoding;
        }*/

		protected byte[] Validate(long byteCount, TextReader tr, string id)
		{
			byte[] bytes = new byte[0];

			try
			{
				IValidationSerializer serializer = GetSerializer();

				if (serializer != null)
				{
					string iCalText = tr.ReadToEnd();

					if (SelectedRuleset != null)
					{
						serializer.Ruleset = SelectedRuleset;

						RulesetValidator rulesetValidator = new RulesetValidator(ResourceManager, SelectedRuleset, iCalText);

						/* log rulenames to a queue, with % completion as queue metadata
						 
						var q = "q" + DateTime.UtcNow.Ticks.ToString();
						this.queue_name.Text = q;
						var connection_string = RoleEnvironment.GetConfigurationSettingValue("DataConnectionString");
						var queue_logger = new QueueLogger(q, connection_string, "0"); // "0" is initial % completion
						var rulesetValidator = new RulesetValidator(ResourceManager, queue_logger, SelectedRuleset, iCalText);
						 */

						/* log rulenames to console
						  
						 * var logger = new ConsoleLogger();
						 * 	var rulesetValidator = new RulesetValidator(ResourceManager, logger, SelectedRuleset, iCalText);
						 */


						if (rulesetValidator != null)
						{
							serializer.ValidationResults = rulesetValidator.Validate();

							// Set the original text for the validation
							serializer.ValidationResults.CalendarText = iCalText;
							serializer.ValidationResults.CalendarPath = CalendarPath;
							serializer.ValidationResults.ByteCount = byteCount;
						}

						Stream stream = new MemoryStream();

						try
						{
							serializer.Serialize(stream, encoding);
							bytes = new byte[stream.Length];
							stream.Seek(0, 0);
							stream.Read(bytes, 0, (int)stream.Length);
						}
						catch (Exception e)
						{
							bytes = ExceptionMessage(e, encoding);
						}
						finally
						{
							stream.Close();
						}
					}
				}
			}

			catch (Exception e)
			{
				bytes = ExceptionMessage(e, encoding);
			}

			finally
			{
				var results_blob = MakeResultsBlob(id);
				results_blob.UploadByteArray(bytes);
			}

			return bytes;
		}

		private byte[] ExceptionMessage(Exception exception, Encoding encoding)
		{
			var msg = "EXCEPTION: " + exception.Message + exception.StackTrace;
			byte[] bytes = encoding.GetBytes(msg);
			return bytes;
		}

		#endregion

		#region Event Handlers

		protected void btnUpload_Click(object sender, EventArgs e)
		{
			try
			{
				// Get the HttpFileCollection
				HttpFileCollection hfc = Request.Files;
				if (hfc.Count > 0)
				{
					HttpPostedFile hpf = hfc[0];
					CalendarPath = hpf.FileName;
					var bytes = ValidateStream(hpf.InputStream, ValidationMode.file);
					SendExceptionOrResult(bytes);
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		private void SendExceptionOrResult(byte[] bytes)
		{
			var str = encoding.GetString(bytes);
			if (str.StartsWith("EXCEPTION"))
			{
				var tmpl = @"<html>
<head><title>icalvalid error</title></head>
<body>
<p>Sorry about that. The following exception has been logged and reported.</p><p>{0}</p>
<p><a href=""/"">Try another?</a></p>
</body>
</html>";
				var page = string.Format(tmpl, str);
				SendPage(encoding.GetBytes(page), "text/html");
			}
			else
				SendPage(bytes, "text/xml");
		}

		protected void btnValidateUrl_Click(object sender, EventArgs e)
		{
			var bytes = ValidateUri(tbUrl.Text);
			SendExceptionOrResult(bytes);
		}

		protected void btnValidateSnippet_Click(object sender, EventArgs e)
		{
			var bytes = ValidateText(tbSnippet.Text, ValidationMode.snippet);
			SendExceptionOrResult(bytes);
		}

		#endregion
	}

	public static class BlobExtensions
	{
		public static bool Exists(this CloudBlob blob)
		{
			try
			{
				blob.FetchAttributes();
				return true;
			}
			catch (StorageClientException e)
			{
				if (e.ErrorCode == StorageErrorCode.ResourceNotFound)
				{
					return false;
				}
				else
				{
					throw;
				}
			}
		}
	}
}
