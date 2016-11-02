using UnityEngine;
using System.Collections;

public class Reset_Button_Script : Photon.MonoBehaviour
{

    private Highlight_Handle_Top_Script handleScript;
    Animator anim;
    public bool isButtonDown = false;
    private bool isAnimating = false;
    private bool isLocked = false;

    Mastermind_Script mastermindScript;
    Abort_Reset_Rotate_Feedback_Script feedbackScript;

    //Network variables
    public string newName;
    public bool resetStatus = false;
    public int playerNum;

    // Use this for initialization
    void Start()
    {
        //Load Network data
        object[] data = photonView.instantiationData;
        if (data != null)
        {
            playerNum = (int)data[0];
        }

        transform.parent = GameObject.Find("Back Panel " + playerNum).transform;

        handleScript = transform.Find("Handle").GetComponent<Highlight_Handle_Top_Script>();
        anim = transform.Find("Handle").GetComponent<Animator>();
        isButtonDown = false;
        isAnimating = false;
        isLocked = false;
        
        mastermindScript = GameObject.FindGameObjectWithTag("Mastermind").GetComponent<Mastermind_Script>();
        feedbackScript = transform.parent.gameObject.GetComponent<Abort_Reset_Rotate_Feedback_Script>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAnimating && isButtonDown && isLocked && !handleScript.isGrabbing && !handleScript.isColliding)
        {
            isLocked = false;
            isButtonDown = false;
            resetStatus = false;
            photonView.RPC("RPCPlayAnim", PhotonTargets.Others, "Button_Up_Anim");
            StartCoroutine(WaitForAnimation(anim, "Button_Up_Anim"));
            if (feedbackScript.isAbortResetFacing)
                photonView.RPC("CmdSendResetCommand", PhotonTargets.MasterClient, resetStatus, playerNum);
        }

        if (!isAnimating && !isLocked && !isButtonDown && (handleScript.isGrabbing || handleScript.isColliding))
        {
            isLocked = true;
            isButtonDown = true;
            resetStatus = true;
            //send tapped rCommand to Server
            photonView.RPC("RPCPlayAnim", PhotonTargets.Others, "Button_Down_Anim");
            StartCoroutine(WaitForAnimation(anim, "Button_Down_Anim"));
            if (feedbackScript.isAbortResetFacing)
                photonView.RPC("CmdSendResetCommand", PhotonTargets.MasterClient, resetStatus, playerNum);
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

    [PunRPC]
    void CmdSendResetCommand(bool sentResetCommand, int sentPlayerNum)
    {
        mastermindScript.ResetCheck(sentResetCommand, sentPlayerNum);
    }

    [PunRPC]
    void RPCPlayAnim(string animationName)
    {
        anim.Play(animationName);
    }

    [PunRPC]
    void RPCDestroy()
    {
        if (photonView.isMine)
            PhotonNetwork.Destroy(gameObject);
    }
}
