using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Words;

namespace Campus.ePaper
{
    public class TeacherDocRecord
    {
        /// <summary>
        /// 教師基本資料
        /// </summary>
        public TeacherR Teacher { get; set; }

        /// <summary>
        /// 文件
        /// </summary>
        public Section Doc { get; set; }

        /// <summary>
        /// 教師姓名
        /// </summary>
        public string TeacherName
        {
            get
            {
                if (Teacher != null)
                    return Teacher.Teacher_Name;
                else
                    return "";
            }
        }

        /// <summary>
        /// 教師暱稱
        /// </summary>
        public string TeacherNickName
        {
            get
            {
                if (Teacher != null)
                    return Teacher.Teacher_NickName;
                else
                    return "";
            }
        }

        /// <summary>
        /// 教師班級
        /// </summary>
        public string TeacherClass
        {
            get
            {
                if (Teacher != null)
                    return Teacher.Teacher_Class;
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
