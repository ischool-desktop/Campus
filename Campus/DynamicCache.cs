using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Campus;

namespace Campus
{
    /// <summary>
    /// 代表一種 Value Object 的資料快取機制。
    /// </summary>
    public class DynamicCache : IEnumerable<XmlObject>
    {
        /// <summary>
        /// 每個 Value Object 的 Id 欄位名稱。
        /// </summary>
        public const string IdProperty = "Id";

        private Dictionary<string, XmlObject> Values = new Dictionary<string, XmlObject>();

        /// <summary>
        /// Property -> (ID -> IsOutOfDate)
        /// </summary>
        private Dictionary<string, Dictionary<string, bool>> ValueStatus = new Dictionary<string, Dictionary<string, bool>>();

        public DynamicCache()
        {
        }

        /// <summary>
        /// 取得目前已包含的屬性清單。
        /// </summary>
        public IEnumerable<string> CurrentProperties { get { return new List<string>(ValueStatus.Keys); } }

        /// <summary>
        /// 取得 Key 清單。
        /// </summary>
        public IEnumerable<string> PrimaryKeys
        {
            get { return new List<string>(Values.Keys); }
        }

        /// <summary>
        /// 依編號取得 Value Object。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public dynamic this[string id]
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (Contains(id))
                    return Values[id];
                else
                    return new XmlObject("Value");
            }
        }

        /// <summary>
        /// 是否包含了某 Value Object。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Contains(string id)
        {
            return Values.ContainsKey(id);
        }

        /// <summary>
        /// 清除所有快取資料。
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Clear()
        {
            Values.Clear();
            ValueStatus.Clear();
        }

        /// <summary>
        /// 將指定屬性設定為已經不是最新狀態。
        /// </summary>
        /// <param name="propertyNames"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetOutOfDate(IEnumerable<string> idList, params string[] propertyNames)
        {
            IEnumerable<string> updateSet = idList;
            if (updateSet == null)
                updateSet = new List<string>(Values.Keys);

            foreach (string name in propertyNames)
            {
                Dictionary<string, bool> statusList;
                if (ValueStatus.TryGetValue(name, out statusList))
                {
                    foreach (string id in updateSet)
                    {
                        if (statusList.ContainsKey(id))
                            statusList[id] = true;
                    }
                }
            }
        }

        /// <summary>
        /// 將指定屬性設定為已經不是最新狀態。
        /// </summary>
        /// <param name="propertyNames"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetOutOfDate(params string[] propertyNames)
        {
            SetOutOfDate(null, propertyNames);
        }

        /// <summary>
        /// 取的屬性資料是否在最新狀態。
        /// </summary>
        /// <param name="propertyNames"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool GetOutOfDate(IEnumerable<string> idList, params string[] propertyNames)
        {
            IEnumerable<string> confirmSet = idList;
            if (confirmSet == null)
                confirmSet = new List<string>(Values.Keys);

            bool propertyOutOfDate = false;
            foreach (string name in propertyNames)
            {
                Dictionary<string, bool> statusList;

                if (ValueStatus.TryGetValue(name, out statusList))
                {
                    bool valueObjectOutOfDate = false;
                    foreach (string id in confirmSet)
                    {
                        if (statusList.ContainsKey(id))
                            valueObjectOutOfDate |= statusList[id];
                        else
                        {
                            valueObjectOutOfDate = true;
                            break;
                        }
                    }
                    propertyOutOfDate |= valueObjectOutOfDate;

                    if (propertyOutOfDate)
                        break;
                }
                else
                {//只要有任一屬性不存在，就是全部 OutOfDate。
                    propertyOutOfDate = true;
                    break;
                }
            }

            return propertyOutOfDate;
        }

        /// <summary>
        /// 取的屬性資料是否在最新狀態。
        /// </summary>
        /// <param name="propertyNames"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool GetOutOfDate(params string[] propertyNames)
        {
            return GetOutOfDate(null, propertyNames);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Remove(params string[] idList)
        {
            Remove((IEnumerable<string>)idList);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Remove(IEnumerable<string> idList)
        {
            foreach (string id in idList)
            {
                if (Contains(id))
                    Values.Remove(id);

                foreach (Dictionary<string, bool> each in ValueStatus.Values)
                {
                    if (each.ContainsKey(id))
                        each.Remove(id);
                }
            }
        }

        /// <summary>
        /// 填入欄位資料。
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void FillProperty(string id, string propertyName, object value)
        {
            dynamic obj = GetOrCreateValue(id);

            obj[propertyName] = value;
            SetUpdateToDate(propertyName, id);
        }

        /// <summary>
        /// 填入欄位資料。
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public string GetProperty(string id, string propertyName)
        {
            if (Contains(id))
                return this[id][propertyName];
            else
                return string.Empty;
        }

        private void SetUpdateToDate(string propertyName, string id)
        {
            if (!ValueStatus.ContainsKey(propertyName))
                ValueStatus.Add(propertyName, new Dictionary<string, bool>());

            if (!ValueStatus[propertyName].ContainsKey(id))
                ValueStatus[propertyName].Add(id, false);

            ValueStatus[propertyName][id] = false;
        }

        private dynamic GetOrCreateValue(string id)
        {
            dynamic obj;
            if (!Contains(id))
            {
                obj = new XmlObject("Value");
                obj.Id = id;
                Values.Add(id, obj);
            }
            return Values[id];
        }

        #region IEnumerable<dynamic> 成員

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerator<XmlObject> GetEnumerator()
        {
            return new List<XmlObject>(Values.Values).GetEnumerator();
        }

        #endregion

        #region IEnumerable 成員

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
