using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Valve_Script : NetworkBehaviour {

    private Transform handleTransform;
    private Highlight_Handle_Top_Script handleScript;
    public float valveLastRotation;
    public float valveTotalRotation;

    public int rCommand = -1;
    
    private bool isCommandSent = false;

    Mastermind_Script mastermindScript;

    [SyncVar(hook = "UpdateName")]
    public string newName;

    private void UpdateName(string name)
    {
        transform.Find("Labels/Name").GetComponent<TextMesh>().text = name;
    }

    // Use this for initialization
    void Start () {
        handleTransform = transform.Find("Handle");
        handleScript = handleTransform.GetComponent<Highlight_Handle_Top_Script>();
        valveLastRotation = handleTransform.localEulerAngles.z;
        valveTotalRotation = 0f;
        isCommandSent = false;
        mastermindScript = GameObject.Find("Mastermind").GetComponent<Mastermind_Script>();
    }
	
	// Update is called once per frame
	void Update () {
        handleTransform.localPosition = new Vector3(0, 0, 0);

        // If the valve was let go and we already sent the command, set the released position to be the new zero
        if (!handleScript.isGrabbing && isCommandSent)
        {
            valveTotalRotation = 0f;
            isCommandSent = false;
        }

        // Check for edge cases as localEulerAngle only goes from 0 to 359 then starts over at 0
        if (handleTransform.localEulerAngles.z - valveLastRotation < -300)
        {
            valveLastRotation -= 360;
        }
        else if (handleTransform.localEulerAngles.z - valveLastRotation > 300)
        {
            valveLastRotation += 360;
        }
        // Update total rotation to the difference between the last rotation and the current rotation
        valveTotalRotation += handleTransform.localEulerAngles.z - valveLastRotation;
        //Debug.Log("Total rotation: " + valveTotalRotation + " isCommandSent: " + isCommandSent);
        valveLastRotation = handleTransform.localEulerAngles.z;

        // Only need to send the command once
        if (!isCommandSent)
        {
            if (valveTotalRotation > 360)
            {
                isCommandSent = true;
                //send tapped command to Mastermind
                int rCommandClockwise = (rCommand * 100) + 1;
                mastermindScript.TappedWaitForSecondsOrTap(rCommandClockwise);
            }
            else if (valveTotalRotation < -360)
            {
                isCommandSent = true;
                //send tapped command to Mastermind
                int rCommandCounterClockwise = (rCommand * 100) + 2;
                mastermindScript.TappedWaitForSecondsOrTap(rCommandCounterClockwise);
            }
        }
    }
}
