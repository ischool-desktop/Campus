using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Campus.Message
{
    /// <summary>
    /// 一個訊息畫面
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public EventHandler ReMove { get; set; }

        public Object 更多資訊 { get; set; }

        /// <summary>
        /// 顯示一個訊息
        /// </summary>
        public UserControl1(CustomRecord cr)
        {
            InitializeComponent();

            SetImage(cr);

            自定標題.Text = cr.Title;
            自定內文.Text = cr.Content;

            if (cr.OtherMore != null)
            {
                更多資訊 = cr.OtherMore;
            }
            else
            {
                //如果沒附其它內容
                自定內文.LinkColor = Color.Black;
                自定內文.ActiveLinkColor = Color.Black;
                自定內文.LinkBehavior = LinkBehavior.NeverUnderline;
            }
        }

        /// <summary>
        /// 設定本訊息的畫面圖樣
        /// </summary>
        private void SetImage(CustomRecord cr)
        {
            if (cr.Type == CrType.Type.News)
            {
                自定圖示.Image = Properties.Resources.speech_balloon_64;
            }
            else if (cr.Type == CrType.Type.Star)
            {
                自定圖示.Image = Properties.Resources.star_64;
            }
            else if (cr.Type == CrType.Type.Error)
            {
                自定圖示.Image = Properties.Resources.delete_64;
            }
            else if (cr.Type == CrType.Type.Warning_Red)
            {
                自定圖示.Image = Properties.Resources.info_64_2;
            }
            else
            {
                自定圖示.Image = Properties.Resources.info_64;
            }
        }

        private void 自定內文_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (更多資訊 != null)
            {
                if (更多資訊 is Form)
                {
                    (更多資訊 as Form).ShowDialog();
                }
                else if (更多資訊 is string)
                {
                    ProcessStartInfo sInfo = new ProcessStartInfo(更多資訊.ToString());
                    Process.Start(sInfo);
                }
            }
        }
    }
}
