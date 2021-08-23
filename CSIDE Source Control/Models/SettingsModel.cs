using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIDESourceControl.Models
{
    public class SettingsModel
    {
        [JsonProperty(PropertyName = "cside_environment")]
        public ServerSetupModel ServerSetupModel { get; set; }

        [JsonProperty(PropertyName = "filter")]
        public ExportFilterModel ExportFilterModel { get; set; }

        [JsonProperty(PropertyName = "git")]
        public GitSettingsModel GitSettingsModel { get; set; }
    }
}
