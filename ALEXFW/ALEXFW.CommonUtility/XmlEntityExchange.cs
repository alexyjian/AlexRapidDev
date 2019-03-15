using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace ALEXFW.CommonUtility
{
    public static class XmlEntityExchange
    {
        /// <summary>
        ///     将XML转换为对象
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T ConvertXml2Entity<T>(Stream stream)
            where T : new()
        {
            var doc = XDocument.Load(stream);

            var obj = Activator.CreateInstance<T>();
            ReadElement(doc.Root, obj);
            return obj;
        }

        private static void ReadElement(XElement element, object obj)
        {
            var type = obj.GetType();
            foreach (var property in type.GetProperties())
            {
                var item = element.Element(property.Name);
                if (item == null)
                    continue;
                //Array
                //if (typeof(IList).IsAssignableFrom(property.PropertyType))
                //{
                //    property.PropertyType.
                //}
                //else
                //{
                var converter = TypeDescriptor.GetConverter(property.PropertyType);
                object value;
                //if (!property.PropertyType.IsValueType)
                //    value = Activator.CreateInstance(property.PropertyType);
                value = converter.ConvertFrom(item.Value);
                property.SetValue(obj, value);
                //}
            }
        }

        /// <summary>
        ///     构造微信消息
        /// </summary>
        /// <param name="t">对象实体</param>
        /// <returns>返回微信消息xml格式</returns>
        public static string ConvertEntityToXml<T>(T t)
            where T : new()
        {
            var doc = new XmlDocument();
            var root = doc.CreateElement("xml");
            doc.AppendChild(root);
            FillElement(root, t);
            return doc.OuterXml;
        }

        private static void FillElement(XmlElement element, object obj)
        {
            var type = obj.GetType();
            foreach (var property in type.GetProperties())
            {
                var value = property.GetValue(obj);
                if (value == null)
                    continue;
                //枚举
                if (typeof(IList).IsAssignableFrom(property.PropertyType))
                {
                    var listElement = element.OwnerDocument.CreateElement(property.Name);
                    foreach (var item in (IList)value)
                    {
                        if (item == null)
                            continue;
                        var itemElement = element.OwnerDocument.CreateElement("item");
                        if (item.GetType().IsValueType)
                            itemElement.InnerText = item.ToString();
                        else
                            FillElement(itemElement, item);
                        listElement.AppendChild(itemElement);
                    }
                    element.AppendChild(listElement);
                }
                else
                {
                    var propertyElement = element.OwnerDocument.CreateElement(property.Name);
                    propertyElement.InnerText = value.ToString();
                    element.AppendChild(propertyElement);
                }
            }
        }
    }
}