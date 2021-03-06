﻿using UnityEngine;
using System.Collections;

public class Valve_Script_copy : MonoBehaviour {

    private Transform handleTransform;
    private Highlight_Handle_Top_Script handleScript;
    public float valveLastRotation;
    public float valveTotalRotation;
    
    private bool isCommandSent = false;

    Mastermind_Script mastermindScript;

    public int rCommand = -1;

    // Use this for initialization
    void Start () {
        handleTransform = transform.Find("Handle");
        handleScript = handleTransform.GetComponent<Highlight_Handle_Top_Script>();
        valveLastRotation = handleTransform.localEulerAngles.z;
        valveTotalRotation = 0f;
        isCommandSent = false;

        //Add hinge joint to Handle
        handleTransform.gameObject.AddComponent<HingeJoint>();
        handleTransform.GetComponent<HingeJoint>().axis = new Vector3(0, 0, 1);
        handleTransform.GetComponent<HingeJoint>().useSpring = true;
        JointSpring springJoint = new JointSpring();
        springJoint.spring = 1f;
        springJoint.damper = 0.0001f;
        springJoint.targetPosition = handleTransform.localEulerAngles.z;
        handleTransform.GetComponent<HingeJoint>().spring = springJoint;
        handleTransform.GetComponent<Rigidbody>().isKinematic = false;
        
        //    mastermindScript = GameObject.FindGameObjectWithTag("Mastermind").GetComponent<Mastermind_Script>();
    }
	
	// Update is called once per frame
	void Update () {
        handleTransform.localPosition = new Vector3(0, 0, 0);

        // If the valve was let go and we already sent the command, set the released position to be the new zero
        if (!handleScript.isGrabbing && isCommandSent)
        {
            valveTotalRotation = 0f;
            isCommandSent = false;
            handleTransform.GetComponent<HingeJoint>().useSpring = true;
            JointSpring springJoint = handleTransform.GetComponent<HingeJoint>().spring;
            float targetPosition = (handleTransform.localEulerAngles.z > 180) ? (handleTransform.localEulerAngles.z - 360) : handleTransform.localEulerAngles.z;
            springJoint.targetPosition = targetPosition;
            handleTransform.GetComponent<HingeJoint>().spring = springJoint;
        }

        if(handleScript.isGrabbing)
        {
            handleTransform.GetComponent<HingeJoint>().useSpring = false;
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
                //int rCommandClockwise = (rCommand * 100) + 1;
                //CmdSendTappedCommand(rCommandClockwise);
            }
            else if (valveTotalRotation < -360)
            {
                isCommandSent = true;
                //send tapped command to Mastermind
                //int rCommandCounterClockwise = (rCommand * 100) + 2;
                //CmdSendTappedCommand(rCommandCounterClockwise);
            }
        }
    }

    //[Command]
    //void CmdSendTappedCommand(int sentRCommand)
    //{
    //    mastermindScript.TappedWaitForSecondsOrTap(sentRCommand);
    //}
}
