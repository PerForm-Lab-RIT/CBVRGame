using System.Collections;
using Core;
using UnityEngine;
using UXF;

namespace TrialTypes
{
    public class AppearanceTrial : MonoBehaviour, ITrial
    {
        private const string TrialName = "Appearance";

        public IEnumerator Perform()
        {
            throw new System.NotImplementedException();
        }

        public string GetTrialName()
        {
            return gameObject.name;
        }

        public string[] GetColumnNames()
        {
            throw new System.NotImplementedException();
        }

        public UXFDataRow RetrieveTrialData()
        {
            throw new System.NotImplementedException();
        }
    }
}
