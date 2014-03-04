namespace Campus.Windows
{
    partial class XTextViewer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            ActiproSoftware.SyntaxEditor.Document document2 = new ActiproSoftware.SyntaxEditor.Document();
            this.XmlLanguage = new ActiproSoftware.SyntaxEditor.Addons.Xml.XmlSyntaxLanguage(this.components);
            this.MainTextBox = new ActiproSoftware.SyntaxEditor.SyntaxEditor();
            this.SuspendLayout();
            // 
            // MainTextBox
            // 
            this.MainTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            document2.Language = this.XmlLanguage;
            this.MainTextBox.Document = document2;
            this.MainTextBox.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainTextBox.Location = new System.Drawing.Point(0, 0);
            this.MainTextBox.Name = "MainTextBox";
            this.MainTextBox.Size = new System.Drawing.Size(581, 468);
            this.MainTextBox.TabIndex = 0;
            // 
            // XTextViewer
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(581, 468);
            this.Controls.Add(this.MainTextBox);
            this.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.Name = "XTextViewer";
            this.Text = "XTextViewer";
            this.ResumeLayout(false);

        }

        #endregion

        private ActiproSoftware.SyntaxEditor.SyntaxEditor MainTextBox;
        private ActiproSoftware.SyntaxEditor.Addons.Xml.XmlSyntaxLanguage XmlLanguage;
    }
}