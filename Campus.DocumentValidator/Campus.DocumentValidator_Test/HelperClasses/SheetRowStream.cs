using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.DocumentValidator;
using Aspose.Cells;

namespace Campus.DocumentValidator_Test
{
    class SheetRowStream : IRowStream
    {
        private Worksheet Sheet { get; set; }

        public Dictionary<string, int> Fields { get; set; }

        private int Max { get; set; }

        public SheetRowStream(Worksheet sheet)
        {
            Sheet = sheet;
            Fields = new Dictionary<string, int>();
            Max = Sheet.Cells.MaxDataRow;

            Reset();

            for (byte i = 0; i <= Sheet.Cells.MaxDataColumn; i++)
            {
                string fieldName = Sheet.Cells[0, i].StringValue.Trim();

                if (string.IsNullOrWhiteSpace(fieldName)) continue;

                Fields.Add(Sheet.Cells[0, i].StringValue, i);
            }

        }

        public bool Next()
        {
            if (Position + 1 > Max) return false;

            Position++;

            return true;
        }

        public void Reset()
        {
            //第 0 筆是欄位。
            Position = 0;
        }

        #region IRowStream Members

        public string GetValue(string fieldName)
        {
            return Sheet.Cells[Position, Fields[fieldName]].StringValue;
        }

        public bool Contains(string fieldName)
        {
            return Fields.ContainsKey(fieldName);
        }

        public int Position { get; private set; }

        #endregion

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator()
        {
            return Fields.Keys.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Fields.Keys.GetEnumerator();
        }

        #endregion
    }
}
