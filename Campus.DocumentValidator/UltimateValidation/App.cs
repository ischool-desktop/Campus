using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace UltimateValidation
{
    public partial class App : Form
    {
        public App()
        {
            InitializeComponent();
        }

        private void lblSelectFile_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = new FileInfo(Application.StartupPath).DirectoryName;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txtDataSourcePath.Text = dialog.SelectedPath;
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {

        }
    }
}
