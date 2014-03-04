using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FISCA;

namespace Campus.Import
{
    /// <summary>
    /// 使用者選擇欄位
    /// 1.若是沒有可選擇欄位，此畫面可不用出現，直接進入到下個畫面。
    /// 2.顯示工作表所有欄位名稱部份暫不設計。
    /// </summary>
    public partial class SelectFields : WizardForm
    {
        private ArgDictionary mArgs;
        private ImportFullOption mImportOption;
        private ImportWizard mImportWizard;
        private List<string> mSelectableFields;

        public SelectFields()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 建構式，傳入精靈參數
        /// </summary>
        /// <param name="args"></param>
        public SelectFields(ArgDictionary args)
            : base(args)
        {
            InitializeComponent();
            NextButtonTitle = "開始驗證";
            mArgs = args;
            mImportOption = mArgs["ImportOption"] as ImportFullOption;
            mImportWizard = mArgs["ImportWizard"] as ImportWizard;
            mSelectableFields = mArgs["SelectableFields"] as List<string>;

            this.Text = mImportWizard.ValidateRule.Root.GetAttributeText("Name") + "-" + this.Text;
            this.Text += "(" + CurrentStep + "/" + TotalStep + ")"; //功能名稱(目前頁數/總頁數)

            RefreshFields();

            chkSelectAll.CheckedChanged += (sender,e) =>
            {
                foreach (ListViewItem Item in lvSourceFieldList.Items)
                {
                    Item.Checked = chkSelectAll.Checked;
                }
            };

            //若是沒有使用者可選擇的欄位，則直接跳到下個畫面；目前設這會有問題...
            //if (mSelectableFields.Count == 0)
            //    this.OnNextButtonClick();
        }

        /// <summary>
        /// 列出使用者可選擇匯入的欄位
        /// </summary>
        private void RefreshFields()
        {
            if (mSelectableFields != null)
            {
                lvSourceFieldList.Items.Clear();

                mSelectableFields.ForEach(x => 
                    {
                        ListViewItem Item = new ListViewItem();
                        Item.Name = x;
                        Item.Text = x;
                        Item.Checked = true;
                        Item.ForeColor = Color.Blue;
                        lvSourceFieldList.Items.Add(Item);
                    }
                );
            }
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
                foreach (ListViewItem Item in lvSourceFieldList.Items)
                    if (Item.Checked && !mImportOption.SelectedFields.Contains(Item.Name))
                        mImportOption.SelectedFields.Add(Item.Name);

                mArgs.SetValue("ImportOption",mImportOption);

                base.OnNextButtonClick();
            }
        }
    }
}