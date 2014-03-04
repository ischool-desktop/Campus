using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace Campus.Windows
{
    public partial class TextViewer : Form
    {
        public TextViewer()
        {
            InitializeComponent();
        }

        public static void ViewText(string data)
        {
            TextViewer viewer = new TextViewer();
            viewer.rtxtContent.Text = data;
            viewer.ShowDialog();
        }

        public static void ViewXml(string data)
        {
            TextViewer viewer = new TextViewer();
            viewer.rtxtContent.Text = Format(data);
            viewer.ShowDialog();
        }

        public static string Format(string xmlContent)
        {
            MemoryStream ms = new MemoryStream();

            XmlTextWriter writer = new XmlTextWriter(ms, System.Text.Encoding.UTF8);

            writer.Formatting = Formatting.Indented;
            writer.Indentation = 1;
            writer.IndentChar = '\t';

            XmlReader Reader = GetXmlReader(xmlContent);
            writer.WriteNode(Reader, true);
            writer.Flush();
            Reader.Close();

            ms.Position = 0;
            StreamReader sr = new StreamReader(ms, System.Text.Encoding.UTF8);

            string Result = sr.ReadToEnd();
            sr.Close();

            writer.Close();
            ms.Close();

            return Result;
        }

        private static XmlReader GetXmlReader(string XmlData)
        {
            XmlReaderSettings setting = new XmlReaderSettings();
            setting.IgnoreWhitespace = true;

            XmlReader Reader = XmlReader.Create(new StringReader(XmlData), setting);

            return Reader;
        }

    }
}
