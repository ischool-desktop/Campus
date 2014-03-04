using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Aspose.Cells;
using Campus.DocumentValidator;
using FISCA.Presentation.Controls;

namespace iValidator
{
    public class Validator
    {
        public static Action<ValidatorInfoObj, int> ProgressFeedback;

        private static RowMessages RowMessages;
        private static int ErrorCount;
        private static int WarningCount;
        private static int AutoCorrectCount;

        static Validator()
        {
            RowMessages = new RowMessages();
        }

        /// <summary>
        /// 資料驗證
        /// </summary>
        /// <param name="ValidatorFile">驗證規則路徑</param>
        /// <param name="DataFile">驗證檔案路徑</param>
        /// <param name="DataSheet">驗證的工作表</param>
        /// <param name="OutputFile">報告輸出路徑</param>
        static public void Validate(string ValidatorFile, string DataFile, string DataSheet, string OutputFile)
        {
            Validate(ValidatorFile, DataFile, DataSheet, OutputFile, OutputOptions.Full);
        }

        /// <summary>
        /// 資料驗證
        /// </summary>
        /// <param name="ValidatorFile">驗證規則路徑</param>
        /// <param name="DataFile">驗證檔案路徑</param>
        /// <param name="DataSheet">驗證的工作表</param>
        /// <param name="OutputFile">報告輸出路徑</param>
        /// <param name="OutputOptions">輸出類型:全部(Full),正確(Correct),錯誤(Error)</param>
        static public void Validate(string ValidatorFile, string DataFile, string DataSheet, string OutputFile, OutputOptions OutputOptions)
        {
            ValidatorPair validatorPair = new ValidatorPair(ValidatorFile, DataFile, DataSheet);
            Validate(new ValidatorPair[] { validatorPair }, OutputFile, OutputOptions);
        }

        /// <summary>
        /// 資料驗證
        /// </summary>
        /// <param name="ValidatePair">資料驗證相關資訊(檔案,工作表,規則)</param>
        /// <param name="OutputFile">報告輸出路徑</param>
        static public void Validate(ValidatorPair ValidatePair, string OutputFile)
        {
            Validate(ValidatePair, OutputFile, OutputOptions.Full);
        }

        /// <summary>
        /// 資料驗證
        /// </summary>
        /// <param name="ValidatePair">資料驗證相關資訊(檔案,工作表,規則)</param>
        /// <param name="OutputFile">報告輸出路徑</param>
        /// <param name="OutputOptions">輸出類型:全部(Full),正確(Correct),錯誤(Error)</param>
        static public void Validate(ValidatorPair ValidatePair, string OutputFile, OutputOptions OutputOptions)
        {
            Validate(new ValidatorPair[] { ValidatePair }, OutputFile, OutputOptions);
        }

        /// <summary>
        /// 資料驗證
        /// </summary>
        /// <param name="ValidatorPairs">資料驗證相關資訊清單(檔案,工作表,規則)</param>
        /// <param name="OutputFile">報告輸出路徑</param>
        static public void Validate(IEnumerable<ValidatorPair> ValidatorPairs, string OutputFile)
        {
            Validate(ValidatorPairs, OutputFile, OutputOptions.Full);
        }

        /// <summary>
        /// 資料驗證
        /// </summary>
        /// <param name="ValidatorPairs">資料驗證相關資訊清單(檔案,工作表,規則)</param>
        /// <param name="OutputFile">報告輸出路徑</param>
        /// <param name="OutputOptions">輸出類型:全部(Full),正確(Correct),錯誤(Error)</param>
        static public void Validate(IEnumerable<ValidatorPair> ValidatorPairs, string OutputFile, OutputOptions OutputOptions)
        {
            if (ValidatorPairs.Count() == 0)
                return;

            //判斷來源檔案是否都屬於同一個檔案，並且輸出選項只為Full
            bool IsOptimizeMode = ValidatorPairs.Select(x => x.DataFile).Distinct().Count() == 1 && OutputOptions == OutputOptions.Full;
            RowMessages = new RowMessages();
            Workbook outputBook = new Workbook();
            outputBook.Worksheets.Clear();

            List<ValidatorPair> PairsList = ValidatorPairs.ToList();

            SheetHelper Sheet = new SheetHelper(PairsList[0].DataFile, PairsList[0].DataSheet);

            DocumentValidate docValidate = new DocumentValidate();
            docValidate.AutoCorrect += new EventHandler<AutoCorrectEventArgs>(docValidate_AutoCorrect);
            docValidate.ErrorCaptured += new EventHandler<ErrorCapturedEventArgs>(docValidate_ErrorCaptured);

            foreach (ValidatorPair each in ValidatorPairs)
            {
                //初始化 Counter
                ErrorCount = WarningCount = AutoCorrectCount = 0;

                #region 讀取驗證規則
                XmlDocument xmldoc = new XmlDocument();

                try
                {
                    xmldoc.Load(each.ValidatorFile);
                    docValidate.LoadRule(xmldoc.DocumentElement);
                }
                catch (Exception ex)
                {
                    MsgBox.Show("讀取驗證規則路徑發生錯誤!\n\n" + ex.Message);
                    SmartSchool.ErrorReporting.ReportingService.ReportException(ex);
                }
                #endregion

                #region 驗證資料

                //將驗證訊息都清空
                RowMessages.Clear();

                //假設資料來源都是同個檔案就不重新產生SheetHelper，只是切換Sheet
                if (IsOptimizeMode)
                    Sheet.SwitchSeet(each.DataSheet);
                else
                    Sheet = new SheetHelper(each.DataFile, each.DataSheet);

                if (ProgressFeedback != null)
                {
                    ValidatorInfoObj obj = new ValidatorInfoObj();

                    obj.ErrorCount = ErrorCount;
                    obj.WarningCount = WarningCount;
                    obj.AutoCorrectCount = AutoCorrectCount;

                    obj.message = "目前驗證: " + Sheet.Sheet.Name;
                    obj.FileName = Path.GetFileName(each.DataFile);
                    obj.SheetName = each.DataSheet;
                    ProgressFeedback(obj, 0);
                }

                double count = 0;
                double total = Sheet.DataRowCount;

                //開始主鍵驗證
                docValidate.BeginDetecteDuplicate();
                Dictionary<string, Exception> exceptions = new Dictionary<string, Exception>();

                SheetRowSource rowSource = new SheetRowSource(Sheet);
                for (int i = Sheet.FirstDataRowIndex; i <= Sheet.DataRowCount; i++)
                {
                    try
                    {
                        rowSource.BindRow(i);
                        bool valid = docValidate.Validate(rowSource);
                        if (ProgressFeedback != null)
                        {
                            ValidatorInfoObj obj = new ValidatorInfoObj();
                            obj.ErrorCount = ErrorCount;
                            obj.WarningCount = WarningCount;
                            obj.AutoCorrectCount = AutoCorrectCount;

                            obj.message = "目前驗證: " + Sheet.Sheet.Name;
                            obj.FileName = Path.GetFileName(each.DataFile);
                            obj.SheetName = each.DataSheet;
                            ProgressFeedback(obj, (int)(++count * 100 / total));
                        }
                    }
                    catch (Exception e)
                    {
                        if (ProgressFeedback != null)
                        {
                            ValidatorInfoObj obj = new ValidatorInfoObj();
                            obj.ErrorCount = ErrorCount;
                            obj.WarningCount = WarningCount;
                            obj.AutoCorrectCount = AutoCorrectCount;
                            obj.message = "驗證錯誤: " + Sheet.Sheet.Name + e.Message;
                            obj.FileName = Path.GetFileName(each.DataFile);
                            obj.SheetName = each.DataSheet;
                            ProgressFeedback(obj, (int)(++count * 100 / total));
                        }

                        if (!exceptions.ContainsKey(e.Message))
                            exceptions.Add(e.Message, e);
                        //SmartSchool.ErrorReporting.ReportingService.ReportException(e);
                    }
                }

                exceptions.Values.ToList().ForEach(ex => SmartSchool.ErrorReporting.ReportingService.ReportException(ex));

                //結束主鍵驗證
                IList<DuplicateData> DuplicateDataList = docValidate.EndDetecteDuplicate();

                foreach (DuplicateData dup in DuplicateDataList)
                {
                    ErrorType errorType = (dup.ErrorType == Campus.DocumentValidator.ErrorType.Error) ? ErrorType.Error : ErrorType.Warning;
                    foreach (DuplicateRecord dupRecord in dup)
                    {
                        List<string> list = new List<string>();
                        for (int i = 0; i < dupRecord.Values.Count; i++)
                            list.Add(string.Format("「{0}：{1}」", dup.Fields[i], dupRecord.Values[i]));

                        string msg = string.Format("{0}的組合不能重覆。", string.Join("、", list.ToArray()));
                        foreach (int position in dupRecord.Positions)
                            RowMessages[position].MessageItems.Add(new MessageItem(errorType, ValidatorType.Row, msg));
                    }
                }

                #endregion

                #region 產生驗證訊息到 Excel

                //驗證訊息ColumnIndex

                int reportColIndex;
                string MaxDataColumnValue = "" + Sheet.Sheet.Cells[0, Sheet.Sheet.Cells.MaxDataColumn].Value;
                if (MaxDataColumnValue.Contains("驗證訊息")) //如果已經有此欄位
                {
                    reportColIndex = Sheet.Sheet.Cells.MaxDataColumn;
                    for (int x = 1; x <= Sheet.Sheet.Cells.MaxDataRow; x++) //清空此欄位的內容
                    {
                        Sheet.Sheet.Cells[x, reportColIndex].PutValue("");
                    }
                }
                else //如果沒有此欄位
                {
                    reportColIndex = Sheet.Sheet.Cells.MaxDataColumn + 1;
                }

                Sheet.Sheet.Cells[0, reportColIndex].PutValue("驗證訊息");
                //驗證欄位是否缺少
                rowSource.BindRow(0);
                string headerMessage = docValidate.ValidateHeader(rowSource);

                if (RowMessages.Count <= 0 && string.IsNullOrEmpty(headerMessage)) continue;

                Sheet.Sheet.Cells[0, reportColIndex].PutValue("驗證訊息" + (!string.IsNullOrEmpty(headerMessage) ? "\r\n" + headerMessage : ""));
                Sheet.Sheet.Cells[0, reportColIndex].Style.IsTextWrapped = true;

                if (ProgressFeedback != null)
                {
                    ValidatorInfoObj obj = new ValidatorInfoObj();
                    obj.ErrorCount = 0;
                    obj.message = "寫入驗證訊息: ";
                    obj.FileName = Path.GetFileName(each.DataFile);
                    obj.SheetName = each.DataSheet;
                    ProgressFeedback(obj, 0);
                }

                count = 0;
                total = RowMessages.Count;

                Dictionary<int, string> temp = new Dictionary<int, string>();

                foreach (RowMessage rowMessage in RowMessages)
                    temp.Add(rowMessage.Position, rowMessage.BestMessageItems.GetDescription());
                RowMessages.RefreshCount();

                foreach (RowMessage rowMessage in RowMessages)
                {
                    //string strDescription = rowMessage.BestMessageItems.GetDescription();
                    string strDescription = temp[rowMessage.Position];
                    Cell cell = Sheet.Sheet.Cells[rowMessage.Position, reportColIndex];
                    cell.PutValue(strDescription);
                    cell.Style.IsTextWrapped = true;
                    if (ProgressFeedback != null)
                    {
                        ValidatorInfoObj obj = new ValidatorInfoObj();

                        obj.ErrorCount = RowMessages.ErrorCount;
                        obj.WarningCount = RowMessages.WarningCount;
                        obj.AutoCorrectCount = RowMessages.AutoCorrectCount;

                        obj.message = "寫入驗證訊息: " + Sheet.Sheet.Name;
                        obj.FileName = Path.GetFileName(each.DataFile);
                        obj.SheetName = each.DataSheet;

                        ProgressFeedback(obj, (int)(++count * 100 / total));
                    }
                }

                count = 0;

                if (IsOptimizeMode)
                {
                    if (ProgressFeedback != null)
                    {
                        ValidatorInfoObj obj = new ValidatorInfoObj();

                        obj.ErrorCount = RowMessages.ErrorCount;
                        obj.WarningCount = RowMessages.WarningCount;
                        obj.AutoCorrectCount = RowMessages.AutoCorrectCount;

                        obj.message = "調整最適欄寬: " + Sheet.Sheet.Name;
                        obj.FileName = Path.GetFileName(each.DataFile);
                        obj.SheetName = each.DataSheet;

                        ProgressFeedback(obj, 0);
                    }
                    Sheet.Sheet.AutoFitColumn(reportColIndex);
                }

                bool full = (OutputOptions & OutputOptions.Full) == OutputOptions.Full;
                bool correct = (OutputOptions & OutputOptions.Correct) == OutputOptions.Correct;
                bool error = (OutputOptions & OutputOptions.Error) == OutputOptions.Error;

                Worksheet fullSheet = null;
                Worksheet correctSheet = null;
                Worksheet errorSheet = null;

                if (full && !IsOptimizeMode)
                    fullSheet = outputBook.Worksheets[outputBook.Worksheets.Add()];
                if (correct)
                {
                    correctSheet = outputBook.Worksheets[outputBook.Worksheets.Add()];
                    correctSheet.Cells.CopyRow(Sheet.Sheet.Cells, 0, 0);
                }
                if (error)
                {
                    errorSheet = outputBook.Worksheets[outputBook.Worksheets.Add()];
                    errorSheet.Cells.CopyRow(Sheet.Sheet.Cells, 0, 0);
                }
                List<int> Positions = RowMessages.Positions;

                int correctCount = 1;
                int errorCount = 1;

                if (!IsOptimizeMode)
                {
                    for (int i = Sheet.FirstDataRowIndex; i <= Sheet.DataRowCount; i++)
                    {
                        if (!Positions.Contains(i) && correct)
                            correctSheet.Cells.CopyRow(Sheet.Sheet.Cells, i, correctCount++);
                        else if (Positions.Contains(i) && error)
                            errorSheet.Cells.CopyRow(Sheet.Sheet.Cells, i, errorCount++);
                        if (ProgressFeedback != null)
                        {
                            ValidatorInfoObj obj = new ValidatorInfoObj();

                            obj.ErrorCount = RowMessages.ErrorCount;
                            obj.WarningCount = RowMessages.WarningCount;
                            obj.AutoCorrectCount = RowMessages.AutoCorrectCount;

                            obj.message = "產生驗證報告: " + Sheet.Sheet.Name;
                            obj.FileName = Path.GetFileName(each.DataFile);
                            obj.SheetName = each.DataSheet;

                            ProgressFeedback(obj, (int)(++count * 100 / total));
                        }
                    }
                }

                //假設選項有full，並且都不來自同個來源才進行複製
                if (full && (!IsOptimizeMode))
                {
                    fullSheet.Copy(Sheet.Sheet);
                    fullSheet.AutoFitColumn(reportColIndex);
                    fullSheet.AutoFitRows();
                    fullSheet.Name = Sheet.Sheet.Name;
                }
                if (correct)
                {
                    correctSheet.AutoFitColumn(reportColIndex);
                    correctSheet.AutoFitRows();
                    correctSheet.Name = Sheet.Sheet.Name + "(正確)";
                }
                if (error)
                {
                    errorSheet.AutoFitColumn(reportColIndex);
                    errorSheet.AutoFitRows();
                    errorSheet.Name = Sheet.Sheet.Name + "(錯誤)";
                }
                #endregion
            }

            //if (outputBook.Worksheets.Count > 0)
            //{
            try
            {
                if (ProgressFeedback != null)
                {
                    ValidatorInfoObj obj = new ValidatorInfoObj();
                    obj.ErrorCount = 0;
                    obj.WarningCount = 0;
                    obj.AutoCorrectCount = 0;
                    obj.message = "驗證報告存檔中!";
                    obj.FileName = "";
                    obj.SheetName = "";

                    ProgressFeedback(obj, 0);
                }

                if (IsOptimizeMode)
                    Sheet.Book.Save(OutputFile);
                else
                    outputBook.Save(OutputFile);
            }
            catch (System.OutOfMemoryException ex)
            {

                string strMessage = "『"+OutputFile + "』檔案在儲存時『記憶體不足（Out Of Memory）』!!\r\n";
                
                strMessage += "解決方案一：將系統未使用程式關閉增加可用之記憶體資源。\r\n";
                strMessage += "解決方案二：建議您運用『切Excel工作表小工具』將原始檔案各個工作表另存為活頁簿，並將原始檔案移至其他目錄，然後再重新驗證。";

                MsgBox.Show(strMessage);

                //MsgBox.Show("記憶體不足!!\n建議將系統部份未使用程式關閉\n增加可用之記憶體資源\n" + ex.Message);
                SmartSchool.ErrorReporting.ReportingService.ReportException(ex);
            }
            catch (System.IO.IOException ex)
            {
                MsgBox.Show("檔案可能開啟中!\n" + ex.Message);
                SmartSchool.ErrorReporting.ReportingService.ReportException(ex);
            }
            catch (Exception ex)
            {
                MsgBox.Show("驗證報告儲存發生錯誤!\n" + ex.Message);
                SmartSchool.ErrorReporting.ReportingService.ReportException(ex);
            }
            //}
        }

        /// <summary>
        /// 自動修正事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void docValidate_AutoCorrect(object sender, AutoCorrectEventArgs e)
        {
            #region 將修正後的值寫入該欄位
            SheetRowSource row = e.Row as SheetRowSource;
            SheetHelper sheetHelper = row.Sheet;

            int FieldIndex = sheetHelper.GetFieldIndex(e.FieldName);
            if (FieldIndex != -1) //當自動修正,Excel-Sheet內卻沒有欄位
            {
                sheetHelper.Sheet.Cells[row.Position, sheetHelper.GetFieldIndex(e.FieldName)].PutValue(e.NewValue);

                string Description = string.Format("「{0}」值由『{1}』改為『{2}』。", e.FieldName, e.OldValue, e.NewValue);
                List<string> Fields = new List<string>() { e.FieldName };
                ErrorType ErrorType = ErrorType.Correct;
                ValidatorType ValidatorType = ValidatorType.Field;

                MessageItem item = new MessageItem(ErrorType, ValidatorType, Description, Fields);

                RowMessages[e.Row.Position].MessageItems.Add(item);

                AutoCorrectCount++;
            }
            else //就不處理
            {

            }
            #endregion
        }

        /// <summary>
        /// 錯誤或警告事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void docValidate_ErrorCaptured(object sender, ErrorCapturedEventArgs e)
        {
            string Description = e.Description;
            List<string> Fields = new List<string>() { e.FieldName };
            ErrorType ErrorType = (e.ErrorType == Campus.DocumentValidator.ErrorType.Error) ? ErrorType.Error : ErrorType.Warning;
            ValidatorType ValidationType = ValidatorType.Field;

            MessageItem item = new MessageItem(ErrorType, ValidationType, Description, Fields);

            RowMessages[e.Row.Position].MessageItems.Add(item);

            if (e.ErrorType == Campus.DocumentValidator.ErrorType.Error)
                ErrorCount++;
            else
                WarningCount++;
        }

    }

    /// <summary>
    /// 代表實際驗證的組合，驗證File路徑為Excel檔案、DataSheet為Excel檔案中的Sheet名稱，ValidatorFile為驗證檔案。
    /// </summary>
    public struct ValidatorPair
    {
        /// <summary>
        /// 驗證規則路徑
        /// </summary>
        public string ValidatorFile { get; set; }

        /// <summary>
        /// 驗證File路徑
        /// </summary>
        public string DataFile { get; set; }

        /// <summary>
        /// 驗證Sheet名稱
        /// </summary>
        public string DataSheet { get; set; }

        public ValidatorPair(string ValidatorFile, string DataFile, string DataSheet)
            : this()
        {
            this.ValidatorFile = ValidatorFile;
            this.DataFile = DataFile;
            this.DataSheet = DataSheet;
        }
    }
}