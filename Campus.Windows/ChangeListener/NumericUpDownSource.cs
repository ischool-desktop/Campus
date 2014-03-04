using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Campus.Windows
{
    public class NumericUpDownSource : ChangeSource
    {
        public NumericUpDownSource(NumericUpDown control)
        {
            Control = control;
            OriginValue = Control.Value;
            Control.ValueChanged += new EventHandler(Control_ValueChanged);
        }

        private void Control_ValueChanged(object sender, EventArgs e)
        {
            if (OriginValue != Control.Value)
                RaiseStatusChanged(ValueStatus.Dirty);
            else
                RaiseStatusChanged(ValueStatus.Clean);
        }

        protected NumericUpDown Control { get; private set; }

        /// <summary>
        /// 原始的資料(執行過 Reset 之後的資料)。
        /// </summary>
        protected decimal OriginValue { get; private set; }

        /// <summary>
        /// 重設狀態為「Clean」。
        /// </summary>
        public override void Reset()
        {
            OriginValue = Control.Value;
        }
    }
}
