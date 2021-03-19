using System.Collections;
using System.Globalization;
using Core;
using EyeTracker;
using UnityEngine;
using UXF;
using Valve.VR;
using Random = UnityEngine.Random;

namespace TrialTypes
{
    public class AppearanceTrial : MonoBehaviour, ITrial
    {
        [SerializeField] private SelectEyeTracker eyeTrackerSelector;
        [SerializeField] private Transform cameraTransform;

        [Header("Stimulus")] 
        [SerializeField] private GameObject targetObject;
        
        // Variable: minTargetPresentationTime
        // The minimum amount of time to wait before presenting the target, in seconds
        [SerializeField] private float minTargetPresentationTime;
        
        // Variable: maxTargetPresentationTime
        // The maximum amount of time to wait before presenting the target, in seconds
        [SerializeField] private float maxTargetPresentationTime;
        
        // Variable: targetTimeout
        // The amount of time it takes for a target to disappear after it's shown, in seconds
        [SerializeField] private float targetTimeout;

        [SerializeField] private float distanceFromCenterDegrees;
        [SerializeField] private float polarAngle;
        
        [Header("Fixation")]
        [SerializeField] private GameObject fixationDot;
        [SerializeField] private float fixationDotRadiusDegrees;
        [SerializeField] private Material fixationDotMaterial;
        [SerializeField] private float fixationTime;
        [SerializeField] private float maxFixationError;
        
        [Header("Trial Plane")]
        [SerializeField] private GameObject trialPlane;
        [SerializeField] private float trialPlaneDepth;

        [Header("Input")] 
        [SerializeField] private SteamVR_Action_Boolean reactionAction;

        [Header("Audio")] 
        [SerializeField] private bool enableSound;
        [SerializeField] private AudioSource reactionSound;
        
        private IEyeTracker eyeTracker;
        private bool wasFixationBroken;
        private bool trialSuccessful;
        private bool trialOver;
        private bool canReact;

        private bool falseAlarm;
        private bool timedOut;

        private float reactionTime;
        private static readonly string[] ColumnNames = { "ReactionTime", "FalseAlarm", "TimedOut" };

        public IEnumerator Perform()
        {
            Initialize();
            gameObject.SetActive(true);
            yield return WaitForFixation();
            fixationDotMaterial.color = Color.green;
            
            yield return WaitForTarget(Random.Range(minTargetPresentationTime, maxTargetPresentationTime));
            if (!falseAlarm)
            {
                ShowTarget();
                if(enableSound)
                    reactionSound.Play();
                
                canReact = true;
                yield return CheckFixationBreakWithDelay();
                canReact = false;
                trialOver = true;
                HideTarget();
            }

            if (wasFixationBroken)
                trialSuccessful = false;

            if (!falseAlarm && !trialSuccessful)
                timedOut = true;
            gameObject.SetActive(false);
            fixationDot.SetActive(false);
        }

        private IEnumerator WaitForTarget(float waitTime)
        {
            var time = 0.0f;
            while (time < waitTime)
            {
                time += Time.deltaTime;
                if(falseAlarm)
                    yield break;
                yield return null;
            }
        }
        
        public void OnEnable()
        {
            reactionAction.onStateDown += OnConfirmAction;
        }

        public void OnDisable()
        {
            reactionAction.onStateDown -= OnConfirmAction;
        }
        
        private void OnConfirmAction(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
        {
            if (canReact)
                trialSuccessful = true;
            else if (!trialOver)
                falseAlarm = true;
        }

        private void HideTarget()
        {
            targetObject.SetActive(false);
        }

        public string GetTrialName()
        {
            return gameObject.name;
        }

        public string[] GetColumnNames()
        {
            return ColumnNames;
        }

        public string GetPromptText()
        {
            return "React!";
        }

        public UXFDataRow RetrieveTrialData()
        {
            var reactionTimeString = trialSuccessful ? reactionTime.ToString(CultureInfo.InvariantCulture) : "NA";
            return new UXFDataRow {(ColumnNames[0], reactionTimeString), 
                (ColumnNames[1], falseAlarm), 
                (ColumnNames[2], timedOut)};
        }
        
        private void Initialize()
        {
            eyeTracker = eyeTrackerSelector.chosenTracker;
            trialPlane.transform.SetParent(cameraTransform);
            trialPlane.transform.localPosition = new Vector3(0, 0, trialPlaneDepth);
            
            var fixationDotSize = 2 * Mathf.Tan(Utility.ToRadians(fixationDotRadiusDegrees)) * trialPlaneDepth;
            fixationDot.transform.localScale = new Vector3(fixationDotSize, fixationDotSize, 0);
            fixationDotMaterial.color = Color.yellow;
            fixationDot.SetActive(true);
            
            wasFixationBroken = false;
            trialSuccessful = false;
            canReact = false;
            trialOver = false;
            falseAlarm = false;
            timedOut = false;
        }

        private void ShowTarget()
        {
            var magnitude = Mathf.Tan(Utility.ToRadians(distanceFromCenterDegrees)) * trialPlaneDepth;
            var planeLocation = Utility.Rotate2D(Vector2.up * magnitude, polarAngle);
            targetObject.transform.localPosition = new Vector3(planeLocation.x, planeLocation.y, 0);
            targetObject.SetActive(true);
        }

        private IEnumerator WaitForFixation()
        {
            var timeFixated = 0.0f;
            while (timeFixated < fixationTime)
            {
                timeFixated += Time.deltaTime;
                if (Physics.Raycast(cameraTransform.position, 
                    cameraTransform.TransformDirection(eyeTracker.GetLocalGazeDirection()), out var hit))
                {
                    var drawVector = hit.distance *
                                     cameraTransform.TransformDirection(eyeTracker.GetLocalGazeDirection());
                    Debug.DrawRay(cameraTransform.position, drawVector, Color.yellow);
                    if ((hit.point - fixationDot.transform.position).magnitude > maxFixationError)
                        timeFixated = 0.0f;
                }
                else
                {
                    timeFixated = 0.0f;
                }
                yield return null;
            }
        }
        
        private IEnumerator CheckFixationBreakWithDelay()
        {
            var time = 0.0f;
            while (time < targetTimeout)
            {
                if (Physics.Raycast(cameraTransform.position,
                    cameraTransform.TransformDirection(eyeTracker.GetLocalGazeDirection()), out var hit))
                {
                    Debug.DrawRay(cameraTransform.position,
                        hit.distance * cameraTransform.TransformDirection(eyeTracker.GetLocalGazeDirection()),
                        Color.yellow);
                    if ((hit.point - fixationDot.transform.position).magnitude > maxFixationError)
                    {
                        fixationDot.SetActive(false);
                        targetObject.SetActive(false);
                        wasFixationBroken = true;
                        break;
                    }
                }

                time += Time.deltaTime;
                
                if (trialSuccessful)
                {
                    reactionTime = time;
                    break;
                }
                yield return null;
            }
        }
    }
}
