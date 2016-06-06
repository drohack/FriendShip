using UnityEngine;
using System.Collections;
using UnityEngine.VR;

public class RecenterController : MonoBehaviour
{

    void Start()
    {
        UnityEngine.VR.InputTracking.Recenter();
    }

    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick) || OVRInput.Get(OVRInput.Button.SecondaryThumbstick))
        {
            UnityEngine.VR.InputTracking.Recenter();
        }
    }
}