using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIDESourceControl.Models
{
    public class ImportSettings
    {
        public bool Modified { get; set; }
        public string Filter { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string VersionList { get; set; }
        public bool CustomFilter { get; set; }
    }
}
