using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UXF;
using Random = UnityEngine.Random;

namespace Core
{
    public class SessionManager : MonoBehaviour
    {
        [SerializeField] private GameObject trialsParent;
        [SerializeField] private float promptTime;
        [SerializeField] private TextMeshPro promptText;

        private ITrial[] customTrials;
        private List<ITrial> trialSequence;
        
        private Dictionary<ITrial, UXFDataTable> trialData;
        private ITrial currentTrial;
        [SerializeField] [ReadOnly] private int numTrials;
        private int trialCount;

        public void OnValidate()
        {
            customTrials = trialsParent.GetComponentsInChildren<ITrial>(true);
        }

        public void BeginSession(Session session)
        {
            trialCount = 0;
            trialData = new Dictionary<ITrial, UXFDataTable>();

            foreach (var trial in customTrials)
            {
                trial.LoadSettingsFromJson();
            }
            
            GenerateTrialSequence();
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
            currentTrial = trialSequence[trialCount];
            
            ShowPrompt();
            yield return new WaitForSeconds(promptTime);
            HidePrompt();
            
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

        private void ShowPrompt()
        {
            promptText.renderer.enabled = true;
            promptText.text = currentTrial.GetPromptText();
        }

        private void HidePrompt()
        {
            promptText.renderer.enabled = false;
        }

        public void EndSession()
        {
            return;
        }
        
        private void GenerateTrialSequence()
        {
            trialSequence = new List<ITrial>();
            
            foreach (var trialType in customTrials)
            {
                for (var i = 0; i < trialType.GetNumRepetitions(); i++)
                {
                    trialSequence.Add(trialType);
                }
            }
            numTrials = trialSequence.Count;
            trialSequence.Shuffle();
        }

        public void GenerateTemplateJson()
        {
            var sessionDict = new Dictionary<string, object>();
            
            foreach (var trial in customTrials)
            {
                sessionDict.Add(trial.GetTrialName(), trial.GetTemplateSettings());
            }

            var serializedJson = MiniJSON.Json.Serialize(sessionDict);
            File.WriteAllText("Assets/StreamingAssets/TEMPLATE.json", serializedJson);
        }
    }
}