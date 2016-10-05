using UnityEngine;
using System.Collections;

public class Ready_Lever_Script : Photon.MonoBehaviour {

    [SerializeField]
    Transform handleTransform;
    [SerializeField]
    Highlight_Handle_Top_Script handleScript;

    PhotonLobbyRoom photonLobbyRoomScript;

    public bool isWLeverUp = false;
    private bool isLocked = true;

    //Network variables
    public int playerPosition;
    //Once object is in final resting position make sure to send that final position
    private bool sendLastStream = false;

    // Use this for initialization
    void Start () {
        object[] data = photonView.instantiationData;
        if (data != null)
        {
            playerPosition = (int)data[0];
        }

        photonLobbyRoomScript = GameObject.Find("LobbyRoom").GetComponent<PhotonLobbyRoom>();

        //Add hinge joint to Handle
        handleTransform.gameObject.AddComponent<HingeJoint>();
        handleTransform.GetComponent<HingeJoint>().anchor = new Vector3(0, 0, -1);
        handleTransform.GetComponent<HingeJoint>().axis = new Vector3(0, 1, 0);
        handleTransform.GetComponent<HingeJoint>().useLimits = true;
        JointLimits hLimits = new JointLimits();
        hLimits.min = -35;
        handleTransform.GetComponent<HingeJoint>().limits = hLimits;
        handleTransform.GetComponent<HingeJoint>().connectedBody = transform.Find("Case").GetComponent<Rigidbody>();

        photonView.RPC("RPCLowerReadyLever", PhotonTargets.All);
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // Only send data when the object is in motion (not locked) or it's final resting position (sendLastStream)
            if (!isLocked || sendLastStream)
            {
                //We own this player: send the others our data
                stream.SendNext(handleTransform.localRotation);
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

    // Update is called once per frame
    void Update () {
        if (!photonView.isMine)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            handleTransform.localRotation = Quaternion.Slerp(handleTransform.localRotation, handleRot, Time.deltaTime * 20);
        }
        else {
            if (handleScript.isGrabbing)
            {
                isLocked = false;
            }
            else
            {
                if (handleTransform.localEulerAngles.y < 90 || handleTransform.localEulerAngles.y > 342.5)
                {
                    handleTransform.localPosition = new Vector3(0, 0, handleTransform.localPosition.z);
                    handleTransform.localEulerAngles = new Vector3(
                        0f,
                        359.999f,
                        0f
                    );

                    //If the last position of the handle was in the middle, and now we are at the up position, then send the command that the L_Lever is now Up
                    if (!isLocked)
                    {
                        sendLastStream = true;
                        isLocked = true;
                        //Lever changed positions
                        isWLeverUp = true;
                        //send ready command to the Server
                        photonView.RPC("CmdSendReadyCommand", PhotonTargets.MasterClient, isWLeverUp, playerPosition);
                    }
                }
                else if (handleTransform.localEulerAngles.y < 342.5 && handleTransform.localEulerAngles.y > 180)
                {
                    handleTransform.localPosition = new Vector3(-0.57f, 0, handleTransform.localPosition.z);
                    handleTransform.localEulerAngles = new Vector3(
                        0f,
                        325.001f,
                        0f
                    );

                    //If the last position of the handle was in the middle, and now we are at the down position, then send the command that the L_Lever is now Down
                    if (!isLocked)
                    {
                        sendLastStream = true;
                        isLocked = true;
                        //Lever changed positions
                        isWLeverUp = false;
                        //send ready command to the Server
                        photonView.RPC("CmdSendReadyCommand", PhotonTargets.MasterClient, isWLeverUp, playerPosition);
                    }
                }
            }
        }
    }

    [PunRPC]
    void CmdSendReadyCommand(bool sentIsReady, int sentPlayerPosition)
    {
        photonLobbyRoomScript.SendReadyCommand(sentIsReady, sentPlayerPosition);
    }

    [PunRPC]
    void RPCLowerReadyLever()
    {
        if(photonView.isMine)
        {
            isWLeverUp = false;
            handleTransform.localPosition = new Vector3(-0.57f, 0, handleTransform.localPosition.z);
            handleTransform.localEulerAngles = new Vector3(
                0f,
                325.001f,
                0f
            );

            photonView.RPC("CmdSendReadyCommand", PhotonTargets.MasterClient, isWLeverUp, playerPosition);

            sendLastStream = true;
        }
    }

    [PunRPC]
    void RPCDestroy()
    {
        if (photonView.isMine)
            PhotonNetwork.Destroy(gameObject);
    }
}
