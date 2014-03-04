using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Aspose.Cells;
using Campus.DocumentValidator;
using Campus.Validator;
using FISCA;
using System.ComponentModel;

namespace Campus.Import
{
    /// <summary>
    /// 匯入畫面
    /// </summary>
    public partial class SelectImport : WizardForm
    {
        private ArgDictionary mArgs;
        private ImportWizard mImportWizard;
        private ImportFullOption mImportOption;
        private ValidatedInfo mValidatedInfo;
        private string mImportName;

        /// <summary>
        /// 無參數建構式
        /// </summary>
        public SelectImport()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 建構式，傳入精靈參數
        /// </summary>
        /// <param name="args"></param>
        public SelectImport(ArgDictionary args)
            : base(args)
        {
            InitializeComponent();

            this.PreviousButtonVisible = false;
            this.NextButtonTitle = "完成";
            this.NextButtonEnabled = false;

            //將精靈參數存起來用
            mArgs = args;
            mImportOption = mArgs["ImportOption"] as ImportFullOption;
            mImportWizard = mArgs["ImportWizard"] as ImportWizard;
            mValidatedInfo = mArgs["ValidatedInfo"] as ValidatedInfo;

            mImportName = mImportWizard.ValidateRule.Root.GetAttributeText("Name");

            this.Text = mImportName + "-" + this.Text;
            this.Text += "(5/5)"; //功能名稱(目前頁數/總頁數)
            //this.TitleText += "(" + CurrentStep + "/" + TotalStep + ")"; //功能名稱(目前頁數/總頁數)
        }

        private void ChangeProgress(int Progress)
        {
            //Invoke(new Action(() =>
            //    {
                    pgImport.Value = Progress;
            //    }));

            Application.DoEvents();
        }

        /// <summary>
        /// 開始匯入
        /// </summary>
        private void StartImport()
        {
            mImportWizard.Prepare(mImportOption);

            List<string> Result = new List<string>();

            #region 已由Validator提供
            //要匯入的IRowStream列表
            List<IRowStream> Rows = mValidatedInfo.ValidatedPairs[0].Rows;

            pgImport.Maximum = Rows.Count;
            #endregion

            #region 實際執行匯入動作
            if (mImportWizard.IsSplit)
            {
                try
                {
                    FunctionSpliter<IRowStream, string> Spliter = new FunctionSpliter<IRowStream, string>(mImportWizard.SplitSize, mImportWizard.SplitThreadCount);
                    
                    Spliter.Function = ExecuteImport; //指定Spliter的執行函式

                    BackgroundWorker workder = new BackgroundWorker();

                    workder.DoWork += (sender, e) => e.Result = Spliter.Execute(Rows);

                    workder.RunWorkerCompleted += (sender, e) => CompleteImport((List<string>)e.Result);

                    Spliter.ProgressChange = x => Invoke(new Action(() =>  pgImport.Value = x));

                    workder.RunWorkerAsync();
                }
                catch (Exception e)
                {
                    MessageBox.Show("匯入過程中發生錯誤，以下為詳細訊息："+System.Environment.NewLine+e.Message);
                    SmartSchool.ErrorReporting.ReportingService.ReportException(e);
                    this.Close();
                    this.Dispose();
                    return;
                }
            }
            else
            {
                #region 重新讀取Rows
                //Rows = new List<IRowStream>();

                ////建立SheetRowSource物件，用來讀取Excel上的資料
                //SheetRowSource RowSource = new SheetRowSource(mValidatedInfo.ResultHelper);

                //int FirstDataRowIndex = mValidatedInfo.ResultHelper.FirstDataRowIndex;
                //int MaxDataRowIndex = mValidatedInfo.ResultHelper.MaxDataRowIndex;

                ////將所有的SheetRowSource物件複製一份並放到IRowStream列表中
                //for (int i = FirstDataRowIndex; i <= MaxDataRowIndex; i++)
                //{
                //    RowSource.BindRow(i); //將SheetRowSource指定到某列
                //    RowStream RowStream = RowSource.Clone() as RowStream; //將SheetRowSource目前所指向的內容複製
                //    Rows.Add(RowStream); //將RowStream加入到集合中
                //}
                #endregion

                try
                {
                    mImportWizard.PropertyChanged += (sender, e) =>
                    {
                        if (e.PropertyName.Equals("ImportProgress"))
                            Invoke(new Action(() => pgImport.Value = mImportWizard.ImportProgress));
                    };

                    Result = ExecuteImport(Rows);
                    CompleteImport(Result);
                }
                catch (Exception e)
                {
                    MessageBox.Show("匯入過程中發生錯誤，以下為詳細訊息：" + System.Environment.NewLine + e.Message);
                    SmartSchool.ErrorReporting.ReportingService.ReportException(e);
                    this.Close();
                    this.Dispose();
                    return;
                }
            }
            #endregion
        }

        /// <summary>
        /// 匯入完成
        /// </summary>
        /// <param name="Result"></param>
        private void CompleteImport(List<string> Result)
        {
            try
            {
                SaveImportMessage();

                StringBuilder strLogMessage = new StringBuilder();

                Result.ForEach(x => strLogMessage.AppendLine(x));

                string CombineResult = strLogMessage.ToString();

                if (!string.IsNullOrEmpty(CombineResult) && mImportWizard.IsLog)
                    FISCA.LogAgent.ApplicationLog.Log(mImportName, "匯入", CombineResult);

                StringBuilder strCompleteMessage = new StringBuilder();

                if (mImportWizard.Complete != null)
                    strCompleteMessage.AppendLine(mImportWizard.Complete.Invoke());

                txtResult.Text = strCompleteMessage.ToString();
                //顯示圖示
                picComplete.Visible = true;

                this.NextButtonEnabled = true;
            }
            catch (Exception e)
            {
                MessageBox.Show("寫入匯入Log及顯示完成訊息發生錯誤，此錯誤不影響匯入結果，詳細訊息：" + System.Environment.NewLine + e.Message);
            }
        }

        /// <summary>
        /// 將匯入訊息寫入到最後的Excel中
        /// </summary>
        private void SaveImportMessage()
        {
            if (mImportWizard.ImportMessages.Positions.Count > 0)
            {
                string ImportFileName = Path.Combine(Constants.ValidationReportsFolder, Path.GetFileNameWithoutExtension(mImportOption.SelectedDataFile) + "(匯入報告).xls");
                int MaxDataColumn = mValidatedInfo.ResultHelper.Sheet.Cells.MaxDataColumn;

                SheetHelper helper = new SheetHelper(mImportOption.SelectedDataFile);
                helper.SwitchSeet(mImportOption.SelectedSheetName);

                InitialMessageHeader(helper.Sheet);

                mImportWizard.ImportMessages.Positions.ForEach(x => helper.SetValue(x, MaxDataColumn, mImportWizard.ImportMessages[x]));

                helper.Book.Save(ImportFileName);

                btnViewResult.Visible = true;

                btnViewResult.Click += (sender, e) =>
                {
                    try
                    {
                        Process.Start(ImportFileName);
                    }
                    catch (Exception ex)
                    {
                        FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
                    }
                };
            }
        }

        /// <summary>
        /// 最後已有『匯入訊息』欄位，則將其下所有欄位值清空，若無的話加上『匯入訊息』表頭
        /// </summary>
        private void InitialMessageHeader(Worksheet sheet)
        {
            int ImportMessageColumnIndex;

            string MaxDataColumnValue = "" + sheet.Cells[0, sheet.Cells.MaxDataColumn].Value;

            if (MaxDataColumnValue.Contains("匯入訊息")) //如果已經有此欄位
            {
                ImportMessageColumnIndex = sheet.Cells.MaxDataColumn;
                for (int x = 1; x <= sheet.Cells.MaxDataRow; x++) //清空此欄位的內容
                    sheet.Cells[x, ImportMessageColumnIndex].PutValue("");
            }
            else //如果沒有此欄位
            {
                ImportMessageColumnIndex = sheet.Cells.MaxDataColumn + 1;
            }

            sheet.Cells[0, ImportMessageColumnIndex].PutValue("匯入訊息");
        }

        /// <summary>
        /// 包裝執行匯入程式，供FunctionSpliter使用
        /// </summary>
        /// <param name="Rows"></param>
        /// <returns></returns>
        private List<string> ExecuteImport(List<IRowStream> Rows)
        {
            List<string> Result = new List<string>();

            string ImportResult = mImportWizard.Import(Rows);

            Result.Add(ImportResult);

            return Result;
        }

        /// <summary>
        /// 顯示表單
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectImport_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();

            StartImport();
        }
    }
}