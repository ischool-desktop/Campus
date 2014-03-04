using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using ActiproSoftware.SyntaxEditor.Addons.PlainText;

namespace Campus.Windows
{
    public partial class XTextViewer : BaseForm
    {
        public XTextViewer()
        {
            InitializeComponent();
        }

        public static void ViewText(string data)
        {
            XTextViewer viewer = new XTextViewer();
            viewer.MainTextBox.Document.Language = new PlainTextSyntaxLanguage();
            viewer.MainTextBox.Text = data;
            viewer.ShowDialog();
        }

        public static void ViewXml(string data)
        {
            XTextViewer viewer = new XTextViewer();
            viewer.MainTextBox.Text = data;
            viewer.Format();
            viewer.ShowDialog();
        }

        public void Format()
        {
            XmlLanguage.FormatDocument(MainTextBox.Document);
        }
    }
}
