using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIDESourceControl.Models
{
    public class ServerSetupModel
    {
        public ServerSetupModel()
        {
            FinExePath = string.Empty;
            Server = string.Empty;
            Database = string.Empty;
            UseNTAuthentication = false;
            UserName = string.Empty;
            Password = string.Empty;
        }

        [JsonProperty(PropertyName = "finexe_path")]
        public string FinExePath { get; set; }

        [JsonProperty(PropertyName = "server")]
        public string Server { get; set; }

        [JsonProperty(PropertyName = "database")]
        public string Database { get; set; }

        [JsonProperty(PropertyName = "use_nt_authentication")]
        public bool UseNTAuthentication { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }
    }
}
