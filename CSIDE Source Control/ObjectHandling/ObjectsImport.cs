using CSIDESourceControl.Helpers;
using CSIDESourceControl.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIDESourceControl.ObjectHandling
{
    public class ObjectsImport
    {
        private readonly Dictionary<string, NavObjectModel> _navObjects;
        public ObjectsImport()
        {
            _navObjects = new Dictionary<string, NavObjectModel>();
        }

        public Dictionary<string, NavObjectModel> NavObjects { get { return _navObjects;  } }

        public void RunImport(string _filePath)
        {
            if (string.IsNullOrEmpty(_filePath))
                return;

            if (!File.Exists(_filePath))
                return;

            ObjectSection currObjectSection = ObjectSection.Unknown;
            NavObjectModel currNavObject = null;

            var lines = File.ReadAllLines(_filePath, Encoding.Default);

            int totalLineCount = lines.Length;
            for (int i = 0; i < totalLineCount; i++)
            {
                ObjectSection objectSection = ObjectHelper.FindObjectSection(lines[i]);
                if (objectSection != ObjectSection.Unknown)
                    currObjectSection = objectSection;

                ProcessLine(lines[i], currObjectSection, ref currNavObject);

                // FireFileReadEvent(i, totalLineCount);
            }
        }

        private void ProcessLine(string line, ObjectSection objectSection, ref NavObjectModel navObject)
        {
            switch (objectSection)
            {
                case ObjectSection.Object:
                    navObject = CreateNewObject(line, objectSection, navObject);
                    break;
                case ObjectSection.ObjectProperties:
                    SetObjectProperties(line, objectSection, ref navObject);
                    navObject.ObjectProperties.Add(line);
                    break;
                default:
                    navObject.Code.Add(line);
                    break;
                    //case ObjectSection.Properties:
                    //    navObject.Properties.Add(line);
                    //    break;
                    //case ObjectSection.Fields:
                    //    navObject.Fields.Add(line);
                    //    break;
                    //case ObjectSection.Keys:
                    //    navObject.Keys.Add(line);
                    //    break;
                    //case ObjectSection.FieldGroups:
                    //    navObject.FieldGroups.Add(line);
                    //    break;
                    //case ObjectSection.Code:
                    //    navObject.Code.Add(line);
                    //    break;
            }

            navObject.ObjectLines.Add(line);
        }

        private NavObjectModel CreateNewObject(string line, ObjectSection objectSection, NavObjectModel navObject)
        {
            NavObjectModel newNavObject = NewObject(line, objectSection);
            if (newNavObject != null)
            {
                navObject = newNavObject;
                _navObjects.Add(newNavObject.InternalId, newNavObject);
            }

            return navObject;
        }

        private NavObjectModel NewObject(string line, ObjectSection objectSection)
        {
            string[] parts = line.Split(' ');

            if (objectSection != ObjectSection.Object)
                return null;

            if (parts.Length == 0)
                return null;

            if (parts[0] != "OBJECT")
                return null;

            return new NavObjectModel()
            {
                Type = parts[1],
                Id = ObjectHelper.GetInt(parts[2]),
                Name = ObjectHelper.GetObjectName(line)
            };
        }

        private void SetObjectProperties(string line, ObjectSection objectSection, ref NavObjectModel navObject)
        {
            if (objectSection != ObjectSection.ObjectProperties)
                return;

            string[] parts = line.Split('=');

            switch (ObjectHelper.RemoveIllChar(parts[0]))
            {
                case "Date":
                    navObject.StringDate = ObjectHelper.RemoveIllChar(parts[1]);
                    break;
                case "Time":
                    navObject.StringTime = ObjectHelper.RemoveIllChar(parts[1]);
                    break;
                case "Modified":
                    navObject.Modified = ObjectHelper.GetBool(parts[1]);
                    break;
                case "Version List":
                    navObject.VersionList = ObjectHelper.GetVersionList(line, parts[0]);
                    break;
            }
        }
    }
}
