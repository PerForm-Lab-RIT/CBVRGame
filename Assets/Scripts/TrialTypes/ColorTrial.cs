using System.Collections;
using Core;
using UnityEngine;
using UXF;

namespace TrialTypes
{
    public class ColorTrial : MonoBehaviour, ITrial
    {
        public IEnumerator Perform()
        {
            throw new System.NotImplementedException();
        }

        public UXFDataRow RetrieveTrialData()
        {
            throw new System.NotImplementedException();
        }

        public string GetTrialName()
        {
            throw new System.NotImplementedException();
        }

        public string[] GetColumnNames()
        {
            throw new System.NotImplementedException();
        }

        public string GetPromptText()
        {
            throw new System.NotImplementedException();
        }

        public IDictionary GetTemplateSettings()
        {
            throw new System.NotImplementedException();
        }

        public void LoadSettingsFromJson()
        {
            throw new System.NotImplementedException();
        }

        public int GetNumRepetitions()
        {
            throw new System.NotImplementedException();
        }
    }
}
