namespace UltimateValidation
{
    partial class App
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
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDataSourcePath = new System.Windows.Forms.TextBox();
            this.btnValidate = new System.Windows.Forms.Button();
            this.lblSelectFile = new System.Windows.Forms.Button();
            this.Progress = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1855, 1614);
            this.button1.Margin = new System.Windows.Forms.Padding(11, 12, 11, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(275, 90);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 52);
            this.label1.Margin = new System.Windows.Forms.Padding(11, 0, 11, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(168, 47);
            this.label1.TabIndex = 1;
            this.label1.Text = "資料目錄";
            // 
            // txtDataSourcePath
            // 
            this.txtDataSourcePath.Location = new System.Drawing.Point(194, 47);
            this.txtDataSourcePath.Margin = new System.Windows.Forms.Padding(11, 12, 11, 12);
            this.txtDataSourcePath.Name = "txtDataSourcePath";
            this.txtDataSourcePath.Size = new System.Drawing.Size(644, 57);
            this.txtDataSourcePath.TabIndex = 2;
            // 
            // btnValidate
            // 
            this.btnValidate.Location = new System.Drawing.Point(670, 147);
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(231, 66);
            this.btnValidate.TabIndex = 3;
            this.btnValidate.Text = "開始驗證";
            this.btnValidate.UseVisualStyleBackColor = true;
            this.btnValidate.Click += new System.EventHandler(this.btnValidate_Click);
            // 
            // lblSelectFile
            // 
            this.lblSelectFile.Location = new System.Drawing.Point(852, 42);
            this.lblSelectFile.Name = "lblSelectFile";
            this.lblSelectFile.Size = new System.Drawing.Size(67, 66);
            this.lblSelectFile.TabIndex = 3;
            this.lblSelectFile.Text = "...";
            this.lblSelectFile.UseVisualStyleBackColor = true;
            this.lblSelectFile.Click += new System.EventHandler(this.lblSelectFile_Click);
            // 
            // Progress
            // 
            this.Progress.Location = new System.Drawing.Point(28, 147);
            this.Progress.Name = "Progress";
            this.Progress.Size = new System.Drawing.Size(634, 66);
            this.Progress.TabIndex = 4;
            // 
            // App
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(931, 242);
            this.Controls.Add(this.Progress);
            this.Controls.Add(this.lblSelectFile);
            this.Controls.Add(this.btnValidate);
            this.Controls.Add(this.txtDataSourcePath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Font = new System.Drawing.Font("Microsoft JhengHei", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Margin = new System.Windows.Forms.Padding(11, 12, 11, 12);
            this.Name = "App";
            this.Text = "Ultimate Validation";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDataSourcePath;
        private System.Windows.Forms.Button btnValidate;
        private System.Windows.Forms.Button lblSelectFile;
        private System.Windows.Forms.ProgressBar Progress;

    }
}

