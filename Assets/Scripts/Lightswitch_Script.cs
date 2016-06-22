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

    // Use this for initialization
    void Start()
    {
        //Load Network data
        object[] data = photonView.instantiationData;
        if (data != null)
        {
            newName = transform.Find("Labels/Name").GetComponent<TextMesh>().text = (string)data[0];
            rCommand = (int)data[1];
        }

        handleScript = transform.Find("Handle").GetComponent<Highlight_Handle_Top_Script>();
        isLightswitchOn = true;
        isAnimating = false;
        isLocked = false;
        anim = transform.Find("Handle").GetComponent<Animator>();

        if (PhotonNetwork.isMasterClient)
            mastermindScript = GameObject.Find("Mastermind").GetComponent<Mastermind_Script>();
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(handle.position);
            stream.SendNext(handle.rotation);
        }
        else
        {
            //Network player, receive data
            handlePos = (Vector3)stream.ReceiveNext();
            handleRot = (Quaternion)stream.ReceiveNext();
        }
    }

    private Vector3 handlePos = Vector3.zero; //We lerp towards this
    private Quaternion handleRot = Quaternion.identity; //We lerp towards this

    private void Update()
    {
        if (!photonView.isMine)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            handle.position = Vector3.Lerp(handle.position, handlePos, Time.deltaTime * 20);
            handle.rotation = Quaternion.Lerp(handle.rotation, handleRot, Time.deltaTime * 20);
        }

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
                photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandUp, isLightswitchOn);
                StartCoroutine(WaitForAnimation(anim, "Lightswitch_Off_Anim"));
            }
            else
            {
                isLightswitchOn = true;
                //send tapped command to Mastermind
                int rCommandDown = (rCommand * 100) + 2;
                photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandDown, isLightswitchOn);
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

    [PunRPC]
    void CmdSendTappedCommand(int sentRCommand, bool sentIsLightswitchOn)
    {
        isLightswitchOn = sentIsLightswitchOn;
        mastermindScript.TappedWaitForSecondsOrTap(sentRCommand);
    }
}
