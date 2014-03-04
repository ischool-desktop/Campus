using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using Aspose.Cells;
using Campus.Validator;
using FISCA;
using FISCA.Presentation.Controls;

namespace Campus.Import
{
    /// <summary>
    /// 選擇來源檔案及資料表：
    /// 重要說明：
    /// 1.
    /// 待處理：
    /// 1.選擇資料表時顯示欄位。
    /// 2.選擇下一步時錯誤訊息呈現方式目前是用MessageBox，呈現方式可以更精緻。
    /// 3.撰寫驗證規則呈現UI。
    /// </summary>
    public partial class SelectSource : WizardForm
    {
        private ImportFullOption mImportOption;
        private ImportWizard mImportWizard;
        private SheetHelper mSheetHelper;
        private Dictionary<string, List<string>> mSelectablKeyFields;
        private ArgDictionary mArgs;
        private string mImportName = string.Empty;


        public SelectSource()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 建構式，傳入精靈選項
        /// </summary>
        /// <param name="args"></param>
        public SelectSource(ArgDictionary args) : base(args)
        {
            InitializeComponent();            

            //初始化參數
            mArgs = args;
            mImportWizard = args["ImportWizard"] as ImportWizard;
            mImportOption = TryGetOption();
            mImportName = mImportWizard.ValidateRule.Root.GetAttributeText("Name");
            mImportWizard.ImportOption = mImportOption;

            this.Text = mImportName + "-選擇檔案與匯入方式" + "(" + CurrentStep + "/" + TotalStep + ")";

            //載入驗證規則及XSLT
            LoadValudateRule();

            //在使用者選擇資料表時，將資料表的欄位都記錄下來
            lstSheetNames.SelectedIndexChanged += (sender, e) =>
            {
                mSheetHelper.SwitchSeet("" + lstSheetNames.SelectedItem);
                mImportOption.SelectedSheetName = "" + lstSheetNames.SelectedItem;
                mImportOption.SheetFields = mSheetHelper.Fields;
                this.NextButtonEnabled = ValidateNext();
            };

            //檢視驗證規則
            btnViewRule.Click += (sender, e) =>
            {
                XmlViewForm ViewForm = new XmlViewForm();

                ViewForm.PopXml(mImportName,mImportOption.SelectedValidateFile);

                ViewForm.ShowDialog();
            };

            //檢視填表說明
            btnViewRuleExcel.Click += (sender, e) =>
            {
                Workbook book = new Workbook();

                string BookAndSheetName = mImportName +"(空白表格)";

                if (!string.IsNullOrEmpty(BookAndSheetName))
                    book.Worksheets[0].Name = BookAndSheetName;

                int Position = 0;

                foreach (XElement Element in mImportWizard.ValidateRule.Root.Element("FieldList").Elements("Field"))
                {
                    StringBuilder strCommentBuilder = new StringBuilder();

                    string Name = Element.GetAttributeText("Name");
                    bool Required = Element.GetAttributeBool("Required",false);

                    book.Worksheets[0].Cells[0, Position].PutValue(Name);
                    book.Worksheets[0].Cells[0, Position].Style.HorizontalAlignment = TextAlignmentType.Center;
                    book.Worksheets[0].Cells[0, Position].Style.VerticalAlignment = TextAlignmentType.Center;

                    if (Required)
                    {
                        book.Worksheets[0].Cells[0, Position].Style.BackgroundColor  = System.Drawing.Color.Red;
                        strCommentBuilder.AppendLine("此為必要欄位。");
                    }

                    foreach(XElement SubElement in Element.Elements("Validate"))
                        strCommentBuilder.AppendLine(SubElement.GetAttributeText("Description"));

                    book.Worksheets[0].Comments.Add(0,(byte)Position);
                    book.Worksheets[0].Comments[0,Position].Note = strCommentBuilder.ToString() ;
                    book.Worksheets[0].Comments[0, Position].WidthInch = 3;

                    Position++;
                }

                book.Worksheets[0].AutoFitColumns();
                Campus.Report.ReportSaver.SaveWorkbook(book, Path.Combine(Constants.ValidationReportsFolder,BookAndSheetName));
            };

            //選擇來源資料檔案
            btnSelectFile.Click += (sender, e) =>
            {
                DialogResult dr = SelectSourceFileDialog.ShowDialog();

                if (dr == DialogResult.OK)
                {
                    try
                    {
                        //記錄來源檔案名稱
                        string FileName = SelectSourceFileDialog.FileName;

                        txtSourceFile.Text = SelectSourceFileDialog.FileName;
                        mImportOption.SelectedDataFile = FileName;
                        mSheetHelper = new SheetHelper(FileName);

                        //將資料表列表顯示在畫面上
                        lstSheetNames.Items.Clear();

                        foreach (Worksheet sheet in mSheetHelper.Book.Worksheets)
                            lstSheetNames.Items.Add(sheet.Name);

                        lstSheetNames.SelectedIndex = 0;
                    }
                    catch (Exception ve)
                    {
                        MsgBox.Show(ve.Message);
                    }
                }
            };

            //將前一步不出現，下一步先失效
            this.PreviousButtonVisible = false;
            this.NextButtonEnabled = false;
        }

        /// <summary>
        /// 嘗試取得之前的記錄
        /// </summary>
        /// <returns></returns>
        private ImportFullOption TryGetOption()
        {
            //待處理
            //ImportFullOption RestoreOption = mArgs["ImportOption"] as ImportFullOption;

            //if (RestoreOption != null)
            //{
            //    txtSourceFile.Text = RestoreOption.SelectedDataFile;

            //    return RestoreOption;    
            //}
             
            return new ImportFullOption();
        }

        /// <summary>
        /// 將介面啟用或失效
        /// </summary>
        /// <param name="IsEnable"></param>
        private void ToggleUI(bool IsEnable)
        {
            this.btnSelectFile.Enabled = IsEnable;
            this.btnViewRule.Enabled = IsEnable;
            this.btnViewRuleExcel.Enabled = IsEnable;
            //變更名稱
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(IsEnable ? mImportName + "-選擇檔案與匯入方式" : mImportName + "-選擇檔案與匯入方式 初始化準備中..");
            strBuilder.Append("(" + CurrentStep + "/" + TotalStep + ")");

            this.Text = strBuilder.ToString();
        }

        /// <summary>
        /// 載入驗證規則
        /// </summary>
        private void LoadValudateRule()
        {
            BackgroundWorker bkwLoadRule = new BackgroundWorker();

            bkwLoadRule.DoWork += (sender, e) =>
            {
                try
                {
                    XDocument Document = XDocument.Load(@"http://sites.google.com/a/ischool.com.tw/leader/modules/format.xsl");

                    Document.Save(Constants.ValidationFormatPath);

                    mImportWizard.ValidateRule.Save(Constants.ValidationRulePath);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            };

            bkwLoadRule.RunWorkerCompleted += (sender, e) =>
            {
                //載入完後將UI啟用
                ToggleUI(true);
                mImportOption.SelectedValidateFile = Constants.ValidationRulePath;
            };

            //載入先先將UI失效
            ToggleUI(false);

            bkwLoadRule.RunWorkerAsync();
        }

        /// <summary>
        /// 驗證是否可進行到下一步
        /// </summary>
        /// <returns></returns>
        private bool ValidateNext()
        {
            try
            {
                ctlerrors.Clear();

                if (string.IsNullOrEmpty(txtSourceFile.Text))
                {
                    ctlerrors.SetError(lstSheetNames, "您必須選擇匯入來源檔案。");
                    return false;
                }

                if (!File.Exists(txtSourceFile.Text))
                {
                    ctlerrors.SetError(lstSheetNames, "您指定的來源檔案並不存在。");
                    return false;
                }

                if (lstSheetNames.SelectedItem == null)
                {
                    ctlerrors.SetError(lstSheetNames, "必須指定資料表!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ctlerrors.SetError(lstSheetNames, ex.Message);
                return false;
            }

            #region 驗證鍵值欄位及必填欄位
            StringBuilder strFieldsErrorBuilder = new StringBuilder();

            bool IsContainKeyFields = mImportWizard.FieldProcessor.KeyFields.Count>0;

            mSelectablKeyFields = mImportWizard.FieldProcessor.GetSelectableKeyFields(mImportOption.SheetFields);

            if (IsContainKeyFields && (mSelectablKeyFields == null || mSelectablKeyFields.Count == 0))
            {
                strFieldsErrorBuilder.AppendLine("資料表缺少鍵值欄位，以下為鍵值組合：");

                foreach (string Key in mImportWizard.FieldProcessor.KeyFields.Keys)
                    strFieldsErrorBuilder.AppendLine(Key+"："+string.Join(",",mImportWizard.FieldProcessor.KeyFields[Key].ToArray()));
            }

            bool IsContainRequiredFields = mImportWizard.FieldProcessor.IsContainRequiredFields(mImportOption.SheetFields);

            if (!IsContainRequiredFields)
            {
                List<string> RequiredFields = new List<string>();

                strFieldsErrorBuilder.AppendLine("資料表缺少必填欄位，以下為必填欄位：");

                mImportWizard.FieldProcessor.RequiredFields.ForEach( x=> RequiredFields.Add(mImportOption.SheetFields.Contains(x)?x:x+"(缺)"));

                strFieldsErrorBuilder.AppendLine(string.Join(",",RequiredFields.ToArray()));
            }

            string FieldsError = strFieldsErrorBuilder.ToString();

            if (!string.IsNullOrEmpty(FieldsError))
            {
                ctlerrors.SetError(lstSheetNames, strFieldsErrorBuilder.ToString());
                return false;
            }
            #endregion

            return true;
        }

        /// <summary>
        /// 進行到下一步
        /// </summary>
        protected override void OnNextButtonClick()
        {
            if (ValidateNext())
            {
                //判斷哪些是使用者可以選擇的欄位，先做此判斷，可以直接跳到下個畫面
                List<string> SelectableFields = mImportWizard.FieldProcessor.GetSelectableFields(new List<string> { }, mImportOption.SheetFields);
                mImportOption.SelectedFields = new List<string>();
                mImportOption.SelectedFields.AddRange(mImportWizard.FieldProcessor.RequiredFields);

                mArgs.SetValue("SelectableFields", SelectableFields);
                //將使用者的選項記錄下來
                mArgs.SetValue("ImportOption",mImportOption);
                //將使用者可選擇的鍵值傳到下個畫面
                mArgs.SetValue("SelectableKeyFields",mSelectablKeyFields);
                base.OnNextButtonClick();
            }
        }
    }
}