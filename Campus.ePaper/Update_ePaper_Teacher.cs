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
    public partial class Update_ePaper_Teacher : BaseForm
    {

        string _DocName { get; set; }

        List<TeacherDocRecord> SectionList { get; set; }

        PrefixTeacher _pf { get; set; }

        int UpdateCount = 0;

        StringBuilder sb { get; set; }
        /// <summary>
        /// 開始針對載入之檔案,進行頁面分析
        /// </summary>
        BackgroundWorker BGW { get; set; }

        BackgroundWorker BGWUpdate { get; set; }

        /// <summary>
        /// 開始進行學生電子報表分析與上傳
        /// </summary>
        /// <param name="SchoolYear">學年度</param>
        /// <param name="Semester">學期</param>
        /// <param name="doc">電子報表</param>
        /// <param name="Name">報表名稱</param>
        /// <param name="pf">前綴詞:學號/身分證號/系統編號</param>
        public Update_ePaper_Teacher(List<Document> DocList, string Name, PrefixTeacher pf)
        {
            InitializeComponent();

            _pf = pf;

            this.Text = "開始報表分析... - " + Name;
            _DocName = Name;
            colStudentNumber.HeaderText = pf.ToString();

            intSchoolYear.Value = tool.PullIntTry(K12.Data.School.DefaultSchoolYear);
            intSemester.Value = tool.PullIntTry(K12.Data.School.DefaultSemester);

            BGW = new BackgroundWorker();
            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);
            BGW.ProgressChanged += new ProgressChangedEventHandler(BGW_ProgressChanged);
            BGW.WorkerReportsProgress = true;

            BGWUpdate = new BackgroundWorker();
            BGWUpdate.DoWork += new DoWorkEventHandler(BGWUpdate_DoWork);
            BGWUpdate.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGWUpdate_RunWorkerCompleted);
            BGWUpdate.ProgressChanged += new ProgressChangedEventHandler(BGWUpdate_ProgressChanged);
            BGWUpdate.WorkerSupportsCancellation = true;
            BGWUpdate.WorkerReportsProgress = true;

            BGW.RunWorkerAsync(DocList);
        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            //確認本背景模式,以哪個An
            List<Document> _DocList = (List<Document>)e.Argument;

            //取得切割後的檔案 & 基本資料內容
            SectionList = new List<TeacherDocRecord>();

            int successCount = 0; //用於計算進度。

            foreach (Document each in _DocList)
            {
                successCount++;

                SectionList.AddRange(ePaper.EatDocument(each, _pf));

                decimal seed = (decimal)successCount / _DocList.Count();
                BGW.ReportProgress((int)(seed * 100));
            }

            //檢查學號是否存在系統內
            SectionList = ePaper.CheckDocument(SectionList, _pf);
        }

        void BGW_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage("處理中...", e.ProgressPercentage);
        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (e.Error == null)
                {
                    this.Text = "分析完成,請確認上傳「" + _DocName + "」";
                    labelX3.Text = "共「" + SectionList.Count + "」筆電子報表(本功能每300張分為一個批次上傳)";
                    //畫面更新
                    BindData();
                }
                else
                {
                    MsgBox.Show("分析發生錯誤!!\n" + e.Error.Message);
                    this.DialogResult = System.Windows.Forms.DialogResult.No;
                }
            }
            else
            {
                MsgBox.Show("作業已中止!");
                this.DialogResult = System.Windows.Forms.DialogResult.No;
            }
        }

        private void BindData()
        {
            dataGridViewX1.AutoGenerateColumns = false;
            dataGridViewX1.DataSource = SectionList;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (SectionList.Count != 0)
            {
                SetingForm = false;

                this.Text = "開始上傳「" + _DocName + "」";
                BGWUpdate.RunWorkerAsync(cbWordChangePDF.Checked);
            }
            else
            {
                this.DialogResult = System.Windows.Forms.DialogResult.No;
            }
        }

        bool SetingForm
        {
            set
            {
                btnUpdate.Enabled = value;
                cbWordChangePDF.Enabled = value;
                linkSaveFile.Enabled = value;
            }
        }

        void BGWUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            bool _cbWordChangePDF = (bool)e.Argument;

            int successCount = 0; //用於計算進度。
            UpdateCount = 0;

            #region 分割報表數量

            Dictionary<int, List<TeacherDocRecord>> path = new Dictionary<int, List<TeacherDocRecord>>();

            int Cset = 1;
            int count = 1;
            foreach (TeacherDocRecord each in SectionList)
            {
                if (count <= 300)
                {
                    count++;
                }
                else
                {
                    Cset++;
                    count = 2;
                }

                if (!path.ContainsKey(Cset))
                    path.Add(Cset, new List<TeacherDocRecord>());

                path[Cset].Add(each);
            }

            #endregion

            foreach (int Dset in path.Keys)
            {
                #region 批次上傳

                if (BGWUpdate.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                // 學生電子報表
                //SmartSchool.ePaper.ElectronicPaper paperForStudent = new SmartSchool.ePaper.ElectronicPaper(_DocName, intSchoolYear.Value.ToString(), intSemester.Value.ToString(), SmartSchool.ePaper.ViewerType.Student);

                // 教師電子報表
                SmartSchool.ePaper.ElectronicPaper paperForTeacher = new SmartSchool.ePaper.ElectronicPaper(_DocName, intSchoolYear.Value.ToString(), intSemester.Value.ToString(), SmartSchool.ePaper.ViewerType.Teacher);
                sb = GetLogString(_cbWordChangePDF);

                foreach (TeacherDocRecord tr in path[Dset])
                {
                    successCount++;

                    if (tr.Status == "(正常)")
                    {
                        if (tr.Doc != null)
                        {
                            //傳參數給PaperItem
                            //格式 / 內容 / 對象的系統編號
                            sb.AppendLine(string.Format("班級「{1}」姓名「{0}」暱稱 「{2}」", tr.TeacherName,tr.TeacherClass, tr.TeacherNickName));
                            MemoryStream stream = new MemoryStream();

                            Document idoc = new Document();
                            idoc.Sections.Clear();
                            Node n = idoc.ImportNode(tr.Doc, true);
                            idoc.Sections.Add(n);

                            if (_cbWordChangePDF)
                            {
                                idoc.Save(stream, SaveFormat.Pdf);
                                paperForTeacher.Append(new PaperItem(PaperFormat.AdobePdf, stream, tr.Teacher.TeacherID));
                            }
                            else
                            {
                                idoc.Save(stream, SaveFormat.Doc);
                                paperForTeacher.Append(new PaperItem(PaperFormat.Office2003Doc, stream, tr.Teacher.TeacherID));
                            }
                            UpdateCount++;

                        }
                    }

                    decimal seed = (decimal)successCount / SectionList.Count();
                    BGWUpdate.ReportProgress((int)(seed * 100));

                }

                if (UpdateCount > 0)
                {
                    BGWUpdate.ReportProgress(0, "共" + SectionList.Count + "張報表,第" + Dset + "/" + path.Count + "批次");

                    SmartSchool.ePaper.DispatcherProvider.Dispatch(paperForTeacher);

                    FISCA.LogAgent.ApplicationLog.Log("電子報表", "上傳", sb.ToString());
                }

                #endregion
            }
        }

        void BGWUpdate_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
            {
                FISCA.Presentation.MotherForm.SetStatusBarMessage(e.UserState.ToString(), e.ProgressPercentage);
            }
            else
            {
                FISCA.Presentation.MotherForm.SetStatusBarMessage("處理中...", e.ProgressPercentage);
            }
        }

        private StringBuilder GetLogString(bool _cbWordChangePDF)
        {
            StringBuilder log = new StringBuilder();
            log.AppendLine("上傳教師電子報表：");
            log.AppendLine("報表名稱「" + _DocName + "」");
            log.AppendLine("學年度「" + intSchoolYear.Value.ToString() + "」");
            log.AppendLine("學期「" + intSemester.Value.ToString() + "」");
            if (_cbWordChangePDF)
                log.AppendLine("格式「PDF」");
            else
                log.AppendLine("格式「Word」");
            log.AppendLine("");
            log.AppendLine("教師清單：");
            return log;
        }

        void BGWUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetingForm = true;

            if (!e.Cancelled)
            {
                if (e.Error == null)
                {
                    if (UpdateCount > 0)
                    {
                        MsgBox.Show("上傳「" + UpdateCount + "」筆電子報表!!");
                        FISCA.Presentation.MotherForm.SetStatusBarMessage("");
                        this.DialogResult = System.Windows.Forms.DialogResult.Yes;
                    }
                    else
                    {
                        this.DialogResult = System.Windows.Forms.DialogResult.No;
                    }
                }
                else
                {
                    MsgBox.Show("發生錯誤!!\n" + e.Error.Message);
                    this.DialogResult = System.Windows.Forms.DialogResult.No;
                }
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
                foreach (TeacherDocRecord record in SectionList)
                {
                    if (record.Teacher != null)
                    {
                        if (cbWordChangePDF.Checked)
                        {
                            Document idoc = new Document();
                            idoc.Sections.Clear();
                            Node n = idoc.ImportNode(record.Doc, true);
                            idoc.Sections.Add(n);

                            idoc.Save(fbd.SelectedPath + "\\" + record.TeacherName + ".pdf", SaveFormat.Pdf);
                        }
                        else
                        {
                            Document idoc = new Document();
                            idoc.Sections.Clear();
                            Node n = idoc.ImportNode(record.Doc, true);
                            idoc.Sections.Add(n);

                            idoc.Save(fbd.SelectedPath + "\\" + record.TeacherName + ".doc", SaveFormat.Doc);
                        }
                    }
                }
                MessageBox.Show("已儲存!!");
            }
        }

        private void Update_ePaper_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (BGWUpdate.IsBusy)
            {
                DialogResult dr = MsgBox.Show("您確定要關閉上傳作業?\n已批次上傳之電子報表\n將需手動刪除", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);
                if (dr == System.Windows.Forms.DialogResult.Yes)
                {
                    FISCA.Presentation.MotherForm.SetStatusBarMessage("");
                    BGWUpdate.CancelAsync();
                }
                else
                {
                    e.Cancel = true;
                    MsgBox.Show("作業將繼續處理...");
                }
            }
        }
    }
}

