using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.DocumentValidator;
using System.Xml;

namespace Campus.DocumentValidator_Test
{
    class ErrorCapturer
    {
        public DocumentValidate Validator { get; private set; }

        private Dictionary<int, XmlElement> Errors = new Dictionary<int, XmlElement>();

        public ErrorCapturer(DocumentValidate validator)
        {
            validator.AutoCorrect += new EventHandler<AutoCorrectEventArgs>(Validator_AutoCorrect);
            validator.ErrorCaptured += new EventHandler<ErrorCapturedEventArgs>(Validator_ErrorCaptured);
        }

        private void Validator_ErrorCaptured(object sender, ErrorCapturedEventArgs e)
        {
            EnsureRowExists(e.Row.Position);

            XmlElement row = Errors[e.Row.Position];

            XmlElement field = row.OwnerDocument.CreateElement("Message");
            row.AppendChild(field);

            field.SetAttribute("VType", e.ValidatorType.ToString());
            field.SetAttribute("Name", e.FieldName);
            field.SetAttribute("ErrorType", e.ErrorType.ToString());
            field.SetAttribute("Description", e.Description);
        }

        private void Validator_AutoCorrect(object sender, AutoCorrectEventArgs e)
        {
            EnsureRowExists(e.Row.Position);

            XmlElement row = Errors[e.Row.Position];

            XmlElement field = row.OwnerDocument.CreateElement("Message");
            row.AppendChild(field);

            field.SetAttribute("VType", e.ValidatorType.ToString());
            field.SetAttribute("Name", e.FieldName);
            field.SetAttribute("ErrorType", "Correct");
            field.SetAttribute("OldValue", e.OldValue);
            field.SetAttribute("NewValue", e.NewValue);
        }

        private void EnsureRowExists(int position)
        {
            if (!Errors.ContainsKey(position))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<Row/>");
                doc.DocumentElement.SetAttribute("Position", position.ToString());
                Errors.Add(position, doc.DocumentElement);
            }
        }

        public XmlElement GetErrorReport()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Document/>");

            foreach (XmlElement each in Errors.Values)
            {
                XmlNode row = doc.ImportNode(each, true);

                doc.DocumentElement.AppendChild(row);
            }

            return doc.DocumentElement;
        }
    }
}
