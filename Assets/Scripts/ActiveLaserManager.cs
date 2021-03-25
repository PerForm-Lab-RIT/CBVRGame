using ScriptableObjects;
using UnityEngine;
using Valve.VR;

public class ActiveLaserManager : MonoBehaviour
{
    [SerializeField] private GameObject leftHandPointer;
    [SerializeField] private GameObject rightHandPointer;
    
    [SerializeField] private SteamVR_Action_Vector2 angleSelectAction;
    [SerializeField] private SteamVR_Action_Boolean confirmAction;
    [SerializeField] private FloatReference deadzone;

    [SerializeField] private GameEvent onControllerChange;

    private GameObject _activeLaser;
    private bool _deactivated;
    
    public void Start()
    {
        _activeLaser = rightHandPointer;
        DeactivateBothLasers();
        confirmAction[SteamVR_Input_Sources.LeftHand].onChange += UpdateActiveLaser;
        confirmAction[SteamVR_Input_Sources.RightHand].onChange += UpdateActiveLaser;
        angleSelectAction[SteamVR_Input_Sources.LeftHand].onChange += UpdateActiveLaser;
        angleSelectAction[SteamVR_Input_Sources.RightHand].onChange += UpdateActiveLaser;
    }

    private void UpdateActiveLaser(SteamVR_Action_Boolean action, SteamVR_Input_Sources source, bool state)
    {
        GameObject inactiveLaser;
        if (source == SteamVR_Input_Sources.RightHand)
        {
            if(!_deactivated && _activeLaser != rightHandPointer)
                onControllerChange.Raise();
            _activeLaser = rightHandPointer;
            inactiveLaser = leftHandPointer;
        }
        else
        {
            if(!_deactivated && _activeLaser != leftHandPointer)
                onControllerChange.Raise();
            _activeLaser = leftHandPointer;
            inactiveLaser = rightHandPointer;
        }

        if (!_deactivated)
        {
            _activeLaser.SetActive(true);
            inactiveLaser.SetActive(false);
        }
    }
    
    private void UpdateActiveLaser(SteamVR_Action_Vector2 action, SteamVR_Input_Sources source, Vector2 axis, Vector2 delta)
    {
        if (axis.sqrMagnitude < deadzone.Value * deadzone.Value)
            return;
        
        GameObject inactiveLaser;

        if (source == SteamVR_Input_Sources.RightHand)
        {
            if(!_deactivated && _activeLaser != rightHandPointer)
                onControllerChange.Raise();
            _activeLaser = rightHandPointer;
            inactiveLaser = leftHandPointer;
        }
        else
        {
            if(!_deactivated && _activeLaser != leftHandPointer)
                onControllerChange.Raise();
            _activeLaser = leftHandPointer;
            inactiveLaser = rightHandPointer;
        }

        if (!_deactivated)
        {
            _activeLaser.SetActive(true);
            inactiveLaser.SetActive(false);
        }
    }

    public void DeactivateBothLasers()
    {
        leftHandPointer.SetActive(false);
        rightHandPointer.SetActive(false);
        _deactivated = true;
    }

    public Transform GetActiveHandTransform()
    {
        return _activeLaser.GetComponentInParent<Transform>();
    }

    public void ActivateLaser()
    {
        _activeLaser.SetActive(true);
        _deactivated = false;
    }

    public void OnDisable()
    {
        confirmAction[SteamVR_Input_Sources.LeftHand].onChange -= UpdateActiveLaser;
        confirmAction[SteamVR_Input_Sources.RightHand].onChange -= UpdateActiveLaser;
        angleSelectAction[SteamVR_Input_Sources.LeftHand].onChange -= UpdateActiveLaser;
        angleSelectAction[SteamVR_Input_Sources.RightHand].onChange -= UpdateActiveLaser;
    }
}
