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
        public static void ExportObjectsToSeparateFolders(List<NavObjectModel> objects, string filePath)
        {
            foreach (NavObjectModel navObject in objects)
            {
                CreateDirectoryIfNotExists(filePath, navObject);

                string fullPath = navObject.GetFullPath(filePath);

                using (StreamWriter textObject = new StreamWriter(fullPath, false, Encoding.Default))
                {
                    foreach (string line in navObject.ObjectLines)
                    {
                        textObject.WriteLine(line);
                    }
                }
            }
        }

        private static void CreateDirectoryIfNotExists(string filePath, NavObjectModel navObject)
        {
            string folder = navObject.GetDirectoryPath(filePath);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }
    }
}
