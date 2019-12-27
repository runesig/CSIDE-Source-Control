using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIDESourceControl.Helpers
{
    public static class GitHelper
    {
        public static void CreateGitFiles(string folder)
        {
            if(!File.Exists(GetFilePath(folder, ".gitignore")))
                File.WriteAllBytes(GetFilePath(folder, ".gitignore"), Resources.gitignore);

            if (!File.Exists(GetFilePath(folder, ".gitattributes")))
                File.WriteAllBytes(GetFilePath(folder, ".gitattributes"), Resources.gitattributes);

            CreateReadMeFile(folder);
        }
        public static void CreateReadMeFile(string folder)
        {
            string filePath = GetFilePath(folder, "README.md");

            if (File.Exists(filePath))
                return;

           var dirName = new DirectoryInfo(folder).Name;

            using (FileStream fileStream = File.Create(filePath, 1024))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(string.Format("# {0}", dirName));
                fileStream.Write(info, 0, info.Length);
            }
        }
        private static string GetFilePath(string folder, string fileName)
        {
            return string.Format(@"{0}\{1}", folder, fileName);
        }

    }
}
