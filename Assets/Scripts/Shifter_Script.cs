using UnityEngine;
using System.Collections;

public class Shifter_Script : Photon.MonoBehaviour
{

    private Transform handleTransform;
    private Highlight_Handle_Top_Script handleScript;
    private HingeJoint m_HingeJoint;

    public int shifterPosition;
    
    private bool isLocked = true;

    Mastermind_Script mastermindScript;

    //Network variables
    [SerializeField]
    Transform handle;
    public string newName;
    public int rCommand = -1;
    //Once object is in final resting position make sure to send that final position
    private bool sendLastStream = false;

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
        
        shifterPosition = 1;
        isLocked = true;

        //Add hinge joint to Handle
        handleTransform.gameObject.AddComponent<HingeJoint>();
        handleTransform.GetComponent<HingeJoint>().axis = new Vector3(0, 0, 1);
        handleTransform.GetComponent<HingeJoint>().useLimits = true;
        JointLimits hLimits = new JointLimits();
        hLimits.min = -45;
        hLimits.max = 45;
        handleTransform.GetComponent<HingeJoint>().limits = hLimits;
        m_HingeJoint = handleTransform.GetComponent<HingeJoint>();

        if (PhotonNetwork.isMasterClient)
            mastermindScript = GameObject.Find("Mastermind").GetComponent<Mastermind_Script>();
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
        else
        {
            handleTransform.localPosition = new Vector3(0, 0, 0);

            if (handleScript.isGrabbing)
            {
                //Debug.Log(handleTransform.localEulerAngles.z);
                isLocked = false;
            }
            else
            {
                if (handleTransform.localEulerAngles.z > 22.5 && handleTransform.localEulerAngles.z < 180)
                {
                    handleTransform.localEulerAngles = new Vector3(
                        0f,
                        0f,
                        44.999f
                    );

                    if (!isLocked)
                    {
                        sendLastStream = true;
                        isLocked = true;
                        //Lever changed positions
                        shifterPosition = 2;
                        //send command tapped to the Server
                        int rCommandTwo = (rCommand * 100) + 2;
                        photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandTwo, shifterPosition);
                    }
                }
                else if (handleTransform.localEulerAngles.z > 337.5 || handleTransform.localEulerAngles.z < 22.5)
                {
                    handleTransform.localEulerAngles = new Vector3(
                        0f,
                        0f,
                        0f
                    );

                    if (!isLocked)
                    {
                        sendLastStream = true;
                        isLocked = true;
                        //Lever changed positions
                        shifterPosition = 1;
                        //send command tapped to the Server
                        int rCommandOne = (rCommand * 100) + 1;
                        photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandOne, shifterPosition);
                    }
                }
                else if (handleTransform.localEulerAngles.z < 337.5 && handleTransform.localEulerAngles.z > 180)
                {
                    handleTransform.localEulerAngles = new Vector3(
                        0f,
                        0f,
                        315.001f
                    );

                    if (!isLocked)
                    {
                        sendLastStream = true;
                        isLocked = true;
                        //Lever changed positions
                        shifterPosition = 0;
                        //send command tapped to the Server
                        int rCommandZero = (rCommand * 100) + 0;
                        photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandZero, shifterPosition);
                    }
                }
            }
        }
    }

    [PunRPC]
    void CmdSendTappedCommand(int sentRCommand, int sentShifterPosition)
    {
        shifterPosition = sentShifterPosition;
        mastermindScript.TappedWaitForSecondsOrTap(sentRCommand);
    }
}
