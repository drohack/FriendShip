using UnityEngine;
using System.Collections;

public class Dial_Script_copy : MonoBehaviour {

    Transform handleTransform;
    private Highlight_Handle_Top_Script handleScript;
    public int dialPosition;

    private bool isLocked = false;

    Mastermind_Script mastermindScript;
    
    public int rCommand = -1;

    private void UpdateQuaternion(Quaternion newQuaternion)
    {
        transform.rotation = newQuaternion;
    }
    private void UpdateName(string name)
    {
        transform.Find("Labels/Name").GetComponent<TextMesh>().text = name;
    }
    private void UpdateRCommand(int command)
    {
        rCommand = command;
    }

    void Start()
    {
        handleTransform = transform.Find("Handle");
        handleScript = handleTransform.GetComponent<Highlight_Handle_Top_Script>();
        dialPosition = 0;
        isLocked = true;

        //Add hinge joint to Handle
        handleTransform.gameObject.AddComponent<HingeJoint>();
        handleTransform.GetComponent<HingeJoint>().axis = new Vector3(0, 1, 0);
        handleTransform.GetComponent<HingeJoint>().useLimits = true;
        JointLimits hLimits = new JointLimits();
        hLimits.min = -90;
        hLimits.max = 90;
        handleTransform.GetComponent<HingeJoint>().limits = hLimits;

        handleTransform.localEulerAngles = new Vector3(0, 0, 0);
        
        //    mastermindScript = GameObject.FindGameObjectWithTag("Mastermind").GetComponent<Mastermind_Script>();
    }

    private void Update()
    {
        handleTransform.localPosition = new Vector3(0, 0, 0);

        if(handleScript.isGrabbing)
        {
            isLocked = false;
            handleTransform.localEulerAngles = new Vector3(0, handleTransform.localEulerAngles.y, 0);
        }
        else
        {
            //snap lever into place near edges 
            if (handleTransform.localEulerAngles.y > 162)
            {
                handleTransform.localEulerAngles = new Vector3(0, 179.9f, 0);

                if (!isLocked)
                {
                    isLocked = true;
                    //Lever changed positions
                    dialPosition = 5;
                    //send command tapped to the Server
                    //int rCommandFive = (rCommand * 100) + 5;
                    //CmdSendTappedCommand(rCommandFive, dialPosition);
                }
            }
            else if (handleTransform.localEulerAngles.y > 126 && handleTransform.localEulerAngles.y < 162)
            {
                handleTransform.localEulerAngles = new Vector3(0, 144, 0);

                if (!isLocked)
                {
                    isLocked = true;
                    //Lever changed positions
                    dialPosition = 4;
                    //send command tapped to the Server
                    //int rCommandFour = (rCommand * 100) + 4;
                    //CmdSendTappedCommand(rCommandFour, dialPosition);
                }
            }
            else if (handleTransform.localEulerAngles.y > 90 && handleTransform.localEulerAngles.y < 126)
            {
                handleTransform.localEulerAngles = new Vector3(0, 108, 0);

                if (!isLocked)
                {
                    isLocked = true;
                    //Lever changed positions
                    dialPosition = 3;
                    //send command tapped to the Server
                    //int rCommandThree = (rCommand * 100) + 3;
                    //CmdSendTappedCommand(rCommandThree, dialPosition);
                }
            }
            else if (handleTransform.localEulerAngles.y > 54 && handleTransform.localEulerAngles.y < 90)
            {
                handleTransform.localEulerAngles = new Vector3(0, 72, 0);

                if (!isLocked)
                {
                    isLocked = true;
                    //Lever changed positions
                    dialPosition = 2;
                    //send command tapped to the Server
                    //int rCommandTwo = (rCommand * 100) + 2;
                    //CmdSendTappedCommand(rCommandTwo, dialPosition);
                }
            }
            else if (handleTransform.localEulerAngles.y > 18 && handleTransform.localEulerAngles.y < 54)
            {
                handleTransform.localEulerAngles = new Vector3(0, 36, 0);

                if (!isLocked)
                {
                    isLocked = true;
                    //Lever changed positions
                    dialPosition = 1;
                    //send command tapped to the Server
                    //int rCommandOne = (rCommand * 100) + 1;
                    //CmdSendTappedCommand(rCommandOne, dialPosition);
                }
            }
            else if (handleTransform.localEulerAngles.y < 18)
            {
                handleTransform.localEulerAngles = new Vector3(0, 0, 0);

                if (!isLocked)
                {
                    isLocked = true;
                    //Lever changed positions
                    dialPosition = 0;
                    //send command tapped to the Server
                    //int rCommandZero = (rCommand * 100) + 0;
                    //CmdSendTappedCommand(rCommandZero, dialPosition);
                }
            }
        }
    }

    //[Command]
    //void CmdSendTappedCommand(int sentRCommand, int sentDialPosition)
    //{
    //    dialPosition = sentDialPosition;
    //    mastermindScript.TappedWaitForSecondsOrTap(sentRCommand);
    //}
}
