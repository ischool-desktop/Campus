using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Campus.Message
{
    /// <summary>
    /// 訊息物件
    /// </summary>
    public class CustomRecord
    {
        /// <summary>
        /// 此訊息是 最新消息 還是 預警內容
        /// </summary>
        public CrType.Type Type { get; set; }

        /// <summary>
        /// 訊息標題
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 訊息內容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 訊息所帶有的更多內容
        /// </summary>
        public Object OtherMore { get; set; }
    }
}
