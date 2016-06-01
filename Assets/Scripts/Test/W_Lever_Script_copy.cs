using UnityEngine;
using System.Collections;

public class W_Lever_Script_copy : MonoBehaviour
{

    Transform handleTransform;
    private Highlight_Handle_Top_Script handleScript;
    private HingeJoint handleJoint;

    public bool isWLeverUp = true;
    private bool isLocked = true;

    Mastermind_Script mastermindScript;

    //Network variables
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
        isWLeverUp = true;
        isLocked = true;

        //Add hinge joint to Handle
        handleTransform.gameObject.AddComponent<HingeJoint>();
        handleTransform.GetComponent<HingeJoint>().anchor = new Vector3(0, 0, -1);
        handleTransform.GetComponent<HingeJoint>().axis = new Vector3(0, 1, 0);
        handleTransform.GetComponent<HingeJoint>().useLimits = true;
        JointLimits hLimits = new JointLimits();
        hLimits.min = -35;
        handleTransform.GetComponent<HingeJoint>().limits = hLimits;
        handleTransform.GetComponent<HingeJoint>().connectedBody = transform.Find("Case").GetComponent<Rigidbody>();
        handleJoint = handleTransform.GetComponent<HingeJoint>();

        //if (isServer)
        //    mastermindScript = GameObject.Find("Mastermind").GetComponent<Mastermind_Script>();
    }

    private void Update()
    {
        if (handleScript.isGrabbing)
        {
            isLocked = false;
        }
        else
        {
            //snap lever into place near edges (on = handleTransform.eulerAngles.x == 0; off = handleTransform.eulerAngles.x == 45)
            if (handleTransform.eulerAngles.x < 17.5)
            {
                handleTransform.localPosition = new Vector3(0, 0, handleTransform.localPosition.z);
                handleTransform.eulerAngles = new Vector3(
                    0.001f,
                    handleTransform.eulerAngles.y,
                    handleTransform.eulerAngles.z
                );

                //If the last position of the handle was in the middle, and now we are at the up position, then send the command that the L_Lever is now Up
                if (!isLocked)
                {
                    //send command tapped to the Server with the lLeverUpCommand
                    int rCommandUp = (rCommand * 100) + 2;
                    //CmdSendTappedCommand(rCommandUp, isWLeverUp);
                    //Lever changed positions
                    isWLeverUp = true;
                    isLocked = true;
                }
            }
            else if (handleTransform.eulerAngles.x > 17.5)
            {
                handleTransform.localPosition = new Vector3(-0.57f, 0, handleTransform.localPosition.z);
                handleTransform.eulerAngles = new Vector3(
                    34.999f,
                    handleTransform.eulerAngles.y,
                    handleTransform.eulerAngles.z
                );

                //If the last position of the handle was in the middle, and now we are at the down position, then send the command that the L_Lever is now Down
                if (!isLocked)
                {
                    //send command tapped to the Server with the lLeverDownCommand
                    int rCommandDown = (rCommand * 100) + 1;
                    //CmdSendTappedCommand(rCommandDown, isWLeverUp);
                    //Lever changed positions
                    isWLeverUp = false;
                    isLocked = true;
                }
            }
        }
    }

    //[Command]
    //void CmdSendTappedCommand(int sentRCommand, bool sentIsWLeverUp)
    //{
    //    isWLeverUp = sentIsWLeverUp;
    //    mastermindScript.TappedWaitForSecondsOrTap(sentRCommand);
    //}
}
