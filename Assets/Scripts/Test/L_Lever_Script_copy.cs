using UnityEngine;
using System.Collections;

public class L_Lever_Script_copy : MonoBehaviour {

    Transform handleTransform;
    private Highlight_Handle_Top_Script handleScript;
    private HingeJoint handleJoint;

    public bool isLLeverUp = true;
    private bool isLocked = true;

    Mastermind_Script mastermindScript;
    
    public int rCommand = -1;

    void Start()
    {
        handleTransform = transform.Find("Handle");
        handleScript = handleTransform.GetComponent<Highlight_Handle_Top_Script>();
        isLLeverUp = true;
        isLocked = true;

        //Add hinge joint to Handle
        handleTransform.gameObject.AddComponent<HingeJoint>();
        handleTransform.GetComponent<HingeJoint>().anchor = new Vector3(0, 0, -1);
        JointLimits hLimits = new JointLimits();
        hLimits.min = -45;
        handleTransform.GetComponent<HingeJoint>().limits = hLimits;
        handleTransform.GetComponent<HingeJoint>().useLimits = true;
        handleTransform.GetComponent<HingeJoint>().axis = new Vector3(0, 1, 0);
        handleTransform.GetComponent<HingeJoint>().connectedBody = transform.Find("Case").GetComponent<Rigidbody>();
        handleJoint = handleTransform.GetComponent<HingeJoint>();

        //if(isServer)
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
            if (handleTransform.eulerAngles.x < 22.5)
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
                    //CmdSendTappedCommand(rCommandUp, isLLeverUp);
                    //Lever changed positions
                    isLLeverUp = true;
                    isLocked = true;
                }
            }
            else if (handleTransform.eulerAngles.x > 22.5)
            {
                handleTransform.localPosition = new Vector3(-0.701f, 0, handleTransform.localPosition.z);
                handleTransform.eulerAngles = new Vector3(
                    44.999f,
                    handleTransform.eulerAngles.y,
                    handleTransform.eulerAngles.z
                );

                //If the last position of the handle was in the middle, and now we are at the down position, then send the command that the L_Lever is now Down
                if (!isLocked)
                {
                    //send command tapped to the Server with the lLeverDownCommand
                    int rCommandDown = (rCommand * 100) + 1;
                    //CmdSendTappedCommand(rCommandDown, isLLeverUp);
                    //Lever changed positions
                    isLLeverUp = false;
                    isLocked = true;
                }
            }
        }
    }
}
