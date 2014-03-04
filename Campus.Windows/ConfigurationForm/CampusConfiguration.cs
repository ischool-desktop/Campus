using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevComponents.DotNetBar;

namespace Campus.Windows
{
    /// <summary>
    /// 通用型的新增、更新、刪除及匯入匯出表單
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class CampusConfiguration <T>: Campus.Windows.ConfigurationItem
    {
        private IConfigurationDataAccess<T> mDataAccess;
        private IContentEditor<T> mEditor;

        /// <summary>
        /// 建構式，傳入DataAccess物件
        /// </summary>
        /// <param name="vDataAccess"></param>
        public CampusConfiguration(IConfigurationDataAccess<T> vDataAccess,IContentEditor<T> vEditor)
        {
            InitializeComponent();
            mDataAccess = vDataAccess;
            mEditor = vEditor;
            this.pnlExpandable.TitleText = mDataAccess.DisplayName;
            this.Caption = mDataAccess.DisplayName;
            this.pnlEditor.Controls.Add(mEditor.Control);
            mEditor.Control.Dock = DockStyle.Fill;

            if (mDataAccess.ExtraCommands != null)
                mDataAccess.ExtraCommands.ForEach
                (x=>
                    {
                        ButtonItem ButtonItem = new ButtonItem(x.Name,x.Text);
                        ButtonItem.Click += (sender, e) =>
                        {
                            x.Execute(GetSelectedName());
                            if (x.IsChangeData)
                            {
                                mEditor.Prepare();
                                Refill();
                            }
                        };
                        btnMore.SubItems.Add(ButtonItem);
                    }
                );

            //btnMore.Click += (sender, e) =>
            //{
            //    string SelectedName = GetSelectedName();

            //    mDataAccess.Export(SelectedName);
            //};

            //btnImport.Click += (sender, e) =>
            //{
            //    mDataAccess.Import();
            //    mEditor.Prepare();
            //    Refill();
            //};
        }

        /// <summary>
        /// 取得選取名稱
        /// </summary>
        /// <returns>選擇名稱</returns>
        private string GetSelectedName()
        {
            string SelectedName = string.Empty;
            if (grdNameList.SelectedRows.Count == 1)
                SelectedName = grdNameList.SelectedRows[0].Cells[0].Value.ToString();

            return SelectedName;
        }

        /// <summary>
        /// 重新取得資料項目名稱
        /// </summary>
        private void Refill()
        {
            string SelectedName = GetSelectedName();

            grdNameList.Rows.Clear();

            List<string> Names = mDataAccess.SelectKeys();

            Names.Sort();

            foreach (string Name in Names)
            {
                if (Name.Equals(SelectedName))
                    grdNameList.Rows[grdNameList.Rows.Add(Name)].Selected = true;
                else
                    grdNameList.Rows.Add(Name);
            }
        }

        /// <summary>
        /// 當被呼叫時填入時
        /// </summary>
        protected override void OnActive()
        {
            Refill();
        }

        /// <summary>
        /// 新增項目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInsert_Click(object sender, EventArgs e)
        {
            NameCreatorForm<T> vCreatorForm = new NameCreatorForm<T>(mDataAccess);

            DialogResult Result = vCreatorForm.ShowDialog();

            if (Result == DialogResult.OK)
            {
                string NewName = vCreatorForm.NewName;
                string DuplicateName = vCreatorForm.DuplicateName;

                if (!string.IsNullOrEmpty(NewName))
                {
                    mDataAccess.Insert(NewName, DuplicateName);
                    Refill();
                }
            }
        }

        /// <summary>
        /// 刪除項目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            string SelectedName = GetSelectedName();

            if (!string.IsNullOrEmpty(SelectedName))
            {
                if (MsgBox.Show("確定要刪除 '" + SelectedName + "' ？", "確定", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    mDataAccess.Delete(SelectedName);

                    Refill();
                }
            }
        }

        /// <summary>
        /// 更新項目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (grdNameList.SelectedRows.Count == 1)
            {
                if (mEditor.Validate())
                {
                    T UpdateRecord = mEditor.Content;

                    mDataAccess.Save(UpdateRecord);

                    Refill();
                }
                else
                {
                    MsgBox.Show("輸入資料有誤，無法儲存。\n請檢查輸入資料。");
                }
            }
        }

        /// <summary>
        /// 當選取項目改變時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdNameList_SelectionChanged(object sender, EventArgs e)
        {
            UserControl vControl = this.mEditor.Control;
            vControl.Visible = ( grdNameList.SelectedRows.Count == 1 );
            string SelectedName = GetSelectedName();

            if (!string.IsNullOrEmpty(SelectedName))
            {
                T SelectRecord = mDataAccess.Select(SelectedName);

                mEditor.Content = SelectRecord;
            }
        }
    }
}