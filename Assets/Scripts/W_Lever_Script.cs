using UnityEngine;
using System.Collections;

public class W_Lever_Script : Photon.MonoBehaviour
{

    Transform handleTransform;
    private Highlight_Handle_Top_Script handleScript;
    private HingeJoint handleJoint;

    public bool isWLeverUp = true;
    private bool isLocked = true;

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
        handleJoint = handleTransform.GetComponent<HingeJoint>();

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
                    //send command tapped to the Server with the lLeverUpCommand
                    int rCommandUp = (rCommand * 100) + 2;
                    photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandUp, isWLeverUp);
                    //Lever changed positions
                    isWLeverUp = true;
                    isLocked = true;
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
                    //send command tapped to the Server with the lLeverDownCommand
                    int rCommandDown = (rCommand * 100) + 1;
                    photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandDown, isWLeverUp);
                    //Lever changed positions
                    isWLeverUp = false;
                    isLocked = true;
                }
            }
        }
    }

    [PunRPC]
    void CmdSendTappedCommand(int sentRCommand, bool sentIsWLeverUp)
    {
        isWLeverUp = sentIsWLeverUp;
        mastermindScript.TappedWaitForSecondsOrTap(sentRCommand);
    }
}
