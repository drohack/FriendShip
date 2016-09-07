using UnityEngine;
using System.Collections;

public class Button_Script : Photon.MonoBehaviour
{

    private Highlight_Handle_Top_Script handleScript;
    Animator anim;
    public bool isButtonDown = false;
    private bool isAnimating = false;
    private bool isLocked = false;

    Mastermind_Script mastermindScript;

    //Network variables
    public string newName;
    public int rCommand = -1;
    public int playerNum;

    // Use this for initialization
    void Start() {
        //Load Network data
        object[] data = photonView.instantiationData;
        if(data != null)
        {
            newName = transform.Find("Labels/Name").GetComponent<TextMesh>().text = (string)data[0];
            rCommand = (int)data[1];
            playerNum = (int)data[2];
        }

        handleScript = transform.Find("Handle").GetComponent<Highlight_Handle_Top_Script>();
        anim = transform.Find("Handle").GetComponent<Animator>();
        isButtonDown = false;
        isAnimating = false;
        isLocked = false;

        if(PhotonNetwork.isMasterClient)
            mastermindScript = GameObject.Find("Mastermind").GetComponent<Mastermind_Script>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAnimating && isButtonDown && isLocked && !handleScript.isGrabbing && !handleScript.isColliding)
        {
            isLocked = false;
            isButtonDown = false;
            photonView.RPC("RPCPlayAnim", PhotonTargets.Others, "Button_Up_Anim");
            StartCoroutine(WaitForAnimation(anim, "Button_Up_Anim"));
        }

        if (!isAnimating && !isLocked && !isButtonDown && (handleScript.isGrabbing || handleScript.isColliding))
        {
            isLocked = true;
            isButtonDown = true;
            //send tapped rCommand to Server
            photonView.RPC("RPCPlayAnim", PhotonTargets.Others, "Button_Down_Anim");
            StartCoroutine(WaitForAnimation(anim, "Button_Down_Anim"));
            photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommand, playerNum);
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
    void CmdSendTappedCommand(int sentRCommand, int sentPlayerNum)
    {
        mastermindScript.TappedWaitForSecondsOrTap(sentRCommand, sentPlayerNum);
    }

    [PunRPC]
    void RPCPlayAnim(string animationName)
    {
        anim.Play(animationName);
    }
}
