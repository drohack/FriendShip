using UnityEngine;
using System.Collections;

public class L_Lever_Script : Photon.MonoBehaviour
{

    Transform handleTransform;
    private Highlight_Handle_Top_Script handleScript;

    public bool isLLeverUp = true;
    private bool isLocked = true;

    Mastermind_Script mastermindScript;

    //Network variables
    [SerializeField]
    Transform handle;
    public string newName;
    public int rCommand = -1;
    //Once object is in final resting position make sure to send that final position
    private bool sendLastStream = false;
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

        handleTransform = transform.Find("Handle");
        handleScript = handleTransform.GetComponent<Highlight_Handle_Top_Script>();
        isLLeverUp = true;
        isLocked = true;

        //Add hinge joint to Handle
        handleTransform.gameObject.AddComponent<HingeJoint>();
        handleTransform.GetComponent<HingeJoint>().anchor = new Vector3(0, 0, -1);
        JointLimits hLimits = new JointLimits();
        hLimits.min = -45;
        handleTransform.GetComponent<HingeJoint>().limits = hLimits;
        handleTransform.GetComponent<HingeJoint>().useLimits = true;
        handleTransform.GetComponent<HingeJoint>().axis = new Vector3(0, 1, 0);
        handleTransform.GetComponent<HingeJoint>().connectedBody = transform.Find("Case").GetComponent<Rigidbody>();
        
        mastermindScript = GameObject.FindGameObjectWithTag("Mastermind").GetComponent<Mastermind_Script>();
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // Only send data when the object is in motion (not locked) or it's final resting position (sendLastStream)
            if (!isLocked || sendLastStream)
            {
                //We own this player: send the others our data
                stream.SendNext(handle.localRotation);
                sendLastStream = false;
            }
        }
        else
        {
            //Network player, receive data
            handleRot = (Quaternion)stream.ReceiveNext();
        }
    }
    
    private Quaternion handleRot = Quaternion.identity; //We lerp towards this

    private void Update()
    {
        if (!photonView.isMine)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            handle.localRotation = Quaternion.Slerp(handle.localRotation, handleRot, Time.deltaTime * 20);
        }
        else {

            if (handleScript.isGrabbing)
            {
                isLocked = false;
            }
            else
            {
                if (handleTransform.localEulerAngles.y < 90 || handleTransform.localEulerAngles.y > 337.5)
                {
                    handleTransform.localPosition = new Vector3(0, 0, handleTransform.localPosition.z);
                    handleTransform.localEulerAngles = new Vector3(
                        0f,
                        359.999f,
                        0f
                    );
                    
                    if (!isLocked)
                    {
                        sendLastStream = true;
                        isLocked = true;
                        //Lever changed positions
                        isLLeverUp = true;
                        //send command tapped to the Server with the lLeverUpCommand
                        int rCommandUp = rCommand + 2;
                        photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandUp, isLLeverUp);                        
                    }
                }
                else if (handleTransform.localEulerAngles.y < 337.5 && handleTransform.localEulerAngles.y > 180)
                {
                    handleTransform.localPosition = new Vector3(-0.701f, 0, handleTransform.localPosition.z);
                    handleTransform.localEulerAngles = new Vector3(
                        0f,
                        315f,
                        0f
                    );
                    
                    if (!isLocked)
                    {
                        sendLastStream = true;
                        isLocked = true;
                        //Lever changed positions
                        isLLeverUp = false;
                        //send command tapped to the Server with the lLeverDownCommand
                        int rCommandDown = rCommand + 1;
                        photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandDown, isLLeverUp);                        
                    }
                }
            }
        }
    }

    [PunRPC]
    void CmdSendTappedCommand(int sentRCommand, bool sentIsLeverUp)
    {
        isLLeverUp = sentIsLeverUp;
        mastermindScript.TappedWaitForSecondsOrTap(sentRCommand, playerNum);
    }

    [PunRPC]
    void RPCDestroy()
    {
        if (photonView.isMine)
            PhotonNetwork.Destroy(gameObject);
    }
}
