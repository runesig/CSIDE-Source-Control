using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIDESourceControl.Models
{
    public class ServerSetup
    {
        public string FinSQLPath { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        public bool UseNTAuthentication { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Filter { get; set; }
    }
}
