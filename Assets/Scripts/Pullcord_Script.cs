using UnityEngine;
using System.Collections;

public class Pullcord_Script : Photon.MonoBehaviour
{

    Transform handleTransform;
    private Highlight_Handle_Top_Script handleScript;
    private ConfigurableJoint handleJoint;
    
    private bool isDown = false;
    
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
        handleJoint = handleTransform.GetComponent<ConfigurableJoint>();

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

        // If you are holding the handle and it is all the way down send the tapped command once
        if (handleScript.isGrabbing && handleTransform.localPosition.y <= -handleJoint.linearLimit.limit && !isDown)
        {
            isDown = true;
            //send command tapped to the Server
            photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommand, isDown);
        }

        // If not holding the handle and it's at the maximum, set handle just above maximum so it bounces back to the center (it locks at maximum)
        if (!handleScript.isGrabbing && handleTransform.localPosition.y <= -handleJoint.linearLimit.limit)
        {
            handleTransform.localPosition = new Vector3(handleTransform.localPosition.x, -1.99f, handleTransform.localPosition.z);
            isDown = false;
        }
    }

    [PunRPC]
    void CmdSendTappedCommand(int sentRCommand, bool sentIsDown)
    {
        isDown = sentIsDown;
        mastermindScript.TappedWaitForSecondsOrTap(sentRCommand);
    }
}
