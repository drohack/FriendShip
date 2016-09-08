using UnityEngine;
using System.Collections;

public class Lightswitch_Script : Photon.MonoBehaviour
{

    private Highlight_Handle_Top_Script handleScript;
    Animator anim;
    public bool isLightswitchOn = true;
    private bool isAnimating = false;
    private bool isLocked = false;

    Mastermind_Script mastermindScript;

    //Network variables
    [SerializeField]
    Transform handle;
    public string newName;
    public int rCommand = -1;
    public int playerNum;

    // Use this for initialization
    void Start()
    {
        //Load Network data
        object[] data = photonView.instantiationData;
        if (data != null)
        {
            newName = transform.Find("Labels/Name").GetComponent<TextMesh>().text = (string)data[0];
            rCommand = (int)data[1];
            playerNum = (int)data[2];
        }

        handleScript = transform.Find("Handle").GetComponent<Highlight_Handle_Top_Script>();
        isLightswitchOn = true;
        isAnimating = false;
        isLocked = false;
        anim = transform.Find("Handle").GetComponent<Animator>();

        if (PhotonNetwork.isMasterClient)
            mastermindScript = GameObject.Find("Mastermind").GetComponent<Mastermind_Script>();
    }

    private void Update()
    {
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
                int rCommandUp = (rCommand * 100) + 1;
                photonView.RPC("RPCPlayAnim", PhotonTargets.Others, "Lightswitch_Off_Anim", isLightswitchOn);
                StartCoroutine(WaitForAnimation(anim, "Lightswitch_Off_Anim"));
                photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandUp, isLightswitchOn);
            }
            else
            {
                isLightswitchOn = true;
                //send tapped command to Mastermind
                int rCommandDown = (rCommand * 100) + 2;
                photonView.RPC("RPCPlayAnim", PhotonTargets.Others, "Lightswitch_On_Anim", isLightswitchOn);
                StartCoroutine(WaitForAnimation(anim, "Lightswitch_On_Anim"));
                photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandDown, isLightswitchOn);
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

    [PunRPC]
    void CmdSendTappedCommand(int sentRCommand, bool sentIsLightswitchOn)
    {
        isLightswitchOn = sentIsLightswitchOn;
        mastermindScript.TappedWaitForSecondsOrTap(sentRCommand, playerNum);
    }

    [PunRPC]
    void RPCPlayAnim(string animationName, bool newIsLightswitchOn)
    {
        isLightswitchOn = newIsLightswitchOn;
        anim.Play(animationName);
    }

    [PunRPC]
    void RPCDestroy()
    {
        if (photonView.isMine)
            PhotonNetwork.Destroy(gameObject);
    }
}
