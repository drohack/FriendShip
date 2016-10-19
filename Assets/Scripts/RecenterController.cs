using System;
using UnityEngine;
using System.Collections;
using UnityEngine.VR;
using Valve.VR;

public class RecenterController : MonoBehaviour
{
    [SerializeField]
    public SteamVR_TrackedObject trackedObjL;
    [SerializeField]
    public SteamVR_TrackedObject trackedObjR;

    SteamVR_Controller.Device deviceL;
    SteamVR_Controller.Device deviceR;

    void Start()
    {
        UnityEngine.VR.InputTracking.Recenter();
    }

    void Update()
    {
#if OCULUS
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick) || OVRInput.Get(OVRInput.Button.SecondaryThumbstick))
        {
            UnityEngine.VR.InputTracking.Recenter();
        }
#else
        if(trackedObjL != null && Convert.ToInt32(trackedObjL.index) != -1)
            deviceL = SteamVR_Controller.Input((int)trackedObjL.index);
        if (trackedObjR != null && Convert.ToInt32(trackedObjR.index) != -1)
            deviceR = SteamVR_Controller.Input((int)trackedObjR.index);
        if ((deviceL != null && deviceL.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
            || (deviceR != null && deviceR.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)))
        {
            UnityEngine.VR.InputTracking.Recenter();
        }
#endif
    }
}