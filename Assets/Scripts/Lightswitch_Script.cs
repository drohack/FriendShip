using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Lightswitch_Script : NetworkBehaviour {

    private Highlight_Handle_Top_Script handleScript;
    Animator anim;
    public bool isLightswitchOn = true;
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
        isLightswitchOn = true;
        isLocked = false;
        anim = transform.Find("Handle").GetComponent<Animator>();
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
                int rCommandUp = (rCommand * 100) + 2;
                mastermindScript.TappedWaitForSecondsOrTap(rCommandUp);
            }
            else
            {
                isLightswitchOn = true;
                anim.Play("Lightswitch_On_Anim");
                //send tapped command to Mastermind
                int rCommandDown = (rCommand * 100) + 1;
                mastermindScript.TappedWaitForSecondsOrTap(rCommandDown);
            }
        }
    }
}
