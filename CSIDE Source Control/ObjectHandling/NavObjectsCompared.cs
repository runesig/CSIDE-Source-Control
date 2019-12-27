using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIDESourceControl.ObjectHandling
{
    public class NavObjectsCompared
    {
        public enum EqualStatus { Equal = 0, Unequal = 1, Unexisting = 2 }

        public NavObjectsCompared(string internalId)
        {
            InternalId = internalId;
            Selected = false;
            Id = 0;
            Type = string.Empty;
            Name = string.Empty;
            StringDateCurrent = string.Empty;
            StringDateNew = string.Empty;
            StringTimeCurrent = string.Empty;
            StringTimeNew = string.Empty;
            VersionListCurrent = string.Empty;
            VersionListNew = string.Empty;
            NoOfLinesCurrent = 0;
            NoOfLinesNew = 0;
            Status = EqualStatus.Equal;
            ObjectPropertiesEqual = true;
            CodeEqual = true;
            Finished = false;
            Edited = false;
            Comment = string.Empty;
        }

        public string InternalId { get; private set; }
        public bool Selected { get; set; }
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string StringDateCurrent { get; set; }
        public string StringDateNew { get; set; }
        public string StringTimeCurrent { get; set; }
        public string StringTimeNew { get; set; }
        public string VersionListCurrent { get; set; }
        public string VersionListNew { get; set; }
        public int NoOfLinesCurrent { get; set; }
        public int NoOfLinesNew { get; set; }
        public EqualStatus Status { get; set; }
        public bool ObjectPropertiesEqual { get; set; }
        public bool CodeEqual { get; set; }
        public bool Finished { get; set; }
        public bool Edited { get; set; }
        public string Comment { get; set; }

        public bool ConsideredEqual
        {
            get
            {
                if (Status == EqualStatus.Equal)
                    return true;

                if ((Status == EqualStatus.Unequal) && (ObjectPropertiesEqual == false) && (CodeEqual == true))
                    return true;

                return false;
            }
        }
    }
}

