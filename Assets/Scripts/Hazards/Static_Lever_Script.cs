using UnityEngine;
using System.Collections;

public class Static_Lever_Script : Photon.MonoBehaviour
{

    Transform handleTransform;
    private Highlight_Handle_Top_Script handleScript;

    public bool isWLeverUp = true;
    private bool isLocked = true;

    Mastermind_Script mastermindScript;

    //Network variables
    [SerializeField]
    Transform handle;
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
            playerNum = (int)data[0];
        }

        transform.parent = GameObject.Find("Back Panel " + playerNum).transform;

        handleTransform = transform.Find("Handle");
        handleScript = handleTransform.GetComponent<Highlight_Handle_Top_Script>();
        isWLeverUp = true;
        isLocked = true;

        //Add hinge joint to Handle
        handleTransform.gameObject.AddComponent<HingeJoint>();
        handleTransform.GetComponent<HingeJoint>().anchor = new Vector3(0, 0, -1);
        handleTransform.GetComponent<HingeJoint>().axis = new Vector3(0, 1, 0);
        handleTransform.GetComponent<HingeJoint>().useLimits = true;
        JointLimits hLimits = new JointLimits();
        hLimits.min = -35;
        handleTransform.GetComponent<HingeJoint>().limits = hLimits;
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
                        //send command tapped to the Server with the lLeverUpCommand
                        photonView.RPC("CmdTurnOffStatic", PhotonTargets.MasterClient, isWLeverUp);
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
                    }
                }
            }
        }
    }

    [PunRPC]
    void CmdTurnOffStatic(bool sentIsWLeverUp)
    {
        isWLeverUp = sentIsWLeverUp;
        if (isWLeverUp)
            mastermindScript.photonView.RPC("TurnOffStatic", PhotonTargets.All, null);
    }

    [PunRPC]
    void RPCRaiseHandle()
    {
        isWLeverUp = true;
        if (photonView.isMine && !handleScript.isGrabbing)
        {
            handleTransform.localPosition = new Vector3(0, 0, handleTransform.localPosition.z);
            handleTransform.localEulerAngles = new Vector3(
                0f,
                359.999f,
                0f
            );
        }
    }

    [PunRPC]
    void RPCLowerHandle()
    {
        isWLeverUp = false;
        if (photonView.isMine && !handleScript.isGrabbing)
        {
            handleTransform.localPosition = new Vector3(-0.57f, 0, handleTransform.localPosition.z);
            handleTransform.localEulerAngles = new Vector3(
                0f,
                325.001f,
                0f
            );
        }
    }

    [PunRPC]
    void RPCDestroy()
    {
        if (photonView.isMine)
            PhotonNetwork.Destroy(gameObject);
    }
}
