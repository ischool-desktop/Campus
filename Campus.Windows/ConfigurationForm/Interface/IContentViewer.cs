using System.Windows.Forms;

namespace Campus.Windows
{
    /// <summary>
    /// 內容檢視器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IContentViewer<T>
    {
        void Prepare();

        /// <summary>
        /// 指定內容
        /// </summary>
        T Content { set; }

        /// <summary>
        /// 顯示的UserControl
        /// </summary>
        UserControl Control { get;}
    }
}