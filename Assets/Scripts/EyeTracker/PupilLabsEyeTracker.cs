using PupilLabs;
using UnityEngine;

namespace EyeTracker
{
    public class PupilLabsEyeTracker : MonoBehaviour, IEyeTracker
    {
        [SerializeField] private RequestController requestController;
        [SerializeField] private GazeController gazeController;
        [SerializeField] private RecordingController recordingController;
        [SerializeField] [Range(0,1)] private float confidenceThreshold;
        

        private Vector3 _localGazeDirection;

        public void OnEnable()
        {
            
            _localGazeDirection = Vector3.forward;
            requestController.OnConnected += recordingController.StartRecording;
            gazeController.OnReceive3dGaze += ReceiveGaze;
        }
        
        public void OnDisable()
        {
            
            requestController.OnConnected -= recordingController.StartRecording;
            gazeController.OnReceive3dGaze -= ReceiveGaze;
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
    }
}