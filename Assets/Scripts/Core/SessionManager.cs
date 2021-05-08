using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PupilLabs;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UXF;
using Random = UnityEngine.Random;

namespace Core
{
    public class SessionManager : MonoBehaviour
    {
        [SerializeField] private GameObject trialsParent;
        [SerializeField] private float promptTime;
        [SerializeField] private TextMeshPro promptText;
        [SerializeField] private SessionSettings sessionSettings;
        [SerializeField] private GameObject pauseUI;
        [SerializeField] private Text infoText;
        [SerializeField] private CalibrationController calibrationController;
        [SerializeField] private SelectEyeTracker eyeTracker;

        private ITrial[] customTrials;
        private List<ITrial> trialSequence;
        
        private Dictionary<ITrial, UXFDataTable> trialData;
        private ITrial currentTrial;
        [SerializeField] [ReadOnly] private int numTrials;
        private int trialCount;
        private bool pauseBuffered;

        public void OnValidate()
        {
            customTrials = trialsParent.GetComponentsInChildren<ITrial>(true);
        }

        public void BeginSession(Session session)
        {
            trialCount = 0;
            trialData = new Dictionary<ITrial, UXFDataTable>();
            
            Random.InitState(DateTime.Now.Millisecond);
            sessionSettings.LoadFromUxfJson();
            pauseBuffered = true;

            foreach (var trial in customTrials)
            {
                trial.LoadSettingsFromJson();
            }
            
            eyeTracker.SelectFromUxf();
            
            HidePrompt();
            GenerateTrialSequence();
            var block = session.CreateBlock();
            block.CreateTrial();
            session.BeginNextTrial();
        }

        // Called from UXF Event
        public void BeginTrial(Trial uxfTrial)
        {
            if (pauseBuffered)
            {
                pauseUI.SetActive(true);
                pauseBuffered = false;
            }
            else
            {
                StartCoroutine(TrialRoutine(uxfTrial));
            }
        }

        public void ResumeTrial()
        {
            pauseBuffered = false;
            StartCoroutine(TrialRoutine(Session.instance.CurrentTrial));
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
            
            sessionDict.Add("SessionSettings", sessionSettings.GetSettingsDict());
            foreach (var trial in customTrials)
            {
                sessionDict.Add(trial.GetTrialName(), trial.GetTemplateSettings());
            }

            var serializedJson = MiniJSON.Json.Serialize(sessionDict);
            File.WriteAllText("Assets/StreamingAssets/TEMPLATE.json", serializedJson);
        }

        public void BufferPause()
        {
            pauseBuffered = true;
        }
        
        public void CalibratePupilLabs()
        {
            if (calibrationController.subsCtrl.IsConnected)
            {
                calibrationController.StartCalibration();
                calibrationController.OnCalibrationSucceeded += CalibrationSuccessful;
                calibrationController.OnCalibrationFailed += CalibrationFailed;
                pauseUI.SetActive(false);
                infoText.gameObject.SetActive(false);
            }
            else
            {
                infoText.text =
                    "PupilLabs tracker disconnected!\n If the tracker was selected in the JSON settings, ensure Pupil Capture is running and try again.";
                infoText.color = Color.red;
            }
        }

        private void CalibrationSuccessful()
        {
            pauseUI.SetActive(true);
            infoText.gameObject.SetActive(true);
            infoText.text = "Calibration successful!";
            infoText.color = Color.green;
        }
    
        private void CalibrationFailed()
        {
            pauseUI.SetActive(true);
            infoText.gameObject.SetActive(true);
            infoText.text = "Calibration failed.\n Ensure that Pupil Capture is running with both eye cameras!";
            infoText.color = Color.red;
        }
    }
}