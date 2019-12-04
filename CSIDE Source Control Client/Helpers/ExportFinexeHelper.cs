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
    }
}
