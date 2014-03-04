using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Campus.Validator
{
    public class FalseRowValidator : Campus.DocumentValidator.IRowVaildator
    {
        #region IRowVaildator 成員

        public bool Validate(Campus.DocumentValidator.IRowStream Value)
        {
            return false;
        }

        public string Correct(Campus.DocumentValidator.IRowStream Value)
        {
            return string.Empty;
        }

        public string ToString(string template)
        {
            return template;
        }

        #endregion
    }
}
