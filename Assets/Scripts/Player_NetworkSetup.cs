using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using OvrTouch.Hands;

public class Player_NetworkSetup : NetworkBehaviour {

    [SerializeField] OVRCameraRig ovrCameraRig;
    [SerializeField] Camera FPSCharacterCam;
	[SerializeField] AudioListener audioListener;
    [SerializeField] Hand handScriptL;
    [SerializeField] VelocityTracker velocityTrackerL;
    [SerializeField] Hand handScriptR;
    [SerializeField] VelocityTracker velocityTrackerR;
    // Use this for initialization
    void Start () 
	{
        Debug.Log("In Player_NetworkSetup, isLocalPlayer: " + isLocalPlayer);
		if (isLocalPlayer) 
		{
			GameObject.Find ("Main Camera").SetActive (false);
            // Add OVRManager
            transform.Find("OVRCameraRig").gameObject.AddComponent<OVRManager>();
            ovrCameraRig.enabled = true;
            FPSCharacterCam.enabled = true;
            audioListener.enabled = true;
            handScriptL.enabled = true;
            velocityTrackerL.enabled = true;
            handScriptR.enabled = true;
            velocityTrackerR.enabled = true;
        }
	}
}
