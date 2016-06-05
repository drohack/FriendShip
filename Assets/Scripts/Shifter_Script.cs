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

        if (handleScript.isGrabbing)
        {
            Debug.Log(handleTransform.localEulerAngles.z);
            isLocked = false;
        }
        else 
        {
            if (handleTransform.localEulerAngles.z > 22.5 && handleTransform.localEulerAngles.z < 180)
            {
                handleTransform.localEulerAngles = new Vector3(
                    0f,
                    0f,
                    44.999f
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
            else if (handleTransform.localEulerAngles.z > 337.5 || handleTransform.localEulerAngles.z < 22.5)
            {
                handleTransform.localEulerAngles = new Vector3(
                    0f,
                    0f,
                    0f
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
            else if (handleTransform.localEulerAngles.z < 337.5 && handleTransform.localEulerAngles.z > 180)
            {
                handleTransform.localEulerAngles = new Vector3(
                    0f,
                    0f,
                    315.001f
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
        }
    }

    [Command]
    void CmdSendTappedCommand(int sentRCommand, int sentShifterPosition)
    {
        shifterPosition = sentShifterPosition;
        mastermindScript.TappedWaitForSecondsOrTap(sentRCommand);
    }
}
