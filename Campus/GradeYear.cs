using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Campus
{
    /// <summary>
    /// 代表年級資訊
    /// </summary>
    public struct GradeYear
    {
        private static GradeYear[] _defaults;

        static GradeYear()
        {
            _defaults = new GradeYear[] { };
        }

        private string[] _cnumber;

        /// <summary>
        /// 建立年級物件。
        /// </summary>
        /// <param name="number">年級等級，小於0一律會變成未分年級。</param>
        internal GradeYear(int number)
            : this()
        {
            _cnumber = new string[] { "〇", "一", "二", "三", "四", "五", "六", "七", "八", "九", };
            Number = number;
        }

        /// <summary>
        /// 取得年級，例：1、2、3。
        /// </summary>
        public int Number { get; private set; }

        /// <summary>
        /// 取得中文的年級數字，例：一、二、三。
        /// </summary>
        public string ChineseNumber
        {
            get
            {
                if (Number < 0)
                    return string.Empty;

                if (Number <= _cnumber.Length)
                    return _cnumber[Number];
                else
                    return Number.ToString();
            }
        }

        /// <summary>
        /// 取得中文的年級標題，例：一年級、二年級、三年級。
        /// </summary>
        public string ChineseTitle
        {
            get
            {
                if (Number < 0)
                    return "未分年級";

                return ChineseNumber + "年級";
            }
        }

        /// <summary>
        /// 提供 ToString 方法呼叫。
        /// </summary>
        public Func<GradeYear, string> ToStringFormatter { get; set; }

        /// <summary>
        /// 取得中文的年級標題。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (ToStringFormatter == null)
                return ChineseTitle;
            else
                return ToStringFormatter(this);
        }

        public static GradeYear operator +(GradeYear a, int b)
        {
            return a + GradeYear.ToGradeYear(b);
        }

        public static GradeYear operator +(GradeYear a, GradeYear b)
        {
            int a1, b1;
            Normalize(a, b, out  a1, out b1);
            return GradeYear.ToGradeYear(a1 + b1);
        }

        public static GradeYear operator -(GradeYear a, int b)
        {
            return a - GradeYear.ToGradeYear(b);
        }

        public static GradeYear operator -(GradeYear a, GradeYear b)
        {
            int a1, b1;
            Normalize(a, b, out  a1, out b1);
            int c1 = a1 - b1;
            return GradeYear.ToGradeYear(c1 < 0 ? 0 : c1);
        }

        public static bool operator >(GradeYear a, int b)
        {
            return a > GradeYear.ToGradeYear(b);
        }

        public static bool operator >(GradeYear a, GradeYear b)
        {
            int a1, b1;
            Normalize(a, b, out  a1, out b1);
            return a1 > b1;
        }

        public static bool operator <(GradeYear a, int b)
        {
            return a < GradeYear.ToGradeYear(b);
        }

        public static bool operator <(GradeYear a, GradeYear b)
        {
            int a1, b1;
            Normalize(a, b, out  a1, out b1);
            return a1 < b1;
        }

        public static bool operator ==(GradeYear a, int b)
        {
            return a == GradeYear.ToGradeYear(b);
        }

        public static bool operator ==(GradeYear a, GradeYear b)
        {
            int a1, b1;
            Normalize(a, b, out  a1, out b1);
            return a1 == b1;
        }

        public static bool operator !=(GradeYear a, int b)
        {
            return a != GradeYear.ToGradeYear(b);
        }

        public static bool operator !=(GradeYear a, GradeYear b)
        {
            int a1, b1;
            Normalize(a, b, out  a1, out b1);
            return a1 != b1;
        }

        private static void Normalize(GradeYear a, GradeYear b, out int a1, out int b1)
        {
            a1 = a.Number < 0 ? int.MinValue : a.Number;
            b1 = b.Number < 0 ? int.MinValue : b.Number;
        }

        public override bool Equals(object obj)
        {
            return Number.Equals(((GradeYear)obj).Number);
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }

        #region Static Methods
        /// <summary>
        /// 取得或設定系統中預設的年級個數。
        /// </summary>
        public static GradeYear[] Defaults
        {
            get { return _defaults; }
            set { _defaults = value; }
        }

        /// <summary>
        /// 取得未分年級。
        /// </summary>
        public static GradeYear Undefined { get { return new GradeYear(int.MinValue); } }

        /// <summary>
        /// 取得中文的年級標題，小於0全都視為「未分年級」。
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ToString(int? number)
        {
            if (!number.HasValue || number < 0)
                return Undefined.ToString();

            return new GradeYear(number.Value).ToString();
        }

        /// <summary>
        /// 將數字轉換為 GradeYear 結構，小於0全都視為「未分年級」。
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static GradeYear ToGradeYear(int? number)
        {
            if (!number.HasValue || number < 0)
                return Undefined;
            else
                return new GradeYear(number.Value);
        }

        /// <summary>
        /// 將數字轉換為 GradeYear 結構，空字串、不合法字串、小於0全都視為「未分年級」。
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static GradeYear ToGradeYear(string number)
        {
            int num;
            if (int.TryParse(number, out num))
                return ToGradeYear(num);
            else
                return ToGradeYear(-1);
        }

        /// <summary>
        /// 將一系列數字轉換為 GradeYear 結構，小於0全都視為「未分年級」。
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static GradeYear[] ToGradeYears(params int[] numbers)
        {
            List<GradeYear> gradeyears = new List<GradeYear>();

            foreach (int number in numbers)
                gradeyears.Add(GradeYear.ToGradeYear(number));

            return gradeyears.ToArray();
        }
        #endregion
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class GradeYear_Extens
    {
        /// <summary>
        /// 設定每一個 SemesterData 的 ToStringFormatProvider 屬性。
        /// </summary>
        /// <param name="gradeYears"></param>
        /// <param name="formatter"></param>
        public static void SetToStringFormatter(this IEnumerable<GradeYear> gradeYears, Func<GradeYear, string> formatter)
        {
            List<GradeYear> list = new List<GradeYear>(gradeYears);

            for (int i = 0; i < list.Count; i++)
            {
                GradeYear gy = list[i];
                gy.ToStringFormatter = formatter;
            }
        }
    }
}
