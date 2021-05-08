using System;
using PupilLabs;
using UnityEngine;
using UXF;

namespace EyeTracker
{
    public class PupilLabsEyeTracker : MonoBehaviour, IEyeTracker
    {
        [SerializeField] private GazeController gazeController;
        [SerializeField] private RecordingController recordingController;
        [SerializeField] private RequestController requestController;
        [SerializeField] [Range(0,1)] private float confidenceThreshold;

        private Vector3 _localGazeDirection;

        public void OnEnable()
        {
            
            _localGazeDirection = Vector3.forward;
            gazeController.OnReceive3dGaze += ReceiveGaze;

            // This operates under the assumption that local file saving is the primary means of file storage
            var session = Session.instance;
            session.BasePath = ((LocalFileDataHander) session.dataHandlers[0]).storagePath;
            var customPath = session.ParticipantPath + "\\S" + session.number.ToString("D3") + "\\PupilData\\";
            
            recordingController.SetCustomPath(customPath);
            requestController.OnConnected += recordingController.StartRecording;
        }
        
        private void ReceiveGaze(GazeData gazeData)
        {
            if (gazeData.Confidence < confidenceThreshold)
                return;

            _localGazeDirection = gazeData.GazeDirection;
        }

        public Vector3 GetLocalGazeDirection()
        {
            return _localGazeDirection;
        }

        public void OnDestroy()
        {
            recordingController.StopRecording();
        }
    }
}