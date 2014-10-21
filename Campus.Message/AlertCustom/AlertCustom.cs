using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Campus.Message
{
    /// <summary>
    /// Summary description for AlertCustom.
    /// </summary>
    public class AlertCustom : DevComponents.DotNetBar.Balloon
    {
        private DevComponents.DotNetBar.LabelX labelX1;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private FlowLayoutPanel flowLayoutPanel1;

        /// <summary>
        /// 計數器
        /// </summary>
        public Timer timer = new Timer();

        /// <summary>
        /// 到數時間
        /// </summary>
        private int timer_End { get; set; }

        /// <summary>
        /// 是否已經顯示於畫面上
        /// </summary>
        public bool IsShow { get; set; }

        public List<CustomRecord> _CustList { get; set; }

        /// <summary>
        /// 傳入CustomRecord 的 List,以建立訊息清單畫面
        /// </summary>
        /// <param name="CustList">訊息清單</param>
        /// <param name="CloseTimeOut">畫面關閉單位為每秒</param>
        public AlertCustom(List<CustomRecord> CustList)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            if (CustList.Count > 0)
            {
                timer.Enabled = true;
                timer.Tick += new EventHandler(OnTimer);
                timer.Interval = 1000; //動畫開啟秒數

                flowLayoutPanel1.Controls.Clear();

                _CustList = CustList;

                foreach (CustomRecord cr in _CustList)
                {
                    UserControl1 uc = new UserControl1(cr);
                    flowLayoutPanel1.Controls.Add(uc);
                }

                ReSize();

                this.AutoClose = true;
                this.TopMost = true;
                this.AlertAnimation = DevComponents.DotNetBar.eAlertAnimation.BottomToTop;
                this.AlertAnimationDuration = 400;
            }
        }

        /// <summary>
        /// 加入一個新訊息
        /// </summary>
        public void AddMessage(CustomRecord cr)
        {
            List<UserControl1> list = new List<UserControl1>();
            foreach (UserControl1 each in flowLayoutPanel1.Controls)
            {
                list.Add(each);
            }
            flowLayoutPanel1.Controls.Clear();

            _CustList.Add(cr);

            UserControl1 uc = new UserControl1(cr);
            flowLayoutPanel1.Controls.Add(uc);

            flowLayoutPanel1.Controls.AddRange(list.ToArray());

            ReSize();
        }

        /// <summary>
        /// 顯示畫面,並且開始計數
        /// </summary>
        /// <param name="CloseTimeOut"></param>
        public void ShowMessage(int CloseTimeOut)
        {
            timer_End = this.AutoCloseTimeOut = CloseTimeOut; //畫面關閉時間
            this.timer.Start();
            IsShow = true;
            this.Show(false);
        }

        private void AlertCustom_FormClosed(object sender, FormClosedEventArgs e)
        {
            IsShow = false;
            this.timer.Stop();
        }

        void OnTimer(object sender, EventArgs e)
        {
            if (timer_End > 0)
            {
                labelX1.Text = string.Format("訊息關閉時間:{0}", timer_End);
                timer_End = timer_End - 1;
            }
        }

        /// <summary>
        /// 重置畫面大小
        /// </summary>
        void ReSize()
        {
            if (flowLayoutPanel1.Controls.Count == 1)
            {
                this.ClientSize = new System.Drawing.Size(374, 60 + 126 * 1);
                this.flowLayoutPanel1.Size = new System.Drawing.Size(355, 126 * 1);
            }
            else if (flowLayoutPanel1.Controls.Count < 3)
            {
                this.ClientSize = new System.Drawing.Size(374, 60 + 126 * flowLayoutPanel1.Controls.Count);
                this.flowLayoutPanel1.Size = new System.Drawing.Size(355, 126 * flowLayoutPanel1.Controls.Count);
            }
            else
            {
                this.ClientSize = new System.Drawing.Size(374, 60 + 126 * 3);
                this.flowLayoutPanel1.Size = new System.Drawing.Size(355, 126 * 3);
            }

            Rectangle r = Screen.GetWorkingArea(this);
            this.Location = new Point(r.Right - this.Width - 25, r.Bottom - this.Height - 25);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // labelX1
            // 
            this.labelX1.AutoSize = true;
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelX1.Location = new System.Drawing.Point(20, 12);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(111, 26);
            this.labelX1.TabIndex = 2;
            this.labelX1.Text = "訊息關閉時間:";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(10, 42);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(374, 126);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // AlertCustom
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 15);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(221)))), ((int)(((byte)(244)))));
            this.BackColor2 = System.Drawing.Color.White;
            this.CaptionFont = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.ClientSize = new System.Drawing.Size(394, 180);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(114)))), ((int)(((byte)(196)))));
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "AlertCustom";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Style = DevComponents.DotNetBar.eBallonStyle.Alert;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AlertCustom_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
