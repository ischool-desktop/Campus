using System;
using System.Drawing;
using System.Windows.Forms;

namespace Campus.Windows
{
    /// <summary>
    /// 最後呈現的設定表單
    /// </summary>
    public partial class ConfigurationForm : FISCA.Presentation.Controls.BaseForm, IConfigurationItem
    {
        private IConfigurationItem mConfigurationItem;

        public ConfigurationForm(IConfigurationItem vConfigurationItem)
        {
            InitializeComponent();
            mConfigurationItem = vConfigurationItem;

            int vHeight, vWidth;
            if (mConfigurationItem.HasControlPanel)
            {
                vHeight = Math.Max(vConfigurationItem.ControlPanel.Height, vConfigurationItem.ContentPanel.Height);
                vWidth = vConfigurationItem.ControlPanel.Width + vConfigurationItem.ContentPanel.Width;
            }
            else
            {
                vHeight = vConfigurationItem.ContentPanel.Height;
                vWidth = vConfigurationItem.ContentPanel.Width;
            }

            Size = new Size(vWidth, vHeight + 20);
        }

        /// <summary>
        /// 載入表單事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigurationForm_Load(object sender, EventArgs e)
        {
            SC.Visible = mConfigurationItem.HasControlPanel;
            if (!mConfigurationItem.HasControlPanel)
            {
                Controls.Add(mConfigurationItem.ContentPanel);
            }
            else
            {
                SC.Panel1.Controls.Add(mConfigurationItem.ControlPanel);
                SC.Panel2.Controls.Add(mConfigurationItem.ContentPanel);
                mConfigurationItem.ControlPanel.Dock = DockStyle.Fill;
            }
            mConfigurationItem.ContentPanel.Dock = DockStyle.Fill;

            Text = Caption;
            mConfigurationItem.Active();
        }

        #region IConfigurationItem 成員

        public void Active()
        {
            ShowDialog();
        }

        public string Caption
        {
            get { return mConfigurationItem.Caption; }
        }

        public string Category
        {
            get { return mConfigurationItem.Category; }
        }

        public Panel ContentPanel
        {
            get { return mConfigurationItem.ContentPanel; }
        }

        public Panel ControlPanel
        {
            get { return mConfigurationItem.ControlPanel; }
        }

        public bool HasControlPanel
        {
            get { return mConfigurationItem.HasControlPanel; }
        }

        public Image Image
        {
            get { return mConfigurationItem.Image; }
        }

        public string TabGroup
        {
            get { return mConfigurationItem.TabGroup; }
        }

        #endregion
    }
}