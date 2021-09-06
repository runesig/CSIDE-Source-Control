using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIDESourceControl.Models
{
    public class GitSettingsModel
    {
        public GitSettingsModel()
        {
            DefaultBranch = "master"; // master default value
        }

        [JsonProperty(PropertyName = "default_branch")]
        public string DefaultBranch { get; set; }
    }
}
