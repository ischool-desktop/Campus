using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iValidator
{
    /// <summary>
    /// 本物件可儲存驗證相關訊息
    /// </summary>
    public class ValidatorInfoObj
    {
        /// <summary>
        /// 指定訊息
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 檔案名稱
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 工作表名稱
        /// </summary>
        public string SheetName { get; set; }

        /// <summary>
        /// 錯誤計數
        /// </summary>
        public int ErrorCount { get; set; }

        /// <summary>
        /// 警告計數
        /// </summary>
        public int WarningCount { get; set; }

        /// <summary>
        /// 自動修正計數
        /// </summary>
        public int AutoCorrectCount { get; set; }

    }
}
