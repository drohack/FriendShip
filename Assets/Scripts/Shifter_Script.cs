using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Shifter_Script : NetworkBehaviour {

    private Transform handleTransform;
    private Highlight_Handle_Top_Script handleScript;
    private HingeJoint m_HingeJoint;

    public int shifterPosition;
    
    private bool isLocked = true;

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
        handleScript = handleTransform.GetComponent<Highlight_Handle_Top_Script>();
        
        shifterPosition = 1;
        isLocked = true;

        //Add hinge joint to Handle
        handleTransform.gameObject.AddComponent<HingeJoint>();
        handleTransform.GetComponent<HingeJoint>().axis = new Vector3(0, 0, 1);
        handleTransform.GetComponent<HingeJoint>().useMotor = true;
        JointMotor hMotor = new JointMotor();
        hMotor.force = 2;
        handleTransform.GetComponent<HingeJoint>().motor = hMotor;
        handleTransform.GetComponent<HingeJoint>().useLimits = true;
        JointLimits hLimits = new JointLimits();
        hLimits.min = -45;
        hLimits.max = 45;
        handleTransform.GetComponent<HingeJoint>().limits = hLimits;
        m_HingeJoint = handleTransform.GetComponent<HingeJoint>();

        if (isServer)
            mastermindScript = GameObject.Find("Mastermind").GetComponent<Mastermind_Script>();
    }

    private void Update()
    {
        handleTransform.localPosition = new Vector3(0, 0, 0);

        if (isLocked && handleScript.isGrabbing)
        {
            isLocked = false;
        }

        if (!handleScript.isGrabbing)
        {
            //snap lever into place near edges (on = handleTransform.eulerAngles.z == 0; off = handleTransform.eulerAngles.z == 45)
            if (handleTransform.eulerAngles.z > 313.5)
            {
                handleTransform.eulerAngles = new Vector3(
                    handleTransform.eulerAngles.x,
                    handleTransform.eulerAngles.y,
                    315
                );

                if (!isLocked)
                {
                    isLocked = true;
                    //Lever changed positions
                    shifterPosition = 2;
                    //send command tapped to the Server
                    int rCommandTwo = (rCommand * 100) + 2;
                    CmdSendTappedCommand(rCommandTwo, shifterPosition);
                }
            }
            else if (handleTransform.eulerAngles.z > 267 && handleTransform.eulerAngles.z < 273)
            {
                handleTransform.eulerAngles = new Vector3(
                    handleTransform.eulerAngles.x,
                    handleTransform.eulerAngles.y,
                    270
                );

                if (!isLocked)
                {
                    isLocked = true;
                    //Lever changed positions
                    shifterPosition = 1;
                    //send command tapped to the Server
                    int rCommandOne = (rCommand * 100) + 1;
                    CmdSendTappedCommand(rCommandOne, shifterPosition);
                }
            }
            else if (handleTransform.eulerAngles.z < 226.5)
            {
                handleTransform.eulerAngles = new Vector3(
                    handleTransform.eulerAngles.x,
                    handleTransform.eulerAngles.y,
                    225
                );

                if (!isLocked)
                {
                    isLocked = true;
                    //Lever changed positions
                    shifterPosition = 0;
                    //send command tapped to the Server
                    int rCommandZero = (rCommand * 100) + 0;
                    CmdSendTappedCommand(rCommandZero, shifterPosition);
                }
            }
            else
            {
                //push lever in direction to go towards edges
                if (handleTransform.eulerAngles.z > 292.5 || (handleTransform.eulerAngles.z > 247.5 && handleTransform.eulerAngles.z < 270))
                {
                    JointMotor motor = m_HingeJoint.motor;
                    motor.targetVelocity = 20;
                    m_HingeJoint.motor = motor;
                }
                else
                {
                    JointMotor motor = m_HingeJoint.motor;
                    motor.targetVelocity = -20;
                    m_HingeJoint.motor = motor;
                }
            }
        }
        else
        {
            if (handleTransform.eulerAngles.z > 292.5 || (handleTransform.eulerAngles.z > 247.5 && handleTransform.eulerAngles.z < 270))
            {
                JointMotor motor = m_HingeJoint.motor;
                motor.targetVelocity = 20;
                m_HingeJoint.motor = motor;
            }
            else
            {
                JointMotor motor = m_HingeJoint.motor;
                motor.targetVelocity = -20;
                m_HingeJoint.motor = motor;
            }
        }
    }

    [Command]
    void CmdSendTappedCommand(int sentRCommand, int sentShifterPosition)
    {
        shifterPosition = sentShifterPosition;
        mastermindScript.TappedWaitForSecondsOrTap(sentRCommand);
    }
}
