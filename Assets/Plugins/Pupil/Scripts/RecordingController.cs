using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PupilLabs
{
    [HelpURL("https://github.com/pupil-labs/hmd-eyes/blob/master/docs/Developer.md#recording-data")]
    public class RecordingController : MonoBehaviour
    {
        public RequestController requestCtrl;

        [Header("Recording Path")]
        public bool useCustomPath;
        [SerializeField] private string customPath;

        [Header("Controls")]
        [SerializeField] private bool recordEyeFrames = true;
        [SerializeField] private bool startRecording;
        [SerializeField] private bool stopRecording;
        [SerializeField] private InputActionReference recordToggle;
        [SerializeField] private GameObject recordingIndicator;

        public bool IsRecording { get; private set; }

        void OnEnable()
        {
            if (requestCtrl == null)
            {
                Debug.LogError("RecordingController is missing the required RequestController reference. Please connect the reference, or the component won't work correctly.");
                enabled = false;
                return;
            }

            recordToggle.action.performed += ToggleRecording;
        }

        void OnDisable()
        {
            if (IsRecording)
            {
                StopRecording();
            }
            recordToggle.action.performed -= ToggleRecording;
        }

        void ToggleRecording(InputAction.CallbackContext context)
        {
            if (!IsRecording)
            {
                StartRecording();
            } 
            else if (IsRecording)
            {
                StopRecording();
            }
        }

        public void StartRecording()
        {
            if (!enabled)
            {
                Debug.LogWarning("Component not enabled");
                return;
            }

            if (!requestCtrl.IsConnected)
            {
                Debug.LogWarning("Not connected");
                return;
            }

            if (IsRecording)
            {
                Debug.Log("Recording is already running.");
                return;
            }

            recordingIndicator.SetActive(true);
            var path = GetRecordingPath();

            requestCtrl.Send(new Dictionary<string, object>
            {
                { "subject","recording.should_start" }
                , { "session_name", path }
                , { "record_eye",recordEyeFrames}
            });

            //abort process on disconnecting
            requestCtrl.OnDisconnecting += StopRecording;
            IsRecording = true;
        }

        public void StopRecording()
        {
            if (!IsRecording)
            {
                Debug.Log("Recording is not running, nothing to stop.");
                return;
            }

            requestCtrl.Send(new Dictionary<string, object>
            {
                { "subject", "recording.should_stop" }
            });

            recordingIndicator.SetActive(false);

            requestCtrl.OnDisconnecting -= StopRecording;
            IsRecording = false;
        }

        public void SetCustomPath(string path)
        {
            useCustomPath = true;
            customPath = path;
        }

        private string GetRecordingPath()
        {
            string path = "";

            if (useCustomPath)
            {
                path = customPath;
            }
            else
            {
                string date = System.DateTime.Now.ToString("yyyy_MM_dd");
                path = $"{Application.dataPath}/{date}";
                path = path.Replace("Assets/", ""); //go one folder up
            }

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            return path;
        }
    }
}
