using UnityEngine;
using System.Collections;

public class Lightswitch_Script_copy : MonoBehaviour {

    private Highlight_Handle_Top_Script handleScript;
    Animator anim;
    public bool isLightswitchOn = true;
    private bool isAnimating = false;
    private bool isLocked = false;

    //Mastermind_Script mastermindScript;

    ////Network variables
    //[SyncVar(hook = "UpdateQuaternion")]
    //public Quaternion newQuaternion;
    //[SyncVar(hook = "UpdateName")]
    //public string newName;
    //[SyncVar(hook = "UpdateRCommand")]
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
        isAnimating = false;
        isLocked = false;
        anim = transform.Find("Handle").GetComponent<Animator>();

        //if(isServer)
        //    mastermindScript = GameObject.Find("Mastermind").GetComponent<Mastermind_Script>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!isAnimating && isLocked && !handleScript.isGrabbing && !handleScript.isColliding)
        {
            isLocked = false;
        }

        if (!isAnimating && !isLocked && (handleScript.isGrabbing || handleScript.isColliding))
        {
            isLocked = true;
            if (isLightswitchOn)
            {
                isLightswitchOn = false;
                //send tapped command to Mastermind
                //int rCommandUp = (rCommand * 100) + 1;
                //CmdSendTappedCommand(rCommandUp, isLightswitchOn);
                StartCoroutine(WaitForAnimation(anim, "Lightswitch_Off_Anim"));
            }
            else
            {
                isLightswitchOn = true;
                //send tapped command to Mastermind
                //int rCommandDown = (rCommand * 100) + 2;
                //CmdSendTappedCommand(rCommandDown, isLightswitchOn);
                StartCoroutine(WaitForAnimation(anim, "Lightswitch_On_Anim"));
            }
        }
    }

    private IEnumerator WaitForAnimation(Animator animation, string animationName)
    {
        isAnimating = true;
        animation.Play(animationName);
        do
        {
            yield return null;
        } while (animation.GetCurrentAnimatorStateInfo(0).IsName(animationName) && !animation.IsInTransition(0));

        isAnimating = false;
    }

    //[Command]
    //void CmdSendTappedCommand(int sentRCommand, bool sentIsLightswitchOn)
    //{
    //    isLightswitchOn = sentIsLightswitchOn;
    //    mastermindScript.TappedWaitForSecondsOrTap(sentRCommand);
    //}
}
