using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Campus
{
    /// <summary>
    /// 錯誤檢查。
    /// </summary>
    public class ErrorTip : ErrorProvider
    {
        private List<Control> _errors;

        public ErrorTip()
        {
            _errors = new List<Control>();
        }

        public new void SetError(Control ctl, string msg)
        {
            if ( string.IsNullOrEmpty(msg) ) //如果訊息設為 Empty，就將移除 Collection。
            {
                if ( _errors.Contains(ctl) )
                    _errors.Remove(ctl);
            }
            else
            {
                if ( !_errors.Contains(ctl) ) //如果未包含，就加入 Collection。
                    _errors.Add(ctl);
            }

            ( this as ErrorProvider ).SetError(ctl, msg);
        }

        public new void Clear()
        {
            _errors = new List<Control>();

            ( this as ErrorProvider ).Clear();
        }

        public bool HasError
        {
            get { return _errors.Count > 0; }
        }

        public bool ContainError(Control ctl)
        {
            return _errors.Contains(ctl);
        }
    }
}
