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

        public string Folder { get; }

        public SettingsHelper(string folder)
        {
            Folder = folder;
        }

        public ServerSetupModel ReadServerSettings()
        {
            string filePath = GetFilePath();

            using (StreamReader streamReader = new StreamReader(filePath))
            {
                return GetFirstInstance<ServerSetupModel>("cside_environment", streamReader);
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

        private string GetFilePath()
        {
            return string.Format(@"{0}\{1}\{2}", Folder, SettingsSubFolder, SettingsFilename);
;       }
    }
}

