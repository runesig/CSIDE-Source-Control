using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIDESourceControl.ExportFinexe
{
    public class ExportResult
    {
        public bool Success { get; set; }
        public string ExportedObjectsPath { get; set; }
        public string Message { get; set; }
    }
}
