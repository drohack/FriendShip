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
    public int rCommand = -1;

    Mastermind_Script mastermindScript;

    [SyncVar(hook = "UpdateName")]
    public string newName;

    private void UpdateName(string name)
    {
       // transform.Find("Labels/Name").GetComponent<TextMesh>().text = name;
		transform.GetChild (0).transform.GetChild (0).GetComponent<TextMesh> ().text = name;
    }

    void Start()
    {
        handleTransform = transform.Find("Handle");
        handleJoint = handleTransform.GetComponent<HingeJoint>();
        isWLeverUp = true;
        lastHandlePosition = upPosition;
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
                int rCommandUp = (rCommand * 100) + 1;
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
                int rCommandDown = (rCommand * 100) + 2;
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
