using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.DocumentValidator;

namespace Campus.Validator
{
    public class SerialNumberValidator : IRowVaildator
    {

        #region IRowVaildator 成員

        public bool Validate(IRowStream Value)
        {
            return !string.IsNullOrEmpty(Value.GetValue("顯示順序"));
        }

        public string Correct(IRowStream Value)
        {
            return "<A><顯示順序>" + Value.Position + "</顯示順序></A>";
        }

        public string ToString(string template)
        {
            return template;
        }

        #endregion
    }
}
