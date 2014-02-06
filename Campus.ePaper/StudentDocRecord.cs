using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Words;

namespace Campus.ePaper
{
    public class StudentDocRecord
    {
        /// <summary>
        /// 學生基本資料
        /// </summary>
        public StudR Student { get; set; }

        /// <summary>
        /// 文件
        /// </summary>
        public Document Doc { get; set; }

        /// <summary>
        /// 學生姓名
        /// </summary>
        public string StudentName
        {
            get
            {
                if (Student != null)
                    return Student.Student_Name;
                else
                    return "";
            }
        }

        /// <summary>
        /// 學生班級
        /// </summary>
        public string StudentClass
        {
            get
            {
                if (Student != null)
                    return Student.Student_Class;
                else
                    return "";
            }
        }

        /// <summary>
        /// 學生座號
        /// </summary>
        public string StudentSeatNo
        {
            get
            {
                if (Student != null)
                    return Student.Student_SeatNo;
                else
                    return "";
            }
        }

        /// <summary>
        /// 比對用的前綴詞
        /// </summary>
        public string PrefixName { get; set; }

        /// <summary>
        /// 目前狀態
        /// </summary>
        public string Status { get; set; }
    }
}
