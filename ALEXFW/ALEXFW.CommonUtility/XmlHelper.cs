using System.Collections.Generic;
using System.Xml;

namespace ALEXFW.CommonUtility
{
    public static class XmlHelper
    {
        public static string GetXml(IDictionary<string, string> directory)
        {
            var doc = new XmlDocument();
            doc.CreateXmlDeclaration("1.0", "utf-8", "yes");
            var root = doc.CreateElement("xml");
            doc.AppendChild(root);
            foreach (var kv in directory)
            {
                var element = doc.CreateElement(kv.Key);
                element.InnerText = kv.Value;
                root.AppendChild(element);
            }
            return doc.OuterXml;
        }
    }
}