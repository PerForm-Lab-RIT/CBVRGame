using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

namespace Core
{
    public class SessionManager : MonoBehaviour
    {
        [SerializeField] private GameObject trialsParent;
        [SerializeField] private int numTrials;

        private ITrial[] customTrials;
        private Dictionary<ITrial, UXFDataTable> trialData;
        private ITrial currentTrial;
        private int trialCount;
    
        public void BeginSession(Session session)
        {
            trialCount = 0;
            trialData = new Dictionary<ITrial, UXFDataTable>();
            customTrials = trialsParent.GetComponentsInChildren<ITrial>(true);
            var block = session.CreateBlock();
            block.CreateTrial();
            session.BeginNextTrial();
        }

        // Called from UXF Event
        public void BeginTrial(Trial uxfTrial)
        {
            StartCoroutine(TrialRoutine(uxfTrial));
        }
    
        private IEnumerator TrialRoutine(Trial uxfTrial)
        {
            currentTrial = customTrials[Random.Range(0, customTrials.Length)];
            yield return currentTrial.Perform();
            trialCount++;
            uxfTrial.End();
        }

        // Called from UXF Event
        public void EndTrial(Trial uxfTrial)
        {
            var rowToAdd = currentTrial.RetrieveTrialData();
            if (!trialData.ContainsKey(currentTrial))
                trialData[currentTrial] = new UXFDataTable(1, currentTrial.GetColumnNames());

            trialData[currentTrial].AddCompleteRow(rowToAdd);

            if (trialCount < numTrials)
            {
                Session.instance.CurrentBlock.CreateTrial();
                Session.instance.BeginNextTrial();
            }
            else
            {
                foreach(var data in trialData)
                {
                    Session.instance.SaveDataTable(data.Value, data.Key.GetTrialName(), UXFDataType.TrialResults);
                }
                Session.instance.End();
            }
        }

        public void EndSession()
        {
            return;
        }
    }
}