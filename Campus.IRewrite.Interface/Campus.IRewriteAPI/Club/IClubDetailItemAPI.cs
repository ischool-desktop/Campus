using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Campus.IRewrite.Interface
{
    /// <summary>
    /// 社團系統 - 社團基本資料
    /// </summary>
    public interface IClubDetailItemAPI
    {
        //社團基本資料項目
        FISCA.Presentation.IDetailBulider CreateBasicInfo();
    }
}
