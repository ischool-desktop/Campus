using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Campus.Configuration
{
    /// <summary>
    /// 代表系統提供的所有組態資料。
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// 取得使用者的個人組態資料，每個使用者都不同。
        /// </summary>
        public static UserConfigManager User { get; private set; }

        /// <summary>
        /// 取得應用程式的組態資料，每個資料庫不同。
        /// </summary>
        public static ConfigurationManager App { get; private set; }

        /// <summary>
        /// 取的全域的組態資料，全世界都一樣。
        /// </summary>
        public static ConfigurationManager Global { get; private set; }

        public static void Initialize(UserConfigManager user, ConfigurationManager app, ConfigurationManager global)
        {
            User = user;
            App = app;
            Global = global;
        }
    }
}
