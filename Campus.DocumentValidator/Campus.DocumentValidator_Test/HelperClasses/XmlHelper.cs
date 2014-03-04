using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Campus.DocumentValidator_Test
{
    class XmlHelper
    {
        public XmlHelper(XmlElement elm)
        {
            if (elm == null) throw new ArgumentException("基礎 XmlElement 物件不可為 Null。", "elm");

            InnerElement = elm;
        }

        public XmlElement InnerElement { get; set; }

        public string GetString(string xpath)
        {
            XmlNode elm = InnerElement.SelectSingleNode(xpath);

            if (elm == null)
                return string.Empty;
            else
                return elm.InnerText;
        }

        public bool GetBoolean(string xpath, bool defaultValue)
        {
            string value = GetString(xpath);
            bool result;

            if (bool.TryParse(value, out result))
                return result;
            else
                return defaultValue;
        }

        public int GetInteger(string xpath, int defaultValue)
        {
            string value = GetString(xpath);
            int result;

            if (int.TryParse(value, out result))
                return result;
            else
                return defaultValue;
        }

        public short GetShort(string xpath, short defaultValue)
        {
            string value = GetString(xpath);
            short result;

            if (short.TryParse(value, out result))
                return result;
            else
                return defaultValue;
        }

        public DateTime GetDateTime(string xpath, DateTime defaultValue)
        {
            string value = GetString(xpath);
            DateTime result;

            if (DateTime.TryParse(value, out result))
                return result;
            else
                return defaultValue;
        }
    }
}
