using UnityEngine;
using System.Collections;

public class Dial_Script : Photon.MonoBehaviour
{

    Transform handleTransform;
    private Highlight_Handle_Top_Script handleScript;
    public int dialPosition;
    Vector3 localEulerAngles;

    private bool isLocked = false;

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
        dialPosition = 0;
        isLocked = true;

        //Add hinge joint to Handle
        handleTransform.gameObject.AddComponent<HingeJoint>();
        handleTransform.GetComponent<HingeJoint>().axis = new Vector3(0, 1, 0);
        handleTransform.GetComponent<HingeJoint>().useLimits = true;
        JointLimits hLimits = new JointLimits();
        hLimits.min = -90;
        hLimits.max = 90;
        handleTransform.GetComponent<HingeJoint>().limits = hLimits;

        handleTransform.localEulerAngles = new Vector3(0, 0, 0);
        
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
            handleTransform.localPosition = new Vector3(0, 0, 0);

            if (handleScript.isGrabbing)
            {
                isLocked = false;
                handleTransform.localEulerAngles = new Vector3(0, handleTransform.localEulerAngles.y, 0);
            }
            else
            {
                localEulerAngles = handleTransform.localEulerAngles;
                //snap lever into place near edges 
                if (localEulerAngles.y > 162)
                {
                    handleTransform.localEulerAngles = new Vector3(0, 179.9f, 0);

                    if (!isLocked)
                    {
                        sendLastStream = true;
                        isLocked = true;
                        //Lever changed positions
                        dialPosition = 5;
                        //send command tapped to the Server
                        int rCommandFive = rCommand + 5;
                        photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandFive, dialPosition);
                    }
                }
                else if (localEulerAngles.y > 126 && localEulerAngles.y < 162)
                {
                    handleTransform.localEulerAngles = new Vector3(0, 144, 0);

                    if (!isLocked)
                    {
                        sendLastStream = true;
                        isLocked = true;
                        //Lever changed positions
                        dialPosition = 4;
                        //send command tapped to the Server
                        int rCommandFour = rCommand + 4;
                        photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandFour, dialPosition);
                    }
                }
                else if (localEulerAngles.y > 90 && localEulerAngles.y < 126)
                {
                    handleTransform.localEulerAngles = new Vector3(0, 108, 0);

                    if (!isLocked)
                    {
                        sendLastStream = true;
                        isLocked = true;
                        //Lever changed positions
                        dialPosition = 3;
                        //send command tapped to the Server
                        int rCommandThree = rCommand + 3;
                        photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandThree, dialPosition);
                    }
                }
                else if (localEulerAngles.y > 54 && localEulerAngles.y < 90)
                {
                    handleTransform.localEulerAngles = new Vector3(0, 72, 0);

                    if (!isLocked)
                    {
                        sendLastStream = true;
                        isLocked = true;
                        //Lever changed positions
                        dialPosition = 2;
                        //send command tapped to the Server
                        int rCommandTwo = rCommand + 2;
                        photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandTwo, dialPosition);
                    }
                }
                else if (localEulerAngles.y > 18 && localEulerAngles.y < 54)
                {
                    handleTransform.localEulerAngles = new Vector3(0, 36, 0);

                    if (!isLocked)
                    {
                        sendLastStream = true;
                        isLocked = true;
                        //Lever changed positions
                        dialPosition = 1;
                        //send command tapped to the Server
                        int rCommandOne = rCommand + 1;
                        photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandOne, dialPosition);
                    }
                }
                else if (localEulerAngles.y < 18)
                {
                    handleTransform.localEulerAngles = new Vector3(0, 0, 0);

                    if (!isLocked)
                    {
                        sendLastStream = true;
                        isLocked = true;
                        //Lever changed positions
                        dialPosition = 0;
                        //send command tapped to the Server
                        int rCommandZero = rCommand + 0;
                        photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandZero, dialPosition);
                    }
                }
            }
        }
    }

    [PunRPC]
    void CmdSendTappedCommand(int sentRCommand, int sentDialPosition)
    {
        dialPosition = sentDialPosition;
        mastermindScript.TappedWaitForSecondsOrTap(sentRCommand, playerNum);
    }

    [PunRPC]
    void RPCDestroy()
    {
        if (photonView.isMine)
            PhotonNetwork.Destroy(gameObject);
    }
}
