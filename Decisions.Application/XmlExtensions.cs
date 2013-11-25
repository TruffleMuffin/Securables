﻿using System.Xml;
using System.Xml.Linq;

namespace Decisions.Application
{
    internal static class XmlExtensions
    {
        public static XmlNode ToXmlNode(this XElement element)
        {
            using (var xmlReader = element.CreateReader())
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlReader);
                return xmlDoc;
            }
        }
    }
}