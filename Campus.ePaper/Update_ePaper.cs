using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using Aspose.Words;
using System.IO;
using SmartSchool.ePaper;

namespace Campus.ePaper
{
    public partial class Update_ePaper : BaseForm
    {

        /// <summary>
        /// 學生電子報表
        /// </summary>
        SmartSchool.ePaper.ElectronicPaper paperForStudent { get; set; }

        Document _doc { get; set; }

        string _DocName { get; set; }

        List<StudentDocRecord> SectionList { get; set; }

        /// <summary>
        /// 開始針對載入之檔案,進行頁面分析
        /// </summary>
        BackgroundWorker BGW { get; set; }

        /// <summary>
        /// 開始進行學生電子報表分析與上傳
        /// </summary>
        /// <param name="SchoolYear">學年度</param>
        /// <param name="Semester">學期</param>
        /// <param name="doc">電子報表</param>
        /// <param name="Name">報表名稱</param>
        /// <param name="pf">前綴詞:學號/身分證號/系統編號</param>
        public Update_ePaper(Document doc, string Name, PrefixStudent pf)
        {
            InitializeComponent();

            _doc = doc;
            this.Text = "開始報表分析... - " + Name;
            _DocName = Name;
            colStudentNumber.HeaderText = pf.ToString();

            intSchoolYear.Value = PullIntTry(K12.Data.School.DefaultSchoolYear);
            intSemester.Value = PullIntTry(K12.Data.School.DefaultSemester);

            BGW = new BackgroundWorker();
            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);

            BGW.RunWorkerAsync(pf);
        }

        private int PullIntTry(string a)
        {
            int k = 1;
            int.TryParse(a, out k);
            return k;
        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            //確認本背景模式,以哪個An
            PrefixStudent _pf = (PrefixStudent)e.Argument;

            //取得切割後的檔案 & 基本資料內容
            SectionList = ePaper.EatDocument(_doc, _pf);

            //檢查學號是否存在系統內
            SectionList = ePaper.CheckDocument(SectionList, _pf);
        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Text = "完成分析請確認上傳 [" + _DocName + "]";
            if (!e.Cancelled)
            {
                if (e.Error == null)
                {
                    //畫面更新
                    BindData();
                }
                else
                {
                    MsgBox.Show("資料分析發生錯誤!!\n" + e.Error.Message);
                }
            }
            else
            {
                MsgBox.Show("資料分析已取消!!");
            }
        }

        private void BindData()
        {
            dataGridViewX1.AutoGenerateColumns = false;
            dataGridViewX1.DataSource = SectionList;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("上傳學生電子報表：");
            sb.AppendLine("報表名稱「" + _DocName + "」");
            sb.AppendLine("學年度「" + intSchoolYear.Value.ToString() + "」");
            sb.AppendLine("學期「" + intSemester.Value.ToString() + "」");
            sb.AppendLine("");
            sb.AppendLine("學生清單：");
            paperForStudent = new SmartSchool.ePaper.ElectronicPaper(_DocName, intSchoolYear.Value.ToString(), intSemester.Value.ToString(), SmartSchool.ePaper.ViewerType.Student);
            int UpdateCount = 0;
            foreach (StudentDocRecord sr in SectionList)
            {
                if (sr.Status == "(正常)")
                {
                    if (sr.Doc != null)
                    {
                        //傳參數給PaperItem
                        //格式 / 內容 / 對象的系統編號
                        sb.AppendLine(string.Format("班級「{1}」座號「{2}」姓名「{0}」學號「{3}」", sr.StudentName, sr.StudentClass, sr.StudentSeatNo, sr.Student.Student_Number));
                        MemoryStream stream = new MemoryStream();
                        sr.Doc.Save(stream, SaveFormat.Doc);
                        paperForStudent.Append(new PaperItem(PaperFormat.Office2003Doc, stream, sr.Student.StudentID));
                        UpdateCount++;
                    }
                }
            }

            MsgBox.Show("上傳「" + UpdateCount + "」筆電子報表!!");

            if (UpdateCount > 0)
            {
                SmartSchool.ePaper.DispatcherProvider.Dispatch(paperForStudent);
                FISCA.LogAgent.ApplicationLog.Log("電子報表", "上傳", sb.ToString());
                this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            }
            else
            {
                this.DialogResult = System.Windows.Forms.DialogResult.No;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
        }

        private void linkSaveFile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (StudentDocRecord record in SectionList)
                {
                    if (record.Student != null)
                    {
                        record.Doc.Save(fbd.SelectedPath + "\\" + record.Student.Student_Number + "_" + record.StudentName + ".doc");
                    }
                }
                MessageBox.Show("已儲存!!");
            }
        }
    }
}
