namespace Campus.Message
{
    partial class UserControl1
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.自定內文 = new System.Windows.Forms.LinkLabel();
            this.自定標題 = new DevComponents.DotNetBar.LabelX();
            this.自定圖示 = new DevComponents.DotNetBar.Controls.ReflectionImage();
            this.SuspendLayout();
            // 
            // 自定內文
            // 
            this.自定內文.BackColor = System.Drawing.Color.Transparent;
            this.自定內文.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.自定內文.Location = new System.Drawing.Point(86, 57);
            this.自定內文.Name = "自定內文";
            this.自定內文.Size = new System.Drawing.Size(231, 54);
            this.自定內文.TabIndex = 5;
            this.自定內文.TabStop = true;
            this.自定內文.Text = "內文";
            this.自定內文.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.自定內文_LinkClicked);
            // 
            // 自定標題
            // 
            this.自定標題.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.自定標題.BackgroundStyle.Class = "";
            this.自定標題.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.自定標題.Font = new System.Drawing.Font("新細明體", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.自定標題.Location = new System.Drawing.Point(79, 12);
            this.自定標題.Name = "自定標題";
            this.自定標題.Size = new System.Drawing.Size(238, 33);
            this.自定標題.TabIndex = 7;
            this.自定標題.Text = "<b>標題</b>";
            // 
            // 自定圖示
            // 
            this.自定圖示.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.自定圖示.BackgroundStyle.Class = "";
            this.自定圖示.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.自定圖示.Image = global::Campus.Message.Properties.Resources.speech_balloon_64;
            this.自定圖示.Location = new System.Drawing.Point(5, 15);
            this.自定圖示.Name = "自定圖示";
            this.自定圖示.Size = new System.Drawing.Size(64, 96);
            this.自定圖示.TabIndex = 6;
            // 
            // UserControl1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.自定標題);
            this.Controls.Add(this.自定圖示);
            this.Controls.Add(this.自定內文);
            this.Name = "UserControl1";
            this.Size = new System.Drawing.Size(324, 124);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.LinkLabel 自定內文;
        public DevComponents.DotNetBar.Controls.ReflectionImage 自定圖示;
        public DevComponents.DotNetBar.LabelX 自定標題;
    }
}
