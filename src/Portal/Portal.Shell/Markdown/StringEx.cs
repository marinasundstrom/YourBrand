using System.Xml;
using System.Xml.XPath;

namespace YourBrand.Portal.Markdown
{
    public static class StringEx
    {
        public static string TruncateHtml(this string input, int length = 300,
                                   string ommission = "...")
        {
            if (input == null || input.Length < length)
                return input;
            int iNextSpace = input.LastIndexOf(" ", length);
            return string.Format("{0}" + ommission, input.Substring(0, (iNextSpace > 0) ?
                                                                  iNextSpace : length).Trim());
        }

        public static string StripTags(this string markup)
        {
            try
            {
                StringReader sr = new StringReader(markup);
                XPathDocument doc;
                using (XmlReader xr = XmlReader.Create(sr,
                                   new XmlReaderSettings()
                                   {
                                       ConformanceLevel = ConformanceLevel.Fragment
                                       // for multiple roots
                                   }))
                {
                    doc = new XPathDocument(xr);
                }

                return doc.CreateNavigator().Value; // .Value is similar to .InnerText of  
                                                    //  XmlDocument or JavaScript's innerText
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}