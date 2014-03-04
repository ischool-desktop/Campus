using System;
using System.Collections.Generic;
using System.Text;

namespace Campus.Configuration
{
    public class UserConfigManager : ConfigurationManager
    {
        public string AccountName { get; private set; }

        public UserConfigManager(IConfigurationProvider provider, string accountName)
            : base(provider)
        {
            AccountName = accountName.ToLower();
        }

        protected override string NamespacePreprocess(string configNamespace)
        {
            return string.Format("{0}::{1}", AccountName, configNamespace);
        }
    }

}
