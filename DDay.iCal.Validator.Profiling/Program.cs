using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using DDay.iCal.Serialization;
using DDay.iCal.Validator.RFC5545;
using DDay.iCal.Validator.Xml;

namespace DDay.iCal.Validator.Profiling
{
    public class Program
    {
        static string _CalendarPath = null;
        static Debug Debug = new Debug();
        static IResourceManager ResourceManager = new ResourceManager();

        static void Main(string[] args)
        {
            try
            {
                Debug.Initialize();

                // Adjust default serialization settings
                ISerializationSettings settings = SerializationContext.Default.GetService<ISerializationSettings>();
                settings.ParsingMode = ParsingModeType.Loose;
                settings.EnsureAccurateLineNumbers = true;
                settings.StoreExtraSerializationData = true;

                for (int i = 0; i < 10000; i++)
                {
                    IXmlDocumentProvider docProvider = null;

                    // Initialize our xml document provider
                    if (File.Exists("icalvalidSchema.zip"))
                        docProvider = new ZipXmlDocumentProvider("icalvalidSchema.zip");
                    else if (Directory.Exists("icalvalidSchema"))
                        docProvider = new DirectoryXmlDocumentProvider("icalvalidSchema");
                    else
                        throw new Exception("A valid schema directory or zip file could not be located!");

                    IValidationRuleset selectedRuleset = null;

                    // Setup the language to use for validation/tests
                    SetupLanguage(docProvider);
                    selectedRuleset = LoadRuleset(docProvider);

                    ValidateFile(@"Calendars\Test1.ics", docProvider, selectedRuleset);
                    ValidateFile(@"Calendars\Test2.ics", docProvider, selectedRuleset);
                    ValidateFile(@"Calendars\Test3.ics", docProvider, selectedRuleset);
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void SetupLanguage(IXmlDocumentProvider docProvider)
        {
            bool foundLanguage = false;

            // Initialize our resource manager with our current culture
            foundLanguage = ResourceManager.Initialize(docProvider, false);

            if (!foundLanguage)
            {
                Console.WriteLine("Could not find the selected language; using English instead...");

                // Force initialization to English
                ResourceManager.Initialize(docProvider, true);
            }
        }

        static IValidationRuleset LoadRuleset(IXmlDocumentProvider docProvider)
        {
            // Load some rulesets
            XmlValidationRulesetLoader loader = new XmlValidationRulesetLoader(docProvider);

            IValidationRuleset[] rulesetArray = loader.Load();
            if (rulesetArray == null)
                throw new Exception("The validation rulesets could not be loaded.  Are you sure the icalvalid schema files are present?");

            List<IValidationRuleset> rulesets = new List<IValidationRuleset>(rulesetArray);

            // Determine a validation ruleset to use
            string validatorName = "Strict_2_0";

            // Select the ruleset
            IValidationRuleset selectedRuleset = rulesets.Find(
                delegate(IValidationRuleset rs)
                {
                    return string.Equals(rs.Name, validatorName, StringComparison.CurrentCultureIgnoreCase);
                }
            );

            return selectedRuleset;
        }

        static void SetupStream(IValidationSerializer serializer, out Stream stream, out Encoding encoding)
        {
            stream = new MemoryStream();
            encoding = Encoding.Unicode;
        }

        static IValidationSerializer GetSerializer(IXmlDocumentProvider docProvider)
        {
            string format = "Text";

            Type type = Type.GetType("DDay.iCal.Validator.Serialization." + format + "ValidationSerializer, DDay.iCal.Validator", false, true);
            if (type != null)
            {
                IValidationSerializer serializer = null;

                ConstructorInfo ci = type.GetConstructor(new Type[] { typeof(IResourceManager) });
                if (ci != null)
                    serializer = ci.Invoke(new object[] { ResourceManager }) as IValidationSerializer;
                else
                {
                    ci = type.GetConstructor(new Type[] { typeof(IResourceManager), typeof(IXmlDocumentProvider) });
                    if (ci != null)
                        serializer = ci.Invoke(new object[] { ResourceManager, docProvider }) as IValidationSerializer;
                }

                if (serializer != null)
                {
                    serializer.ApplicationName = "icalvalid Console";
                    serializer.ApplicationUrl = "http://icalvalid.cloudapp.net";
                    serializer.ApplicationVersion = Assembly.GetExecutingAssembly().GetName().Version;
                }

                return serializer;
            }

            return null;
        }

        static void ValidateFile(string filename, IXmlDocumentProvider docProvider, IValidationRuleset selectedRuleset)
        {
            IValidationSerializer serializer = GetSerializer(docProvider);
            bool needsMoreArguments = false;

            if (serializer != null)
            {
                string iCalText = null;
                long byteCount = 0;

                if (selectedRuleset != null)
                {
                    serializer.Ruleset = selectedRuleset;

                    // Load the calendar from a local file
                    Console.Write(ResourceManager.GetString("loadingCalendar"));
                    FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    if (fs != null)
                    {
                        // Get the calendar path so it can be reported later
                        _CalendarPath = Path.GetFullPath(filename);

                        StreamReader sr = new StreamReader(fs);
                        byteCount = fs.Length;
                        iCalText = sr.ReadToEnd();
                        sr.Close();
                    }
                    Console.WriteLine(ResourceManager.GetString("Done"));
                    
                    if (iCalText == null)
                    {
                        throw new Exception(ResourceManager.GetString("calendarNotFound"));
                    }
                    else
                    {
                        RulesetValidator rulesetValidator = new RulesetValidator(ResourceManager, selectedRuleset, iCalText);

                        if (rulesetValidator != null)
                        {
                            Console.Write(string.Format(
                                ResourceManager.GetString("validatingCalendar"),
                                ResourceManager.GetString(selectedRuleset.NameString)
                            ));

                            serializer.ValidationResults = rulesetValidator.Validate();

                            // Set the original text for the validation
                            serializer.ValidationResults.CalendarPath = _CalendarPath;
                            serializer.ValidationResults.CalendarText = iCalText;
                            serializer.ValidationResults.ByteCount = byteCount;

                            Console.WriteLine(ResourceManager.GetString("done"));
                        }
                    }

                    Stream stream;
                    Encoding encoding;
                    SetupStream(serializer, out stream, out encoding);

                    try
                    {
						serializer.Serialize(stream, encoding, permalink: "");
                    }
                    finally
                    {
                        stream.Close();
                    }
                }
            }
        }
    }
}
