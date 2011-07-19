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

        protected void Page_Load(object sender, EventArgs e)
        {
            Assembly ddayical = Assembly.Load("DDay.iCal");
            DDayICalVersion = ddayical.GetName().Version.ToString();

            Setup();

            // Validate the URI if it was passed via query string.
            if (Request["uri"] != null)
                ValidateUri(Request["uri"]);
        }

        #region Protected Methods

        protected void Setup()
        {
            Debug = new Debug();
            Debug.Initialize();

            ISerializationSettings settings = SerializationContext.Default.GetService<ISerializationSettings>();
            settings.ParsingMode = ParsingModeType.Loose;
            settings.EnsureAccurateLineNumbers = true;
            settings.StoreExtraSerializationData = true;

            DocumentProvider = new DirectoryXmlDocumentProvider(Path.Combine(Request.PhysicalApplicationPath, "icalvalidSchema"));

            SelectedRuleset = null;
            bool successfulLoad = true;
            try
            {
                ResourceManager = new ResourceManager();

                // Setup the language to use for validation/tests
                SetupLanguage();
                SelectedRuleset = LoadRuleset();
            }
            catch (ValidationRuleLoadException)
            {
                successfulLoad = false;
            }
        }

        protected void ValidateUri(string uriString)
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
                ValidateText(iCalText);
            }
            else
            {
                // FIXME: throw an exception?
            }
        }

        protected void ValidateStream(Stream s)
        {
            Validate(s.Length, new StreamReader(s));

            // End the response, so we don't give garbage HTML along with
            // the contents of the response.
            Response.End();
        }

        protected void ValidateText(string text)
        {
            Validate(Encoding.UTF8.GetByteCount(text), new StringReader(text));

            // End the response, so we don't give garbage HTML along with
            // the contents of the response.
            Response.End();
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

        protected void SetupStream(IValidationSerializer serializer, out Stream stream, out Encoding encoding)
        {
            Response.ContentType = "text/xml";
            stream = Response.OutputStream;
            encoding = Response.Output.Encoding;
        }

        protected void Validate(long byteCount, TextReader tr)
        {
            IValidationSerializer serializer = GetSerializer();

            if (serializer != null)
            {
                string iCalText = tr.ReadToEnd();

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

                    Stream stream;
                    Encoding encoding;
                    SetupStream(serializer, out stream, out encoding);

                    try
                    {
                        serializer.Serialize(stream, encoding);
                    }
                    finally
                    {
                        stream.Close();
                    }
                }
            }
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
                    ValidateStream(hpf.InputStream);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected void btnValidateUrl_Click(object sender, EventArgs e)
        {
            ValidateUri(tbUrl.Text);
        }

        protected void btnValidateSnippet_Click(object sender, EventArgs e)
        {
            ValidateText(tbSnippet.Text);
        }        

        #endregion        
    }
}
