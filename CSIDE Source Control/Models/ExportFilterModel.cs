using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIDESourceControl.Models
{
    public class ExportFilterModel
    {
        public ExportFilterModel()
        {
            Modified = true;
            DateFrom = null;
            DateTo = null;
            VersionList = string.Empty;
            UseCustomFilter = false;
            CustomFilter = string.Empty;
        }

        [JsonProperty(PropertyName = "modified")]
        public bool Modified { get; set; }

        [JsonProperty(PropertyName = "date_from")]
        public DateTime? DateFrom { get; set; }

        [JsonProperty(PropertyName = "date_to")]
        public DateTime? DateTo { get; set; }

        [JsonProperty(PropertyName = "version_list")]
        public string VersionList { get; set; }

        [JsonProperty(PropertyName = "use_custom_filter")]
        public bool UseCustomFilter { get; set; }

        [JsonProperty(PropertyName = "custom_filter")]
        public string CustomFilter { get; set; }
    }
}
