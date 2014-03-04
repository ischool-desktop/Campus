using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Campus.Validator;
using FISCA;

namespace Campus.Import
{
    /// <summary>
    /// 資料驗證畫面
    /// </summary>
    public partial class SelectValidate : WizardForm
    {
        private ArgDictionary mArgs;
        private ImportWizard mImportWizard;
        private ImportFullOption mImportOption;
        private ValidatedInfo mValidatedInfo;
        private string mResultFilename;
        private BackgroundWorker worker = new BackgroundWorker();

        public SelectValidate()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 建構式，傳入精靈參數
        /// </summary>
        /// <param name="args"></param>
        public SelectValidate(ArgDictionary args)
            : base(args)
        {
            InitializeComponent();

            //介面初始化設定
            NextButtonTitle = "開始匯入";
            NextButtonEnabled = false;
            btnViewResult.Enabled = false;

            //將精靈參數存起來用
            mArgs = args;
            mImportOption = mArgs["ImportOption"] as ImportFullOption;
            mImportWizard = mArgs["ImportWizard"] as ImportWizard;
            mImportWizard.ImportOption = mImportOption;

            mImportWizard.LoadRule();
            mImportWizard.ValidateRule.Save(Constants.ValidationRulePath);

            mResultFilename = Path.Combine(Constants.ValidationReportsFolder, Path.GetFileNameWithoutExtension(mImportOption.SelectedDataFile) + "(驗證報告).xls");
            this.Text = mImportWizard.ValidateRule.Root.GetAttributeText("Name") + "-" + this.Text;
            this.Text += "(" + CurrentStep + "/" + TotalStep + ")"; //功能名稱(目前頁數/總頁數)
            #region 初始化事件
            //加入可停止執行的事件內容
            lnkCancelValid.Click += (sender, e) => worker.CancelAsync();

            //加入檢查驗證結果程式碼
            btnViewResult.Click += (sender, e) =>
            {
                try
                {
                    Process.Start(mResultFilename);
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
                }
            };
            #endregion

            StartValidate();
        }

        /// <summary>
        /// 開始進行資料驗證
        /// </summary>
        private void StartValidate()
        {
            //建立資料驗證組合
            ValidatePair Pair = new ValidatePair();
            Pair.DataFile = mImportOption.SelectedDataFile; //資料驗證來源檔案
            Pair.DataSheet = mImportOption.SelectedSheetName; //資料驗證來源檔案資料表
            Pair.ValidatorFile = mImportOption.SelectedValidateFile; //資料驗證描述檔

            Validator.Validator valStart = new Validator.Validator();

            if (mImportWizard.CustomValidate != null)
                valStart.CustomValidate = mImportWizard.CustomValidate;

            //執行資料驗證方法
            worker.DoWork += (sender, e) => valStart.Validate(Pair, mResultFilename);

            //將驗證過程顯示在畫面上
            worker.ProgressChanged += (sender, e) =>
            {
                //取得資訊物件(數量/訊息/檔名/工作表名)
                ValidatingPair obj = (ValidatingPair)e.UserState;

                //指定介面進度
                pgValidProgress.Value = e.ProgressPercentage;

                lblProgress.Text = obj.Message;

                //如果錯誤大於0
                if (obj.ErrorCount + obj.WarningCount + obj.AutoCorrectCount > 0)
                {
                    lblErrorCount.Text = ""+obj.ErrorCount;
                    lblWarningCount.Text = ""+obj.WarningCount;
                    lblCorrectCount.Text = ""+obj.AutoCorrectCount;
                }
            };

            //資料驗證完成
            worker.RunWorkerCompleted += (sender, e) =>
            {
                if (e.Error != null)
                    throw e.Error;

                int ErrorText = int.Parse(lblErrorCount.Text);
                int WarningText = int.Parse(lblWarningCount.Text);
                int CorrectText = int.Parse(lblCorrectCount.Text);

                //若是錯誤數量為0才可進行到下一步
                if (lblErrorCount.Text.Equals("0"))
                    this.NextButtonEnabled = true;

                if (ErrorText >= 1) //錯誤大於1
                    pictureBox1.Image = Properties.Resources.filter_data_close_64;
                else if (WarningText >= 1) //警告大於1
                    pictureBox1.Image = Properties.Resources.filter_data_info_64;
                else //無錯誤亦無警告時
                    pictureBox1.Image = Properties.Resources.filter_data_ok_64;

                //將檢視驗證報告的按鈕啟用
                btnViewResult.Enabled = true;

                //將可暫停非同步作業的按鈕取消
                lnkCancelValid.Enabled = false;

                if (mValidatedInfo.ValidatedPairs[0].Exceptions.Count > 0)
                {
                    string ExceptionMessage = string.Empty;

                    foreach (Exception Exception in mValidatedInfo.ValidatedPairs[0].Exceptions)
                        ExceptionMessage += Exception.Message + System.Environment.NewLine;

                    if (!string.IsNullOrEmpty(ExceptionMessage))
                    {
                        ExceptionMessage = "驗證過程中發生錯誤，以下為詳細錯誤訊息：" + System.Environment.NewLine + ExceptionMessage;
                        MessageBox.Show(ExceptionMessage);
                    }

                    pictureBox1.Image = Properties.Resources.filter_data_close_64;

                    this.NextButtonEnabled = false;
                }
            };

            //接收資料驗證進度回報函式
            valStart.Progress  = (message, progress) => worker.ReportProgress(progress, message);
            //接收資料驗證完成函式
            valStart.Complete = (message) =>
               {
                   mValidatedInfo = message;
               };

            //支援非同步取消及進度回報
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;

            //運用非同步執行資料驗證
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// 按下進到匯入畫面，並將驗證結果儲存起來
        /// </summary>
        protected override void OnNextButtonClick()
        {
            //將驗證結果儲存起來
            mArgs.SetValue("ValidatedInfo", mValidatedInfo);
            base.OnNextButtonClick();
        }
    }
}