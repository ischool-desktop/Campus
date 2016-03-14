using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Aspose.Words;
using FISCA.Presentation.Controls;

namespace Campus.ePaper
{
    public class ConvertAspose
    {
        /// <summary>
        /// 支援傳送舊版Word格式至 Aspose Word V14
        /// </summary>
        /// <param name="MemStreamList"></param>
        /// <param name="name"></param>
        /// <param name="pf"></param>
        public static void Update_ePaperWordV14(List<MemoryStream> MemStreamList,string name,PrefixStudent pf)
        {
            try
            {
                List<Document> DocList = new List<Document>();
                foreach(MemoryStream ms in MemStreamList)
                {
                    Document doc = new Document(ms);
                    DocList.Add(doc);
                }

                Update_ePaper wp = new Update_ePaper(DocList, name, pf);
                if(wp.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                {
                    MsgBox.Show("電子報表已上傳!");
                }
                else
                {
                    MsgBox.Show("已取消!");
                }
            }
            catch (Exception ex)
            {
                MsgBox.Show("電子報表Word版本轉換或上傳失敗," + ex.Message);
            }

        }
    }
}
