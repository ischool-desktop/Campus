
namespace Campus.Windows
{
    /// <summary>
    /// 內容編輯器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IContentEditor<T>:IContentViewer<T>
    {
        /// <summary>
        /// 取得或設定內容
        /// </summary>
        T Content { get; set; }

        /// <summary>
        /// 驗證內容是否合法
        /// </summary>
        /// <returns></returns>
        bool Validate();
    }
}