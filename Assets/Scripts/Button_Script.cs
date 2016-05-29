using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Button_Script : NetworkBehaviour {

    private Highlight_Handle_Top_Script handleScript;
    Animator anim;
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
    void Start() {
        handleScript = transform.Find("Handle").GetComponent<Highlight_Handle_Top_Script>();
        anim = transform.Find("Handle").GetComponent<Animator>();
        isLocked = false;
        if(isServer)
            mastermindScript = GameObject.Find("Mastermind").GetComponent<Mastermind_Script>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocked && !handleScript.isGrabbing)
        {
            isLocked = false;
        }

        if (!isLocked && handleScript.isGrabbing)
        {
            isLocked = true;
            anim.Play("Button_Press_Anim");
            //send tapped rCommand to Server
            CmdSendTappedCommand(rCommand);
        }
    }

    [Command]
    void CmdSendTappedCommand(int sentRCommand)
    {
        Debug.Log("CmdSendTappedCommand: " + sentRCommand);
        mastermindScript.TappedWaitForSecondsOrTap(sentRCommand);
    }
}
