using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Lightswitch_Script : NetworkBehaviour {

    private Highlight_Handle_Top_Script handleScript;
    Animator anim;
    public bool isLightswitchOn = true;
    private bool isLocked = false;

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

    // Use this for initialization
    void Start () {
        handleScript = transform.Find("Handle").GetComponent<Highlight_Handle_Top_Script>();
        isLightswitchOn = true;
        isLocked = false;
        anim = transform.Find("Handle").GetComponent<Animator>();

        if(isServer)
            mastermindScript = GameObject.Find("Mastermind").GetComponent<Mastermind_Script>();
    }
	
	// Update is called once per frame
	void Update () {
        if(isLocked && !handleScript.isGrabbing)
        {
            isLocked = false;
        }

        if (!isLocked && handleScript.isGrabbing)
        {
            isLocked = true;
            if (isLightswitchOn)
            {
                isLightswitchOn = false;
                anim.Play("Lightswitch_Off_Anim");
                //send tapped command to Mastermind
                int rCommandUp = (rCommand * 100) + 1;
                CmdSendTappedCommand(rCommandUp, isLightswitchOn);
            }
            else
            {
                isLightswitchOn = true;
                anim.Play("Lightswitch_On_Anim");
                //send tapped command to Mastermind
                int rCommandDown = (rCommand * 100) + 2;
                CmdSendTappedCommand(rCommandDown, isLightswitchOn);
            }
        }
    }

    [Command]
    void CmdSendTappedCommand(int sentRCommand, bool sentIsLightswitchOn)
    {
        isLightswitchOn = sentIsLightswitchOn;
        mastermindScript.TappedWaitForSecondsOrTap(sentRCommand);
    }
}
