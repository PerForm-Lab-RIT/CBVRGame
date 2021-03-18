using System;
using EyeTracker;
using UnityEngine;

public class SelectEyeTracker : MonoBehaviour
{
    private enum ETrackerSelection
    {
        PupilLabs,
        Dummy
    }

    [SerializeField] private ETrackerSelection selection;
    private ETrackerSelection cachedSelection;
    
    [SerializeField] private GameObject pupilEyeTracker;
    [SerializeField] private bool enableDebugView;
    [SerializeField] private float debugDistance;
    [SerializeField] private Transform cameraOrigin;

    private DummyEyeTracker dummyEyeTracker;
    
    private IEyeTracker pupilTracker;

    public IEyeTracker chosenTracker { get; private set; }
    
    public void Start()
    {
        pupilTracker = pupilEyeTracker.GetComponent<IEyeTracker>();
        dummyEyeTracker = new DummyEyeTracker();
        UpdateChosenEyeTracker();
        cachedSelection = selection;
    }

    private void UpdateChosenEyeTracker()
    {
        switch (selection)
        {
            case ETrackerSelection.PupilLabs:
                pupilEyeTracker.SetActive(true);
                chosenTracker = pupilTracker;
                break;
            case ETrackerSelection.Dummy:
                chosenTracker = dummyEyeTracker;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Update()
    {
        if (enableDebugView)
            Debug.DrawRay(cameraOrigin.position, debugDistance * cameraOrigin.TransformDirection(chosenTracker.GetLocalGazeDirection()));
        
        if (cachedSelection != selection)
        {
            UpdateChosenEyeTracker();
            cachedSelection = selection;
        }
    }
}
