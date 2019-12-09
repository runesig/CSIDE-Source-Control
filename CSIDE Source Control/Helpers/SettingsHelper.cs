using System;
using System.Collections.Generic;
using System.IO;
using CSIDESourceControl.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CSIDESourceControl.Helpers
{
    public class SettingsHelper
    {
        public static string SettingsFilename = "Settings.json";
        public static string SettingsSubFolder = ".csc";
        public static string CSideEnvironmentSetting = "cside_environment";
        public static string FilterSetting = "filter";

        public string Folder { get; }

        public SettingsHelper(string folder)
        {
            Folder = folder;
        }

        public ServerSetupModel ReadServerSettings()
        {
            string filePath = GetSettingsFilePath();

            if (!File.Exists(filePath))
                return new ServerSetupModel();

            using (StreamReader streamReader = new StreamReader(filePath))
            {
                return GetFirstInstance<ServerSetupModel>(CSideEnvironmentSetting, streamReader);
            }
        }

        public ExportFilterModel ReadFilterSettings()
        {
            string filePath = GetSettingsFilePath();

            if (!File.Exists(filePath))
                return new ExportFilterModel();

            using (StreamReader streamReader = new StreamReader(filePath))
            {
                return GetFirstInstance<ExportFilterModel>(FilterSetting, streamReader);
            }
        }

        public T GetFirstInstance<T>(string propertyName, StreamReader streamReader)
        {
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonToken.PropertyName
                        && (string)jsonReader.Value == propertyName)
                    {
                        jsonReader.Read();

                        var serializer = new JsonSerializer();
                        return serializer.Deserialize<T>(jsonReader);
                    }
                }

                return default(T);
            }
        }

        public void SerializeToSettingsFile(ServerSetupModel serverSetupModel, ExportFilterModel exportFilterModel)
        {
            if (string.IsNullOrEmpty(Folder) || string.IsNullOrEmpty(SettingsSubFolder))
                return;

            DirectoryInfo di = Directory.CreateDirectory(GetSettingsFolder());
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            SettingsModel model = new SettingsModel();
            model.ServerSetupModel = serverSetupModel;
            model.ExportFilterModel = exportFilterModel;

            string settingsString = JsonConvert.SerializeObject(model);

            File.WriteAllText(GetSettingsFilePath(), settingsString);
        }

        public bool SettingsFolderExists()
        {
            return Directory.Exists(GetSettingsFolder());
        }

        private string GetSettingsFilePath()
        {
            return string.Format(@"{0}\{1}", GetSettingsFolder(), SettingsFilename);
;       }

        private string GetSettingsFolder()
        {
            return string.Format(@"{0}\{1}", Folder, SettingsSubFolder);
        }
    }
}

