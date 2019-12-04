using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSIDESourceControl.ExportFinexe;
using CSIDESourceControl.Models;


namespace CSIDESourceControl.Client.Helpers
{
    public class ExportFinexeHelper
    {
        public event ExportErrorEventHandling OnExportError;

        async public Task<ExportResult> ExportObjectsFromFinExe(ServerSetupModel serverSetup, ExportFilterModel exportFilter)
        {
            var result = await Task.Factory.StartNew(() =>
            {
                ExportFinexeHandling fileHandeling = new ExportFinexeHandling()
                {
                    FinsqlPath = serverSetup.FinExePath,
                    ServerName = serverSetup.Server,
                    Database = serverSetup.Database,
                    NTAuthentication = serverSetup.UseNTAuthentication
                };
                fileHandeling.OnExportError += FileHandeling_OnExportError;

                if (!serverSetup.UseNTAuthentication)
                {
                    fileHandeling.Username = serverSetup.UserName;
                    fileHandeling.Password = serverSetup.Password;
                }

                fileHandeling.Filter = ExportFilter.CreateFilterString(exportFilter);

                if (!fileHandeling.ExportObjects(out string exportedObjectsPath, out string message))
                {
                    return new ExportResult { Success = false, ExportedObjectsPath = exportedObjectsPath, Message = message };
                }

                return new ExportResult { Success = true, ExportedObjectsPath = exportedObjectsPath, Message = message };
            });

            return result;
        }

        private void FileHandeling_OnExportError(object source, ExportErrorEventArgs e)
        {
            OnExportError?.Invoke(source, e);
        }
    }
}
