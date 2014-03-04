using System;
using System.Collections.Generic;
using System.Linq;
using FISCA;
using FISCA.Presentation.Controls;

namespace Campus.Import
{
    /// <summary>
    /// 選擇鍵值及異動動作表單
    /// 1.預設的動作為新增或更新。
    /// 2.待處理，若隨時將畫面關閉則會爆掉。
    /// </summary>
    public partial class SelectKey : WizardForm
    {
        private Dictionary<string, List<string>> mSelectableKeyFields;
        private ArgDictionary mArgs;
        private ImportFullOption mImportOption;
        private ImportWizard mImportWizard;

        public SelectKey()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 建構式，傳入精靈參數
        /// </summary>
        /// <param name="args"></param>
        public SelectKey(ArgDictionary args)
            : base(args)
        {
            InitializeComponent();
            mArgs = args;
            mImportOption = args["ImportOption"] as ImportFullOption;
            mImportWizard = args["ImportWizard"] as ImportWizard;
            this.Text = mImportWizard.ValidateRule.Root.GetAttributeText("Name") + "-" + this.Text;
            this.Text += "(" + CurrentStep + "/" + TotalStep + ")"; //功能名稱(目前頁數/總頁數)

            #region 將使用者可選擇的鍵值顯示在畫面上
            mSelectableKeyFields = args["SelectableKeyFields"] as Dictionary<string, List<string>>;

            int cboIdFieldLen = cboIdField.Width;

            foreach (List<string> KeyFields in mSelectableKeyFields.Values)
            {
                string NewItem = string.Join(",", KeyFields.ToArray());

                cboIdField.Items.Add(NewItem);

                if (NewItem.Length > cboIdFieldLen)
                    cboIdFieldLen = NewItem.Length;
            }

            if (cboIdField.Items.Count > 0)
            {
                cboIdField.SelectedIndex = 0;
                //cboIdField.Width = cboIdFieldLen;
            }
            #endregion

            #region 將使用者可選擇的資料動作
            ImportAction Actions = mImportWizard.GetSupportActions();

            bool IsInsert = (Actions & ImportAction.Insert) == ImportAction.Insert;
            bool IsUpdate = (Actions & ImportAction.Update) == ImportAction.Update;
            bool IsInsertOrUpdate = (Actions & ImportAction.InsertOrUpdate) == ImportAction.InsertOrUpdate;
            bool IsCover = (Actions & ImportAction.Cover) == ImportAction.Cover;
            bool IsDelete = (Actions & ImportAction.Delete) == ImportAction.Delete;

            if (IsInsert)
                lstActions.Items.Add("新增資料");
            if (IsUpdate)
                lstActions.Items.Add("更新資料");
            if (IsInsertOrUpdate)
                lstActions.Items.Add("新增或更新資料");
            if (IsCover)
                lstActions.Items.Add("覆蓋資料");

            lstActions.SelectedIndexChanged += (sender, e) =>
            {
                switch ("" + lstActions.SelectedItem)
                {
                    case "新增資料": lblImportActionMessage.Text = "此選項是將所有資料新增到資料庫中，不會對現有的資料進行任何修改動作。"; break;
                    case "更新資料": lblImportActionMessage.Text = "此選項將修改資料庫中的現有資料，會依據您所指定的識別欄修改資料庫中具有相同識別的資料。"; break;
                    case "新增或更新資料": lblImportActionMessage.Text = "此選項是將資料新增或更新到資料庫中，會針對您的資料來自動判斷新增或更新。"; break;
                    case "覆蓋資料": lblImportActionMessage.Text = "此選項是將資料庫中的資料都先刪除再全部新增"; break;
                    case "刪除資料": lblImportActionMessage.Text = "此選項將依匯入資料中的鍵值刪除資料庫中的現有資料，請您務必小心謹慎使用。"; break;
                };
            };

            lstActions.KeyDown  += (sender, e) =>
            {
                if (e.KeyData == System.Windows.Forms.Keys.Delete)
                    if (IsDelete)
                        lstActions.Items.Add("刪除資料"); 
            };

            lstActions.SelectedIndex = 0;

            #endregion
        }

        /// <summary>
        /// 取得使用者選擇的匯入類型
        /// </summary>
        /// <returns></returns>
        private ImportAction GetSelectedImportAction()
        {
            switch ("" + lstActions.SelectedItem)
            {
                case "新增資料": return ImportAction.Insert;
                case "更新資料": return ImportAction.Update;
                case "新增或更新資料": return ImportAction.InsertOrUpdate;
                case "覆蓋資料": return ImportAction.Cover;
                case "刪除資料": return ImportAction.Delete;
            };

            throw new Exception("使用者沒有選擇異動類別!");
        }

        /// <summary>
        /// 驗證是否可進行到下個畫面
        /// </summary>
        /// <returns></returns>
        private bool ValidateNext()
        {
            return true;
        }

        protected override void OnNextButtonClick()
        {
            if (ValidateNext())
            {
                try
                {
                    #region 將使用者所選擇鍵值及動作記錄下來
                    //記錄選取鍵值及動作
                    mImportOption.SelectedKeyFields = mSelectableKeyFields[mSelectableKeyFields.Keys.ToList()[cboIdField.SelectedIndex]];
                    //mImportOption.SelectedFields = new List<string>(); //初始化SelectedFields，若是SelectableFields為0，則會判斷跳到下個畫面。
                    mImportOption.Action = GetSelectedImportAction();

                    //再次取得使用者選取的鍵值欄位
                    List<string> SelectedKeyFields = mImportOption.SelectedKeyFields;

                    SelectedKeyFields.ForEach(x =>
                        {
                            if (!mImportOption.SelectedFields.Contains(x))
                                mImportOption.SelectedFields.Add(x);
                        }
                    );

                    //判斷哪些是使用者可以選擇的欄位，先做此判斷，可以直接跳到下個畫面
                    List<string> SelectableFields = mImportWizard.FieldProcessor.GetSelectableFields(SelectedKeyFields, mImportOption.SheetFields);

                    mArgs.SetValue("ImportOption", mImportOption);
                    mArgs.SetValue("SelectableFields", SelectableFields);
                    #endregion

                    base.OnNextButtonClick();
                }
                catch (Exception e)
                {
                    MsgBox.Show(e.Message);
                }
            }
        }
    }
}