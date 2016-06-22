using UnityEngine;
using System.Collections;

public class Dial_Script : Photon.MonoBehaviour
{

    Transform handleTransform;
    private Highlight_Handle_Top_Script handleScript;
    public int dialPosition;

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

        handleTransform.localPosition = new Vector3(0, 0, 0);

        if(handleScript.isGrabbing)
        {
            isLocked = false;
            handleTransform.localEulerAngles = new Vector3(0, handleTransform.localEulerAngles.y, 0);
        }
        else
        {
            //snap lever into place near edges 
            if (handleTransform.localEulerAngles.y > 162)
            {
                handleTransform.localEulerAngles = new Vector3(0, 179.9f, 0);

                if (!isLocked)
                {
                    isLocked = true;
                    //Lever changed positions
                    dialPosition = 5;
                    //send command tapped to the Server
                    int rCommandFive = (rCommand * 100) + 5;
                    photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandFive, dialPosition);
                }
            }
            else if (handleTransform.localEulerAngles.y > 126 && handleTransform.localEulerAngles.y < 162)
            {
                handleTransform.localEulerAngles = new Vector3(0, 144, 0);

                if (!isLocked)
                {
                    isLocked = true;
                    //Lever changed positions
                    dialPosition = 4;
                    //send command tapped to the Server
                    int rCommandFour = (rCommand * 100) + 4;
                    photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandFour, dialPosition);
                }
            }
            else if (handleTransform.localEulerAngles.y > 90 && handleTransform.localEulerAngles.y < 126)
            {
                handleTransform.localEulerAngles = new Vector3(0, 108, 0);

                if (!isLocked)
                {
                    isLocked = true;
                    //Lever changed positions
                    dialPosition = 3;
                    //send command tapped to the Server
                    int rCommandThree = (rCommand * 100) + 3;
                    photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandThree, dialPosition);
                }
            }
            else if (handleTransform.localEulerAngles.y > 54 && handleTransform.localEulerAngles.y < 90)
            {
                handleTransform.localEulerAngles = new Vector3(0, 72, 0);

                if (!isLocked)
                {
                    isLocked = true;
                    //Lever changed positions
                    dialPosition = 2;
                    //send command tapped to the Server
                    int rCommandTwo = (rCommand * 100) + 2;
                    photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandTwo, dialPosition);
                }
            }
            else if (handleTransform.localEulerAngles.y > 18 && handleTransform.localEulerAngles.y < 54)
            {
                handleTransform.localEulerAngles = new Vector3(0, 36, 0);

                if (!isLocked)
                {
                    isLocked = true;
                    //Lever changed positions
                    dialPosition = 1;
                    //send command tapped to the Server
                    int rCommandOne = (rCommand * 100) + 1;
                    photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandOne, dialPosition);
                }
            }
            else if (handleTransform.localEulerAngles.y < 18)
            {
                handleTransform.localEulerAngles = new Vector3(0, 0, 0);

                if (!isLocked)
                {
                    isLocked = true;
                    //Lever changed positions
                    dialPosition = 0;
                    //send command tapped to the Server
                    int rCommandZero = (rCommand * 100) + 0;
                    photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandZero, dialPosition);
                }
            }
        }
    }

    [PunRPC]
    void CmdSendTappedCommand(int sentRCommand, int sentDialPosition)
    {
        dialPosition = sentDialPosition;
        mastermindScript.TappedWaitForSecondsOrTap(sentRCommand);
    }
}
