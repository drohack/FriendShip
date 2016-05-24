using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Button_Script : NetworkBehaviour {

    private Highlight_Handle_Top_Script handleScript;
    Animator anim;
    private bool isLocked = false;

    public int rCommand = -1;

    Mastermind_Script mastermindScript;

    [SyncVar(hook = "UpdateName")]
    public string newName;

    private void UpdateName(string name)
    {
        transform.Find("Labels/Name").GetComponent<TextMesh>().text = name;
    }

    // Use this for initialization
    void Start () {
        handleScript = transform.Find("Handle").GetComponent<Highlight_Handle_Top_Script>();
        anim = transform.Find("Handle").GetComponent<Animator>();
        isLocked = false;
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
            //Check to see if the command is to press the button (rCommand == 0)
            //send command tapped to the Console_Text_Script
            mastermindScript.TappedWaitForSecondsOrTap(rCommand);
        }
    }
}
