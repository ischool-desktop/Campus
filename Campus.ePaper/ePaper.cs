using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Words;
using System.IO;
using FISCA.Presentation.Controls;
using System.Data;
using System.Text.RegularExpressions;

namespace Campus.ePaper
{
    public enum Tag { Student, Class, Teacher, Course };

    public enum PrefixStudent { 學號, 身分證號, 系統編號 }

    public enum PrefixClass { 班級名稱, 系統編號 }

    public enum PrefixTeacher { 身分證號, 系統編號,教師姓名_暱稱 }

    public class ePaper
    {
        static StringBuilder sb = new StringBuilder();

        static FISCA.Data.QueryHelper _Query = new FISCA.Data.QueryHelper();

        /// <summary>
        /// 傳入欲上傳的檔案
        /// </summary>
        /// <param name="doc">檔案</param>
        /// <param name="Prefix">標記之前綴詞 學號/身分證號/系統編號</param>
        /// <returns>回傳依據前綴詞所分割完成之物件</returns>
        public static List<StudentDocRecord> EatDocument(Document doc, PrefixStudent Prefix)
        {
            List<StudentDocRecord> SectionList = new List<StudentDocRecord>();
            int count = 1;
            foreach (Section each in doc.Sections)
            {
                string text = MatchText(each.GetText(), Prefix);
                if (!string.IsNullOrEmpty(text))
                {
                    SectionList.Add(SetSections(each, text, "(正常)"));
                }
                else
                {
                    SectionList.Add(SetSections(each, "", "(無法比對)"));
                    count++;
                }
            }

            return SectionList;
        }


        /// <summary>
        /// 傳入欲上傳的檔案
        /// </summary>
        /// <param name="doc">檔案</param>
        /// <param name="Prefix">標記之前綴詞 學號/身分證號/系統編號</param>
        /// <returns>回傳依據前綴詞所分割完成之物件</returns>
        public static List<TeacherDocRecord> EatDocument(Document doc, PrefixTeacher Prefix)
        {
            List<TeacherDocRecord> SectionList = new List<TeacherDocRecord>();
            int count = 1;
            foreach (Section each in doc.Sections)
            {
                string text = MatchText(each.GetText(), Prefix);
                if (!string.IsNullOrEmpty(text))
                {
                    SectionList.Add(SetSections(each, text, "(正常)", Prefix));
                }
                else
                {
                    SectionList.Add(SetSections(each, "", "(無法比對)", Prefix));
                    count++;
                }
            }

            return SectionList;
        }

        /// <summary>
        /// 傳入切割後頁面,回傳學生與文件
        /// </summary>
        /// <param name="OnePage">一張紙</param>
        /// <param name="PageName">報表名稱</param>
        /// <returns></returns>
        private static StudentDocRecord SetSections(Section OnePage, string PrefixName, string NowStatus)
        {
            StudentDocRecord record = new StudentDocRecord();
            record.PrefixName = PrefixName;
            record.Doc = OnePage;
            record.Status = NowStatus;
            return record;
        }

        /// <summary>
        /// 傳入切割後頁面,回傳教師與文件
        /// </summary>
        /// <param name="OnePage">一張紙</param>
        /// <param name="PageName">報表名稱</param>
        /// <returns></returns>
        private static TeacherDocRecord SetSections(Section OnePage, string PrefixName, string NowStatus, PrefixTeacher Prefix)
        {
            TeacherDocRecord record = new TeacherDocRecord();
            record.PrefixName = PrefixName;
            record.Doc = OnePage;
            record.Status = NowStatus;
            return record;
        }

        /// <summary>
        /// 是否有符合
        /// Prefix{xxxxx}之字串
        /// </summary>
        /// <param name="Text">待檢查字串</param>
        private static string MatchText(string Text, PrefixStudent Prefix)
        {
            //可輸入0~9 a~z A~Z
            //Regex rx = new Regex(@"學號\{([0-9a-zA-Z]+)\}");
            Regex rx;
            //可輸入0~9
            if (Prefix == PrefixStudent.系統編號)
            {
                rx = new Regex(@"系統編號\{([0-9a-zA-Z]+)\}");
            }
            else if (Prefix == PrefixStudent.身分證號)
            {
                rx = new Regex(@"身分證號\{([0-9a-zA-Z]+)\}");
            }
            else
            {
                rx = new Regex(@"學號\{([0-9a-zA-Z]+)\}");
            }

            Match ch = rx.Match(Text);
            if (ch.Success)
            {
                Group g = ch.Groups[1];
                return g.Value;
            }
            else
            {
                return "";
            }
        }


        /// <summary>
        /// 是否有符合
        /// Prefix{xxxxx}之字串
        /// </summary>
        /// <param name="Text">待檢查字串</param>
        private static string MatchText(string Text, PrefixTeacher Prefix)
        {
            //可輸入0~9 a~z A~Z
            //Regex rx = new Regex(@"學號\{([0-9a-zA-Z]+)\}");
            Regex rx;
            //可輸入0~9
            if (Prefix == PrefixTeacher.系統編號)
            {
                rx = new Regex(@"系統編號\{([0-9a-zA-Z]+)\}");
            }
            else if (Prefix == PrefixTeacher.身分證號)
            {
                rx = new Regex(@"身分證號\{([0-9a-zA-Z]+)\}");
            }
            else
            {
                // 可濾出 所有非含有 ^ 的字串
                rx = new Regex(@"教師姓名_暱稱\{([^\^]+)\}");
            }

            Match ch = rx.Match(Text);
            if (ch.Success)
            {
                Group g = ch.Groups[1];
                return g.Value;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 檢查與取得相對應的編號
        /// </summary>
        /// <param name="SectionList">Section每一頁是一個上傳單位</param>
        /// <param name="Prefix">比對前綴詞</param>
        public static List<StudentDocRecord> CheckDocument(List<StudentDocRecord> SectionList, PrefixStudent Prefix)
        {
            //取得學號清單
            List<string> NumberList = new List<string>();
            foreach (StudentDocRecord record in SectionList)
            {
                if (!string.IsNullOrEmpty(record.PrefixName))
                {
                    if (!NumberList.Contains(record.PrefixName))
                    {
                        NumberList.Add(record.PrefixName);
                    }
                }
            }

            if (NumberList.Count != 0)
            {
                #region 取得資料 & 填入資料
                StringBuilder Sq = new StringBuilder();
                Dictionary<string, StudR> CheckDic = new Dictionary<string, StudR>();

                if (Prefix == PrefixStudent.學號)
                {
                    Sq.Append(string.Format("select student.id,student.student_number,student.id_number,student.name,class.class_name,student.seat_no from student left join class on ref_class_id=class.id where student.student_number in ('{0}') and student.status in ('1','2','4','8')", string.Join("','", NumberList)));

                    DataTable dt = _Query.Select(Sq.ToString());
                    foreach (DataRow row in dt.Rows)
                    {
                        StudR r = GetStudR(row);
                        if (!CheckDic.ContainsKey(r.Student_Number))
                        {
                            CheckDic.Add(r.Student_Number, r);
                        }
                    }
                }
                else if (Prefix == PrefixStudent.身分證號)
                {
                    Sq.Append(string.Format("select student.id,student.student_number,student.id_number,student.name,class.class_name,student.seat_no from student left join class on ref_class_id=class.id where student.id_number in ('{0}') and student.status in ('1','2','4','8')", string.Join("','", NumberList)));

                    DataTable dt = _Query.Select(Sq.ToString());
                    foreach (DataRow row in dt.Rows)
                    {
                        StudR r = GetStudR(row);
                        if (!CheckDic.ContainsKey(r.Id_Number))
                        {
                            CheckDic.Add(r.Id_Number, r);
                        }
                    }
                }
                else
                {
                    Sq.Append(string.Format("select student.id,student.student_number,student.id_number,student.name,class.class_name,student.seat_no from student left join class on ref_class_id=class.id where student.id in ('{0}') and student.status in ('1','2','4','8')", string.Join("','", NumberList)));

                    DataTable dt = _Query.Select(Sq.ToString());
                    foreach (DataRow row in dt.Rows)
                    {
                        StudR r = GetStudR(row);
                        if (!CheckDic.ContainsKey(r.StudentID))
                        {
                            CheckDic.Add(r.StudentID, r);
                        }
                    }
                }

                foreach (StudentDocRecord each in SectionList)
                {
                    if (CheckDic.ContainsKey(each.PrefixName))
                    {
                        each.Student = CheckDic[each.PrefixName];
                    }
                    else
                    {
                        each.Status = "(查無學生)";
                    }
                }
                #endregion
            }

            return SectionList;
        }

        private static StudR GetStudR(DataRow row)
        {
            StudR r = new StudR();
            r.StudentID = "" + row["id"]; ;
            r.Student_Number = "" + row["student_number"];
            r.Id_Number = "" + row["id_number"];
            r.Student_Name = "" + row["name"];
            r.Student_Class = "" + row["class_name"];
            r.Student_SeatNo = "" + row["seat_no"];

            return r;
        }



        private static TeacherR GetTeacherR(DataRow row)
        {
            TeacherR r = new TeacherR();
            r.TeacherID = "" + row["id"]; ;            
            r.Id_Number = "" + row["id_number"];
            r.Teacher_Name = "" + row["teacher_name"];
            r.Teacher_NickName = "" + row["nickname"];
            r.Teacher_Class = "" + row["class_name"];
            

            return r;
        }

        /// <summary>
        /// 依據檔案內容之標記
        /// 傳送至
        /// </summary>
        /// <param name="doc">所要分割的檔案</param>
        /// <returns>回傳上傳內容與狀態</returns>
        public static string Update(List<StudentDocRecord> SectionList)
        {

            //開始上傳
            UpdateDocument(SectionList, Tag.Student);




            //回傳檔案上傳訊息
            return sb.ToString();
        }

        /// <summary>
        /// 開始依據系統編號進行資料上傳
        /// </summary>
        private static void UpdateDocument(List<StudentDocRecord> SectionDic, Tag _tag)
        {
            if (_tag == Tag.Student)
            {




            }
        }


        /// <summary>
        /// 檢查與取得相對應的編號
        /// </summary>
        /// <param name="SectionList">Section每一頁是一個上傳單位</param>
        /// <param name="Prefix">比對前綴詞</param>
        public static List<TeacherDocRecord> CheckDocument(List<TeacherDocRecord> SectionList, PrefixTeacher Prefix)
        {
            //取得學號清單
            List<string> NumberList = new List<string>();
            foreach (TeacherDocRecord record in SectionList)
            {
                if (!string.IsNullOrEmpty(record.PrefixName))
                {
                    if (!NumberList.Contains(record.PrefixName))
                    {
                        NumberList.Add(record.PrefixName);
                    }
                }
            }

            if (NumberList.Count != 0)
            {
                #region 取得資料 & 填入資料
                StringBuilder Sq = new StringBuilder();
                Dictionary<string, TeacherR> CheckDic = new Dictionary<string, TeacherR>();

                if (Prefix == PrefixTeacher.教師姓名_暱稱)
                {
                    //Sq.Append(string.Format("select student.id,student.student_number,student.id_number,student.name,class.class_name,student.seat_no from student left join class on ref_class_id=class.id where student.student_number in ('{0}') and student.status in ('1','2','4','8')", string.Join("','", NumberList)));
                    Sq.Append(string.Format("select teacher.id,teacher.id_number,teacher.teacher_name,teacher.nickname,class.class_name from class left outer join teacher on ref_teacher_id=teacher.id where teacher.id_number in ('{0}') and teacher.status in ('1','2','4','8')", string.Join("','", NumberList)));

                    DataTable dt = _Query.Select(Sq.ToString());
                    foreach (DataRow row in dt.Rows)
                    {
                        TeacherR r = GetTeacherR(row);
                        //if (!CheckDic.ContainsKey(r.Student_Number))
                        //{
                        //    CheckDic.Add(r.Student_Number, r);
                        //}
                    }
                }
                else if (Prefix == PrefixTeacher.身分證號)
                {
                    //Sq.Append(string.Format("select student.id,student.student_number,student.id_number,student.name,class.class_name,student.seat_no from student left join class on ref_class_id=class.id where student.id_number in ('{0}') and student.status in ('1','2','4','8')", string.Join("','", NumberList)));
                    
                    Sq.Append(string.Format("select teacher.id,teacher.id_number,teacher.teacher_name,teacher.nickname,class.class_name from class left outer join teacher on ref_teacher_id=teacher.id where teacher.id_number in ('{0}') and teacher.status in ('1','2','4','8')", string.Join("','", NumberList)));

                    DataTable dt = _Query.Select(Sq.ToString());
                    foreach (DataRow row in dt.Rows)
                    {
                        TeacherR r = GetTeacherR(row);
                        if (!CheckDic.ContainsKey(r.Id_Number))
                        {
                            CheckDic.Add(r.Id_Number, r);
                        }
                    }
                }
                else
                {                                    
                    Sq.Append(string.Format("select teacher.id,teacher.id_number,teacher.teacher_name,teacher.nickname,class.class_name from class left outer join teacher on ref_teacher_id=teacher.id where teacher.id in ('{0}') and teacher.status in ('1','2','4','8')", string.Join("','", NumberList)));

                    DataTable dt = _Query.Select(Sq.ToString());
                    foreach (DataRow row in dt.Rows)
                    {
                        TeacherR r = GetTeacherR(row);
                        if (!CheckDic.ContainsKey(r.TeacherID))
                        {
                            CheckDic.Add(r.TeacherID, r);
                        }
                    }
                }

                foreach (TeacherDocRecord each in SectionList)
                {
                    if (CheckDic.ContainsKey(each.PrefixName))
                    {
                        each.Teacher = CheckDic[each.PrefixName];
                    }
                    else
                    {
                        each.Status = "(查無教師)";
                    }
                }
                #endregion
            }

            return SectionList;
        }


    }
}
