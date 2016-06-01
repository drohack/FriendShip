using UnityEngine;
using System.Collections;

public class Button_Script_copy : MonoBehaviour {

    private Highlight_Handle_Top_Script handleScript;
    Animator anim;
    private bool isLocked = false;

    Mastermind_Script mastermindScript;

    //Network variables
    public Quaternion newQuaternion;
    public string newName;
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
        anim = transform.Find("Handle").GetComponent<Animator>();
        isLocked = false;
        //mastermindScript = GameObject.Find("Mastermind").GetComponent<Mastermind_Script>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocked && !handleScript.isGrabbing && !handleScript.isColliding)
        {
            isLocked = false;
        }

        if (!isLocked && (handleScript.isGrabbing || handleScript.isColliding))
        {
            isLocked = true;
            anim.Play("Button_Press_Anim");
            //Check to see if the command is to press the button (rCommand == 0)
            //send command tapped to the Console_Text_Script
            //mastermindScript.TappedWaitForSecondsOrTap(rCommand);
        }
    }
}
