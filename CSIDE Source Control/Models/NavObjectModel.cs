using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using CSIDESourceControl.Helpers;

namespace CSIDESourceControl.Models
{
    public enum ObjectSection { Unknown, Object, ObjectProperties, Properties, Fields, Keys, FieldGroups, Code };

    public class NavObjectModel : INotifyPropertyChanged
    {
        private bool _isEdited;
        public string InternalId
        {
            get
            {
                if (string.IsNullOrEmpty(this.Type))
                    throw new Exception("'Type' cannot be null or empty.");

                return string.Format("{0}-{1}", this.Type.ToUpper(), this.Id);
            }
        }

        public string Type { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateTime
        {
            get
            {
                if ((!string.IsNullOrEmpty(this.StringDate)) && (!string.IsNullOrEmpty(this.StringTime)))
                {
                    string stringDateTime = string.Format("{0} {1}", this.StringDate, this.StringTime);
                    return ObjectHelper.GetDateTime(stringDateTime);
                }

                return DateTime.MinValue;
            }
        }
        public bool Modified { get; set; }
        public string VersionList { get; set; }
        public string StringTime { get; set; }
        public string StringDate { get; set; }

        public int NoOfLines { get { return _objectLines.Count;  } }

        public string GetLine(int index)
        {
            if (_objectLines.Count > index)
                return _objectLines[index];

            return string.Empty;
        }

        private List<string> _objectLines = new List<string>();
        public List<string> ObjectLines { get { return _objectLines; } set { _objectLines = value; } }

        private List<string> _objectProperties = new List<string>();
        public List<string> ObjectProperties { get { return _objectProperties; } set { _objectProperties = value; } }

        private List<string> _properties = new List<string>();
        public List<string> Properties { get { return _properties; } set { _properties = value; } }

        private List<string> _fields = new List<string>();
        public List<string> Fields { get { return _fields; } set { _fields = value; } }

        private List<string> _keys = new List<string>();
        public List<string> Keys { get { return _keys; } set { _keys = value; } }

        private List<string> _fieldGroups = new List<string>();
        public List<string> FieldGroups { get { return _fieldGroups; } set { _fieldGroups = value; } }

        private List<string> _code = new List<string>();
        public List<string> Code { get { return _code; } set { _code = value; } }

        public bool IsEdited
        {
            get { return _isEdited; }
            set { _isEdited = value; OnPropertyChange("IsEdited"); }
        }

        public bool IsExisting(NavObjectModel objectToCompare)
        {
            if (objectToCompare == null)
                return false;

            return true;
        }

        public bool IsEqualTo(NavObjectModel objectToCompare)
        {
            if (objectToCompare == null)
                return false;

            if (this.ObjectLines.Count != objectToCompare.ObjectLines.Count)
                return false;

            for (int i = 0; i < this.ObjectLines.Count; i++)
            {
                string lineA = GetLine(i);
                string lineB = objectToCompare.GetLine(i);

                if (!lineA.Equals(lineB))
                    return false;
            }

            return true;
        }

        public bool IsObjectPropertiesEqual(NavObjectModel objectToCompare)
        {
            if (objectToCompare == null)
                return false;

            if (this.ObjectProperties.Count != objectToCompare.ObjectProperties.Count)
                return false;

            for (int i = 0; i < this.ObjectProperties.Count; i++)
            {
                string lineA = this.ObjectProperties[i];
                string lineB = string.Empty;
                if (objectToCompare.ObjectProperties.Count > i)
                    lineB = objectToCompare.ObjectProperties[i];

                if (!lineA.Equals(lineB))
                    return false;
            }

            return true;
        }

        public bool IsPropertiesEqual(NavObjectModel objectToCompare)
        {
            if (objectToCompare == null)
                return false;

            if (this.Properties.Count != objectToCompare.Properties.Count)
                return false;

            for (int i = 0; i < this.Properties.Count; i++)
            {
                string lineA = this.Properties[i];
                string lineB = string.Empty;
                if (objectToCompare.Properties.Count > i)
                    lineB = objectToCompare.Properties[i];

                if (!lineA.Equals(lineB))
                    return false;
            }

            return true;
        }

        public bool IsCodeEqual(NavObjectModel objectToCompare)
        {
            if (objectToCompare == null)
                return false;

            if (this.Code.Count != objectToCompare.Code.Count)
                return false;

            for (int i = 0; i < this.Code.Count; i++)
            {
                string lineA = this.Code[i];
                string lineB = string.Empty;
                if (objectToCompare.Code.Count > i)
                    lineB = objectToCompare.Code[i];

                if (!lineA.Equals(lineB))
                    return false;
            }

            return true;
        }

        public static bool operator ==(NavObjectModel a, NavObjectModel b)
        {
            if (System.Object.ReferenceEquals(a, b))
                return true;

            if ((a is null) || (b is null))
                return false;

            return a.InternalId == b.InternalId;
        }

        public static bool operator !=(NavObjectModel a, NavObjectModel b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region Serialize
        public void Serialize(ref BinaryWriter writer)
        {
            WriteGenericList(ObjectLines, ref writer);
            WriteGenericList(ObjectProperties, ref writer);
            WriteGenericList(Properties, ref writer);
            WriteGenericList(Fields, ref writer);
            WriteGenericList(Keys, ref writer);
            WriteGenericList(FieldGroups, ref writer);
            WriteGenericList(Code, ref writer);

            writer.Write(Type);
            writer.Write(Id);
            writer.Write(Name);
            writer.Write(Modified);
            writer.Write(VersionList);
            writer.Write(StringTime);
            writer.Write(StringDate);
        }

        public static string FilePathToInternalId(string filePath)
        {
            string internalId = string.Empty;

            string[] fileSplit = filePath.Split('/');

            foreach (string filename in fileSplit)
            {
                if (filename.Contains(".txt"))
                {
                    internalId = filename.Replace(".txt", string.Empty).ToUpper();
                }
            }

            return internalId;
        }

        public static string PathToType(string filePath)
        {
            string[] fileSplit = filePath.Split('/');

            if (fileSplit.Length == 2)
            {
                if (string.IsNullOrEmpty(fileSplit[1]))
                    return fileSplit[0].ToUpper();
            }

            return string.Empty;
        }


        private void WriteGenericList(List<string> list, ref BinaryWriter writer)
        {
            writer.Write(list.Count);
            foreach (string line in list)
            {
                writer.Write(line);
            }
        }

        public static NavObjectModel Desserialize(ref BinaryReader reader)
        {
            return new NavObjectModel()
            {
                ObjectLines = ReadGenericList(ref reader),
                ObjectProperties = ReadGenericList(ref reader),
                Properties = ReadGenericList(ref reader),
                Fields = ReadGenericList(ref reader),
                Keys = ReadGenericList(ref reader),
                FieldGroups = ReadGenericList(ref reader),
                Code = ReadGenericList(ref reader),

                Type = reader.ReadString(),
                Id = reader.ReadInt32(),
                Name = reader.ReadString(),
                Modified = reader.ReadBoolean(),
                VersionList = reader.ReadString(),
                StringTime = reader.ReadString(),
                StringDate = reader.ReadString()
            };
        }

        private static List<string> ReadGenericList(ref BinaryReader reader)
        {
            int count = reader.ReadInt32();
            var list = new List<string>(count);
            for (int i = 0; i < count; i++)
            {
                var line = reader.ReadString();
                list.Add(line);
            }
            return list;
        }

        public string GetFullPath(string path)
        {
            return string.Format(@"{0}\{1}\{2}.txt", path, Type, InternalId);
        }

        public string GetDirectoryPath(string path)
        {
            return string.Format(@"{0}\{1}", path, Type);
        }

        #endregion Serialize

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

