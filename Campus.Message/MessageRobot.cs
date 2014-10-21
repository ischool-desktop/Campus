using FISCA.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Campus.Message
{
    /// <summary>
    /// 系統載入後,統一顯示的訊息內容
    /// </summary>
    static public class MessageRobot
    {
        static AlertCustom ac { get; set; }

        /// <summary>
        /// 於系統載入時使用
        /// 加入訊息顯示
        /// </summary>
        static public void AddMessage(CustomRecord cr)
        {
            if (cr.Title != null && cr.Content != null)
            {
                if (IsShow)
                {
                    ac.AddMessage(cr);
                }
                else
                {
                    ShowMessage(cr);
                }
            }
            else
            {
                MsgBox.Show("快速訊息 標題&內容 不可為空");
            }
        }

        /// <summary>
        /// 最新消息視窗是否已顯示
        /// </summary>
        static public bool IsShow
        {
            get
            {
                if (ac != null)
                    return ac.IsShow;
                else
                    return false;
            }
        }

        /// <summary>
        /// 顯示訊息內容
        /// </summary>
        static public void ShowMessage(CustomRecord cr)
        {
            List<CustomRecord> _CustList = new List<CustomRecord>() { cr };
            ac = new AlertCustom(_CustList);
            ac.ShowMessage(300);
        }
    }
}
