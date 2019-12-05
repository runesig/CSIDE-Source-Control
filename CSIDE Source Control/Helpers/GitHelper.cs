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

            if (!File.Exists(GetFilePath(folder, "README.md")))
                File.WriteAllBytes(GetFilePath(folder, "README.md"), Resources.README);
        }

        private static string GetFilePath(string folder, string fileName)
        {
            return string.Format(@"{0}\{1}", folder, fileName);
        }
    }
}
