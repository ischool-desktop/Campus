using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevComponents.DotNetBar;

namespace Campus.Windows
{
    /// <summary>
    /// �q�Ϋ����s�W�B��s�B�R���ζפJ�ץX���
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class CampusConfiguration <T>: Campus.Windows.ConfigurationItem
    {
        private IConfigurationDataAccess<T> mDataAccess;
        private IContentEditor<T> mEditor;

        /// <summary>
        /// �غc���A�ǤJDataAccess����
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
        /// ���o����W��
        /// </summary>
        /// <returns>��ܦW��</returns>
        private string GetSelectedName()
        {
            string SelectedName = string.Empty;
            if (grdNameList.SelectedRows.Count == 1)
                SelectedName = grdNameList.SelectedRows[0].Cells[0].Value.ToString();

            return SelectedName;
        }

        /// <summary>
        /// ���s���o��ƶ��ئW��
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
        /// ��Q�I�s�ɶ�J��
        /// </summary>
        protected override void OnActive()
        {
            Refill();
        }

        /// <summary>
        /// �s�W����
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
        /// �R������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            string SelectedName = GetSelectedName();

            if (!string.IsNullOrEmpty(SelectedName))
            {
                if (MsgBox.Show("�T�w�n�R�� '" + SelectedName + "' �H", "�T�w", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    mDataAccess.Delete(SelectedName);

                    Refill();
                }
            }
        }

        /// <summary>
        /// ��s����
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
                    MsgBox.Show("��J��Ʀ��~�A�L�k�x�s�C\n���ˬd��J��ơC");
                }
            }
        }

        /// <summary>
        /// �������ا��ܮ�
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