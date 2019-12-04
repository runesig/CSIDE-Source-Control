using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIDESourceControl.Helpers
{
    public static class RecentDestinationFolder
    {
        private static string RecentFolderKey = "RecentFolder";

        public static void Save(Configuration config, string folder)
        {
            if (config.AppSettings.Settings[RecentFolderKey] != null)
                config.AppSettings.Settings[RecentFolderKey].Value = folder;
            else
                config.AppSettings.Settings.Add(RecentFolderKey, folder);

            config.Save(ConfigurationSaveMode.Minimal);
        }

        public static string Read(Configuration config)
        {
            if (config.AppSettings.Settings[RecentFolderKey] != null)
                return config.AppSettings.Settings[RecentFolderKey].Value;

            return string.Empty;
        }
    }
}
