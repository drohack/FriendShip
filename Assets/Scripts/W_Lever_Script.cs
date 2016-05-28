using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class W_Lever_Script : NetworkBehaviour
{

    Transform handleTransform;
    private HingeJoint handleJoint;

    const int upPosition = 1;
    const int middlePosition = 0;
    const int downPosition = -1;

    private int lastHandlePosition;

    public bool isWLeverUp = true;

    Mastermind_Script mastermindScript;

    //Network variables
    [SyncVar(hook = "UpdateQuaternion")]
    public Quaternion newQuaternion;
    [SyncVar(hook = "UpdateName")]
    public string newName;
    [SyncVar(hook = "UpdateRCommand")]
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
        isWLeverUp = true;
        lastHandlePosition = upPosition;

        //Add hinge joint to Handle
        handleTransform.gameObject.AddComponent<HingeJoint>();
        handleTransform.GetComponent<HingeJoint>().anchor = new Vector3(0, 0, -1);
        handleTransform.GetComponent<HingeJoint>().axis = new Vector3(0, 1, 0);
        handleTransform.GetComponent<HingeJoint>().useMotor = true;
        JointMotor hMotor = new JointMotor();
        hMotor.force = 2;
        handleTransform.GetComponent<HingeJoint>().motor = hMotor;
        handleTransform.GetComponent<HingeJoint>().useLimits = true;
        JointLimits hLimits = new JointLimits();
        hLimits.min = -35;
        handleTransform.GetComponent<HingeJoint>().limits = hLimits;
        handleTransform.GetComponent<HingeJoint>().connectedBody = transform.Find("Case").GetComponent<Rigidbody>();
        handleJoint = handleTransform.GetComponent<HingeJoint>();

        mastermindScript = GameObject.Find("Mastermind").GetComponent<Mastermind_Script>();
    }

    private void Update()
    {
        //snap lever into place near edges (on = handleTransform.eulerAngles.x == 0; off = handleTransform.eulerAngles.x == 35)
        if (handleTransform.eulerAngles.x < 1.5)
        {
            handleTransform.eulerAngles = new Vector3(
                0,
                handleTransform.eulerAngles.y,
                handleTransform.eulerAngles.z
            );

            //If the last position of the handle was in the middle, and now we are at the up position, then send the command that the W_Lever is now Up
            if (lastHandlePosition == middlePosition)
            {
                //send command tapped to the Console_Text_Script with wLeverUpCommand
                int rCommandUp = (rCommand * 100) + 2;
                mastermindScript.TappedWaitForSecondsOrTap(rCommandUp);
                //Lever changed positions
                isWLeverUp = true;
            }

            //update last handle position
            lastHandlePosition = upPosition;
        }
        else if (handleTransform.eulerAngles.x > 33.5)
        {
            handleTransform.eulerAngles = new Vector3(
                35,
                handleTransform.eulerAngles.y,
                handleTransform.eulerAngles.z
            );

            //If the last position of the handle was in the middle, and now we are at the down position, then send the command that the W_Lever is now Down
            if (lastHandlePosition == middlePosition)
            {
                //send command tapped to the Console_Text_Script with wLeverDownCommand
                int rCommandDown = (rCommand * 100) + 1;
                mastermindScript.TappedWaitForSecondsOrTap(rCommandDown);
                //Lever changed positions
                isWLeverUp = false;
            }

            //update last handle position
            lastHandlePosition = downPosition;
        }
        else
        {
            lastHandlePosition = middlePosition;
        }

        //push lever in direction to go towards edges
        if (handleTransform.eulerAngles.x < 17.5)
        {
            JointMotor motor = handleJoint.motor;
            motor.targetVelocity = 20;
            handleJoint.motor = motor;
        }
        else
        {
            JointMotor motor = handleJoint.motor;
            motor.targetVelocity = -20;
            handleJoint.motor = motor;
        }
    }
}
