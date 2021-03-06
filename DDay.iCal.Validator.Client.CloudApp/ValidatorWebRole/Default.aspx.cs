﻿using System;
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
        public string ValidatorVersion { get; protected set; }
        public string ValidatorWebRoleVersion { get; protected set; }
		public IXmlDocumentProvider DocumentProvider { get; protected set; }
		public IValidationRuleset SelectedRuleset { get; protected set; }
		public IResourceManager ResourceManager { get; protected set; }
		public Debug Debug { get; protected set; }
		public string CalendarPath { get; set; }

        Encoding encoding = UTF8Encoding.UTF8;

		private delegate byte[] AsyncValidator(long byte_count, TextReader tr, string id);
		private string progress_fmt = "/?mode={0}&uri={1}&id={2}&progress={3}";

		private enum ValidationMode { url, file, snippet };



		protected void Page_Load(object sender, EventArgs e)
		{

                Utils.LogMsg("request", Request.RawUrl, String.Format("{0} {1}", Request.QueryString, Request.UrlReferrer));
                Server.ScriptTimeout = 120;

                Assembly ddayical = Assembly.Load("DDay.iCal");
                DDayICalVersion = ddayical.GetName().Version.ToString();

                Assembly validator = Assembly.Load("DDay.iCal.Validator");
                ValidatorVersion = validator.GetName().Version.ToString();

                Assembly webrole = Assembly.Load("ValidatorWebRole");
                ValidatorWebRoleVersion = webrole.GetName().Version.ToString();

                var setup_result = Setup();
                if (setup_result.Length > 0)
                {
                    SendPage(setup_result, "text/html");
                    return;
                }

                if (Request["check_completion"] != null)
                {
                    var blob = Utils.MakeResultsBlob(Request["check_completion"]);
                    if (blob.Exists())
                        SendPage(encoding.GetBytes("YES"), "text/plain");
                    else
                        SendPage(encoding.GetBytes("NO"), "text/plain");
                    return;
                }

                if (Request["id"] != null)
                {
                    var blob = Utils.MakeResultsBlob(Request["id"]);
                    if (blob.Exists())
                    {
                        var bytes = blob.DownloadByteArray();
                        SendResult(bytes);
                    }
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
                    var script_text = "<script type='text/javascript'>RunProgress('" + id + "');</script>";
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

		private void SendPage(byte[] bytes, string content_type)
		{
            try
            {
                Response.ContentType = content_type;
                var page = Response.Output.Encoding.GetString(bytes);
                Response.Write(page);
                //this.Context.ApplicationInstance.CompleteRequest();
                //Response.End();
            }
            catch (Exception e)
            {
                Utils.LogMsg("exception", "SendPage", e.Message);
            }
            finally
            {
                Response.End();
            }
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
                Utils.LogMsg("exception", e.Message, e.StackTrace);
				bytes = Response.Output.Encoding.GetBytes(e.Message);
			}

			return bytes;
		}

		protected byte[] ValidateUri(string uriString)
		{
            Utils.LogMsg("info", "ValidateUri", uriString);
			Uri uri;
			if (Uri.TryCreate(uriString, UriKind.Absolute, out uri) == false)
			{
                var msg = "malformed URL: " + uriString;
				var exception = new Exception(msg);
                Utils.LogMsg("exception", "ValidateURI", msg);
		        return ExceptionMessage(exception);
			}

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

			WebClient client = new WebClient();

			// If username/password were provided, then include these credentials in our web request!
			if (username != null && password != null)
				client.Credentials = new System.Net.NetworkCredential(username, password);

			string iCalText = client.DownloadString(uri);
			CalendarPath = uriString;
			return ValidateText(iCalText, ValidationMode.url, save_results: true, async: true);
		}

		private byte[] ValidateText(string text, ValidationMode mode, bool save_results, bool async)
		{
            Utils.LogMsg("info", "ValidateText", null);
			var sr = new StringReader(text);
			var id = MakeId(save_results);
            Utils.LogMsg("info", "ValidateText", id);
			var count = Encoding.UTF8.GetByteCount(text);
			byte[] bytes;
			bytes = ValidateHelper(mode, async, sr, id, count);
			return bytes;
		}

		private byte[] ValidateStream(Stream s, ValidationMode mode, bool save_results, bool async)
		{
			string id = MakeId(save_results);
            Utils.LogMsg("info", "ValidateStream", id);
			var sr = new StreamReader(s);
			var count = s.Length;
			byte[] bytes = ValidateHelper(mode, async, sr, id, count);
			return bytes;
		}

		private byte[] ValidateHelper(ValidationMode mode, bool async, TextReader tr, string id, long count)
		{
            // async = false;  
			byte[] bytes;
			if (async)
				bytes = AsyncValidate(count, tr, id, mode);
			else
				bytes = Validate(count, tr, id);
			return bytes;
		}

        // ids prefixed with _ will be expunged by the cleanup thread
		private static string MakeId(bool save_results)
		{
			string id = DateTime.UtcNow.Ticks.ToString();
			if (save_results == false) id = "_" + id;
			return id;
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
            Utils.LogMsg("info", "Validate", id);
			byte[] bytes = new byte[0];

			try
			{
				IValidationSerializer serializer = GetSerializer();

				if (serializer != null)
				{
					string iCalText = tr.ReadToEnd();

					try
					{
						var ms = new MemoryStream(encoding.GetBytes(iCalText));
						var ical = DDay.iCal.iCalendar.LoadFromStream(ms);
						ms.Close();
					}
					catch (Exception e)
					{
                        Utils.LogMsg("exception", "Validate", e.Message + e.StackTrace);
						bytes = ExceptionMessage(e);
						return bytes;
					}

					if (SelectedRuleset != null)
					{
						serializer.Ruleset = SelectedRuleset;

						RulesetValidator rulesetValidator = new RulesetValidator(ResourceManager, SelectedRuleset, iCalText);

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
							var permalink = Utils.MakePermalinkUrl(id);
							serializer.Serialize(stream, encoding, permalink);
							bytes = new byte[stream.Length];
							stream.Seek(0, 0);
							stream.Read(bytes, 0, (int)stream.Length);
						}
						catch (Exception e)
						{
							bytes = ExceptionMessage(e);
                            Utils.LogMsg("exception", "Validate", e.Message + e.StackTrace);
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
				bytes = ExceptionMessage(e);
                Utils.LogMsg("exception", "Validate", e.Message + e.StackTrace);
			}

			finally
			{
				var results_blob = Utils.MakeResultsBlob(id);
                results_blob.UploadByteArray(bytes);
                results_blob.Properties.ContentType = "text/xml";
                results_blob.SetProperties();
			}

			return bytes;
		}

		private byte[] ExceptionMessage(Exception e)
		{
			var template = Encoding.UTF8.GetString(new WebClient().DownloadData(Utils.unparseable_message_uri));
			var html = string.Format(template,
				e.Message,
				e.StackTrace,
				e.InnerException);
			byte[] bytes = encoding.GetBytes(html);
			return bytes;
		}

		#endregion

		#region Event Handlers

		protected void btnUpload_Click(object sender, EventArgs e)
		{
            Utils.LogMsg("info", "Validate uploaded file", null);
				// Get the HttpFileCollection
			HttpFileCollection hfc = Request.Files;
			if (hfc.Count > 0)
			{
				HttpPostedFile hpf = hfc[0];
				CalendarPath = hpf.FileName;
				var bytes = ValidateStream(hpf.InputStream, ValidationMode.file, save_results: false, async: true);
    			SendResult(bytes);
			}
		}

		private void SendResult(byte[] bytes)
		{
            string str = String.Empty;

            bool is_html = false;
            bool is_xml = false;

            try
            {
               // Utils.LogMsg("info", "bytes len", bytes.Length.ToString());
                if (bytes.Length > 10)
                {
                    var hex = BitConverter.ToString(bytes.Take(10).ToArray());
                    Utils.LogMsg("info", hex, null);
                }
                str = Encoding.UTF8.GetString(bytes);
                var bom = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                //Utils.LogMsg("info", "str orig len", str.Length.ToString());
                str = str.Remove(0, bom.Length);
                //Utils.LogMsg("info", "str new len", str.Length.ToString());
                is_html = str.StartsWith("<html>");
                is_xml = str.StartsWith("<?xml");
                Utils.LogMsg("info", "html? " + is_html.ToString() + " xml: " + is_xml.ToString(), null);
            }
            catch (Exception e)
            {
                Utils.LogMsg("exception", "SendResult", e.Message + e.StackTrace);
                SendPage(ExceptionMessage(e), "text/plain");
            }

			if (is_html)
				SendPage(bytes, "text/html");

            else if ( is_xml )
                SendPage(bytes, "text/xml");

            else
                SendPage(bytes, "text/plain");
		}

		protected void btnValidateUrl_Click(object sender, EventArgs e)
		{
			var bytes = ValidateUri(tbUrl.Text);
			SendResult(bytes);
		}

		protected void btnValidateSnippet_Click(object sender, EventArgs e)
		{
			var bytes = ValidateText(tbSnippet.Text, ValidationMode.snippet, save_results: false, async: true);
			SendResult(bytes);
		}

		#endregion
	}

}

