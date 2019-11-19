using CSIDESourceControl.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIDESourceControl.ObjectHandling
{
    public class ObjectsExport
    {
        public static void ExportObjects(List<NavObject> objects, string filePath)
        {
            foreach (NavObject navObject in objects)
            {
                CreateDirectoryIfNotExists(filePath, navObject);

                string fullPath = GetFullPath(filePath, navObject);

                using (StreamWriter textObject = new StreamWriter(fullPath, false, Encoding.Default))
                {
                    foreach (string line in navObject.ObjectLines)
                    {
                        textObject.WriteLine(line);
                    }
                }
            }
        }

        private static void CreateDirectoryIfNotExists(string filePath, NavObject navObject)
        {
            string folder = GetDirectoryPath(filePath, navObject);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }

        private static string GetDirectoryPath(string filePath, NavObject navObject)
        {
            return string.Format(@"{0}\{1}", filePath, navObject.Type);
        }

        private static string GetFullPath(string filePath, NavObject navObject)
        {
            return string.Format(@"{0}\{1}\{2}.txt", filePath, navObject.Type, navObject.InternalId);
        }
    }
}
