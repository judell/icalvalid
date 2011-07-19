using System;
using System.Collections.Generic;
using System.Text;
using DDay.iCal.Validator.Xml;

namespace DDay.iCal.Validator
{
    public interface IResourceManager
    {
        string CurrentLanguageIdentifier { get; }
        bool Initialize(IXmlDocumentProvider docProvider, bool forceEnglishOnNotFound);
        bool Initialize(IXmlDocumentProvider docProvider, string language);
        string GetString(string key);
        string GetError(string key);
        string[] GetResolutions(string key);
    }
}
