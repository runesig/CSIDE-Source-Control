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
            string filePath = GetFilePath();

            if (!File.Exists(filePath))
                return new ServerSetupModel();

            using (StreamReader streamReader = new StreamReader(filePath))
            {
                return GetFirstInstance<ServerSetupModel>(CSideEnvironmentSetting, streamReader);
            }
        }

        public ExportFilterModel ReadFilterSettings()
        {
            string filePath = GetFilePath();

            if (!File.Exists(filePath))
                return null;

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
            string settingsString = JsonConvert.SerializeObject(new
            {
                cside_environment = JsonConvert.SerializeObject(serverSetupModel),
                filter = JsonConvert.SerializeObject(exportFilterModel),
            });

            File.WriteAllText(GetFilePath2(), settingsString);
        }

        private string GetFilePath()
        {
            return string.Format(@"{0}\{1}\{2}", Folder, SettingsSubFolder, SettingsFilename);
;       }

        private string GetFilePath2()
        {
            return string.Format(@"{0}\{1}\{2}", Folder, SettingsSubFolder, "Settings2.json");
        }
    }
}

