using UnityEngine;
using System.Collections;

public class Pullcord_Script : Photon.MonoBehaviour
{

    Transform handleTransform;
    private Highlight_Handle_Top_Script handleScript;
    private ConfigurableJoint handleJoint;

    public bool isDown = false;
    private float linearLimit = -2;

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
            //newName = transform.Find("Labels/Name").GetComponent<TextMesh>().text = (string)data[0];
            rCommand = (int)data[1];
            playerNum = (int)data[2];
        }

        handleTransform = transform.Find("Handle");
        handleScript = handleTransform.GetComponent<Highlight_Handle_Top_Script>();
        
        isDown = false;

        //Add configurable joint to Handle
        handleTransform.gameObject.AddComponent<ConfigurableJoint>();
        handleTransform.GetComponent<ConfigurableJoint>().axis = new Vector3(1, 0, 0);
        handleTransform.GetComponent<ConfigurableJoint>().xMotion = ConfigurableJointMotion.Locked;
        handleTransform.GetComponent<ConfigurableJoint>().yMotion = ConfigurableJointMotion.Limited;
        handleTransform.GetComponent<ConfigurableJoint>().zMotion = ConfigurableJointMotion.Locked;
        handleTransform.GetComponent<ConfigurableJoint>().angularXMotion = ConfigurableJointMotion.Locked;
        handleTransform.GetComponent<ConfigurableJoint>().angularYMotion = ConfigurableJointMotion.Locked;
        handleTransform.GetComponent<ConfigurableJoint>().angularZMotion = ConfigurableJointMotion.Locked;
        SoftJointLimit softJointLimit = new SoftJointLimit();
        softJointLimit.limit = 0.2f;
        handleTransform.GetComponent<ConfigurableJoint>().linearLimit = softJointLimit;
        JointDrive yDrive = new JointDrive();
        yDrive.maximumForce = 3.402823e+38f;
        yDrive.positionSpring = 20;
        handleTransform.GetComponent<ConfigurableJoint>().yDrive = yDrive;
        handleJoint = handleTransform.GetComponent<ConfigurableJoint>();

        linearLimit = -((handleJoint.linearLimit.limit - 0.000001f) * 10);

        if (PhotonNetwork.isMasterClient)
            mastermindScript = GameObject.Find("Mastermind").GetComponent<Mastermind_Script>();
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(handle.localPosition);
        }
        else
        {
            //Network player, receive data
            handlePos = (Vector3)stream.ReceiveNext();
        }
    }

    private Vector3 handlePos = Vector3.zero; //We lerp towards this

    private void Update()
    {
        if (!photonView.isMine)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            handle.localPosition = Vector3.Lerp(handle.localPosition, handlePos, Time.deltaTime * 20);
        }
        else
        {
            // If you are holding the handle and it is all the way down send the tapped command once
            if (handleScript.isGrabbing && handleTransform.localPosition.y <= linearLimit && !isDown)
            {
                isDown = true;
                //send command tapped to the Server
                photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommand, isDown);
            }
            // Else if not holding the handle and it's at the maximum, set handle just above maximum so it bounces back to the center (it locks at maximum)
            else if (!handleScript.isGrabbing && handleTransform.localPosition.y <= linearLimit)
            {
                handleTransform.localPosition = new Vector3(handleTransform.localPosition.x, -1.99f, handleTransform.localPosition.z);
                isDown = false;
                photonView.RPC("RPCUpdateIsDown", PhotonTargets.Others, isDown);
            }
            // Else if the handle is above the maximum and the last isDown sent was "true" let others know it's now false
            else if (handleTransform.localPosition.y > linearLimit && isDown == true)
            {
                isDown = false;
                photonView.RPC("RPCUpdateIsDown", PhotonTargets.Others, isDown);
            }
        }
    }

    [PunRPC]
    void RPCUpdateIsDown(bool sentIsDown)
    {
        isDown = sentIsDown;
    }

    [PunRPC]
    void CmdSendTappedCommand(int sentRCommand, bool sentIsDown)
    {
        isDown = sentIsDown;
        mastermindScript.TappedWaitForSecondsOrTap(sentRCommand, playerNum);
    }

    [PunRPC]
    void RPCDestroy()
    {
        if (photonView.isMine)
            PhotonNetwork.Destroy(gameObject);
    }
}
