using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Campus.Windows
{
    public static class MsgBox
    {
        /// <summary>
        /// 訊息
        /// </summary>
        /// <returns></returns>
        public static DialogResult Show(string text)
        {
            return MessageBox.Show(text, "", MessageBoxButtons.OK);
        }

        /// <summary>
        /// 訊息,標題(預設使用確定按鈕)
        /// </summary>
        /// <returns></returns>
        public static DialogResult Show(string text, string caption)
        {
            return MessageBox.Show(text, caption, MessageBoxButtons.OK);
        }

        /// <summary>
        /// 訊息,Buttons(包含產品名稱)
        /// </summary>
        /// <returns></returns>
        public static DialogResult Show(string text, MessageBoxButtons buttons)
        {
            return MsgBox.Show(text, Application.ProductName, buttons);
        }

        /// <summary>
        /// 訊息,Buttons(包含產品名稱)
        /// </summary>
        /// <returns></returns>
        public static DialogResult Show(string text, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        {
            return MessageBox.Show(text, Application.ProductName, buttons, MessageBoxIcon.None, defaultButton);
        }

        /// <summary>
        /// 訊息,標題,Buttons
        /// </summary>
        /// <returns></returns>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            return MessageBox.Show(text, caption, buttons);
        }

        /// <summary>
        /// 訊息,標題,Buttons,Icon
        /// </summary>
        /// <returns></returns>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return MessageBox.Show(text, caption, buttons, icon);
        }

        /// <summary>
        /// 訊息,標題,Buttons,Icon,DefaultButton(預設停在"否"的MessageBox)
        /// </summary>
        /// <returns></returns>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            return MessageBox.Show(text, caption, buttons, icon, defaultButton);
        }
    }
}
