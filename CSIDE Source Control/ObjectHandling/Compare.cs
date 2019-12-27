using CSIDESourceControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIDESourceControl.ObjectHandling
{
    public class Compare
    {
        private Dictionary<string, NavObjectModel> _currentNavObjects;
        private Dictionary<string, NavObjectModel> _newNavObjects;

        public Dictionary<string, NavObjectModel> CurrentNavObjects { get { return _currentNavObjects; } }
        public Dictionary<string, NavObjectModel> NewNavObjects { get { return _newNavObjects; } }

        private Dictionary<string, NavObjectsCompared> _objectsComparedDict = new Dictionary<string, NavObjectsCompared>();

        private int _counter = 0;

        public Compare(Dictionary<string, NavObjectModel> currentNavObjects, Dictionary<string, NavObjectModel> newNavObjects)
        {
            _currentNavObjects = currentNavObjects;
            _newNavObjects = newNavObjects;
        }

        public void Run()
        {
            foreach (string internalId in _currentNavObjects.Keys)
            {
                FindDifferencesCurrent(internalId);

                _counter++;
                FireCompareEvent();
            }

            foreach (string internalId in _newNavObjects.Keys)
            {
                FindDifferencesNew(internalId);

                _counter++;
                FireCompareEvent();
            }
        }

        private void FindDifferencesCurrent(string internalId)
        {
            NavObjectModel navObjectCurrent = GetDictValue(_currentNavObjects, internalId);
            NavObjectModel navObjectNew = GetDictValue(_newNavObjects, internalId);

            NavObjectsCompared newObjectsCompared = new NavObjectsCompared(internalId)
            {
                Id = navObjectCurrent.Id,
                Type = navObjectCurrent.Type,
                Name = navObjectCurrent.Name,
                Edited = (navObjectCurrent == null ? false : navObjectCurrent.IsEdited) || (navObjectNew == null ? false : navObjectNew.IsEdited)
            };

            GetDifference(navObjectCurrent, navObjectNew, newObjectsCompared);

            SetCurrentValues(navObjectCurrent, newObjectsCompared);
            SetNewValues(navObjectNew, newObjectsCompared);

            AddObjectComparedToDictionary(internalId, newObjectsCompared);
        }

        public void FindDifferencesNew(string internalId)
        {
            NavObjectModel navObjectNew = GetDictValue(_newNavObjects, internalId);
            NavObjectModel navObjectCurrent = GetDictValue(_currentNavObjects, internalId);

            NavObjectsCompared newObjectsCompared = new NavObjectsCompared(internalId)
            {
                Id = navObjectNew.Id,
                Type = navObjectNew.Type,
                Name = navObjectNew.Name,
                Edited = (navObjectNew == null ? false : navObjectNew.IsEdited) || (navObjectCurrent == null ? false : navObjectCurrent.IsEdited)
            };

            GetDifference(navObjectNew, navObjectCurrent, newObjectsCompared);

            SetCurrentValues(navObjectCurrent, newObjectsCompared);
            SetNewValues(navObjectNew, newObjectsCompared);

            AddObjectComparedToDictionary(internalId, newObjectsCompared);
        }

        private static NavObjectModel GetDictValue(Dictionary<string, NavObjectModel> dict, string key)
        {
            if (!dict.TryGetValue(key, out NavObjectModel navObject))
                return null;

            return navObject;
        }
        #region Private Methods

        private void AddObjectComparedToDictionary(string internalId, NavObjectsCompared newObjectsCompared)
        {
            if (!_objectsComparedDict.ContainsKey(internalId))
                _objectsComparedDict.Add(internalId, newObjectsCompared);
            else
            {
                if (_objectsComparedDict.TryGetValue(internalId, out NavObjectsCompared prevObjectsCompared))
                {
                    newObjectsCompared.Selected = prevObjectsCompared.Selected;
                }
                _objectsComparedDict[internalId] = newObjectsCompared;
            }
        }

        private void GetDifference(NavObjectModel navObject1, NavObjectModel navObject2, NavObjectsCompared objectsCompared)
        {

            if (!ObjectExists(navObject1, navObject2, ref objectsCompared))
                return;

            if (!ObjectIsEqual(navObject1, navObject2, ref objectsCompared))
                return;

            objectsCompared.CodeEqual = true;
            objectsCompared.ObjectPropertiesEqual = true;
            objectsCompared.Status = NavObjectsCompared.EqualStatus.Equal;
        }

        private bool ObjectExists(NavObjectModel navObject1, NavObjectModel navObject2, ref NavObjectsCompared objectsCompared)
        {
            if (!navObject1.IsExisting(navObject2))
            {
                objectsCompared.CodeEqual = false;
                objectsCompared.ObjectPropertiesEqual = false;
                objectsCompared.Status = NavObjectsCompared.EqualStatus.Unexisting;
                objectsCompared.Comment = "Object does not exists";

                return false;
            }

            return true;
        }

        private bool ObjectIsEqual(NavObjectModel navObject1, NavObjectModel navObject2, ref NavObjectsCompared objectsCompared)
        {
            string comment = string.Empty;
            objectsCompared.ObjectPropertiesEqual = true;
            objectsCompared.CodeEqual = true;

            if (!navObject1.IsEqualTo(navObject2))
            {
                objectsCompared.Status = NavObjectsCompared.EqualStatus.Unequal;

                // Do More Analysis
                if (!navObject1.IsObjectPropertiesEqual(navObject2))
                {
                    objectsCompared.ObjectPropertiesEqual = false;
                    AddToComment(ref comment, "Date, Time or Version");
                }

                if (!navObject1.IsCodeEqual(navObject2))
                {
                    objectsCompared.CodeEqual = false;
                    AddToComment(ref comment, "Code");
                }
                objectsCompared.Comment = comment;

                return false;
            }

            return true;
        }

        private void AddToComment(ref string comment, string value)
        {
            if (!string.IsNullOrEmpty(comment))
                comment = string.Format("{1}, {0}", comment, value);
            else
                comment = value;
        }


        private void FireCompareEvent()
        {
            //if (this.OnCompared != null)
            //{
            //    double percentageCompleted = 0;
            //    if (_totalObjectsToCompare != 0)
            //        percentageCompleted = (((double)_counter / (double)_totalObjectsToCompare) * 100);
            //    else
            //        percentageCompleted = 100;

            //    this.OnCompared((int)percentageCompleted);
            //}
        }

        private static void SetCurrentValues(NavObjectModel navObjectCurrent, NavObjectsCompared objectsCompared)
        {
            if (navObjectCurrent != null)
            {
                objectsCompared.StringDateCurrent = navObjectCurrent.StringDate;
                objectsCompared.StringTimeCurrent = navObjectCurrent.StringTime;
                objectsCompared.VersionListCurrent = navObjectCurrent.VersionList;
                objectsCompared.NoOfLinesCurrent = navObjectCurrent.ObjectLines.Count;
            }
        }

        private static void SetNewValues(NavObjectModel navObjectNew, NavObjectsCompared objectsCompared)
        {
            if (navObjectNew != null)
            {
                objectsCompared.StringDateNew = navObjectNew.StringDate;
                objectsCompared.StringTimeNew = navObjectNew.StringTime;
                objectsCompared.VersionListNew = navObjectNew.VersionList;
                objectsCompared.NoOfLinesNew = navObjectNew.ObjectLines.Count;
            }
        }

        #endregion Private Methods
    }
}
