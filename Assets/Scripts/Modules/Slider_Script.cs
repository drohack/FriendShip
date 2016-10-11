using UnityEngine;
using System.Collections;

public class Slider_Script : Photon.MonoBehaviour
{

    Transform handleTransform;
    private Highlight_Handle_Top_Script handleScript;
    public int sliderPosition;
    
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

        handleTransform.gameObject.AddComponent<ConfigurableJoint>();
        handleTransform.GetComponent<ConfigurableJoint>().axis = new Vector3(1, 0, 0);
        handleTransform.GetComponent<ConfigurableJoint>().connectedBody = transform.Find("Case").GetComponent<Rigidbody>();
        handleTransform.GetComponent<ConfigurableJoint>().xMotion = ConfigurableJointMotion.Locked;
        handleTransform.GetComponent<ConfigurableJoint>().yMotion = ConfigurableJointMotion.Locked;
        handleTransform.GetComponent<ConfigurableJoint>().zMotion = ConfigurableJointMotion.Limited;
        handleTransform.GetComponent<ConfigurableJoint>().angularXMotion = ConfigurableJointMotion.Locked;
        handleTransform.GetComponent<ConfigurableJoint>().angularYMotion = ConfigurableJointMotion.Locked;
        handleTransform.GetComponent<ConfigurableJoint>().angularZMotion = ConfigurableJointMotion.Locked;
        SoftJointLimit softJointLimit = new SoftJointLimit();
        softJointLimit.limit = 0.113f;
        handleTransform.GetComponent<ConfigurableJoint>().linearLimit = softJointLimit;

        //Set Handle to 0
        handleTransform.localPosition = new Vector3(0, 0, -1.6f);
        sliderPosition = 0;
        isLocked = true;
        sendLastStream = true;
        
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
                stream.SendNext(handle.localPosition);
                sendLastStream = false;
            }
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
        else {
            if (handleScript.isGrabbing)
            {
                isLocked = false;
            }
            else
            {
                //snap lever into place near edges (on = handleTransform.localPosition.z == 0; off = handleTransform.localPosition.z == 45)
                if (handleTransform.localPosition.z > 1.066)
                {
                    handleTransform.localPosition = new Vector3(0, 0, 1.6f);

                    if (!isLocked)
                    {
                        sendLastStream = true;
                        isLocked = true;
                        //Lever changed positions
                        sliderPosition = 3;
                        //send command tapped to the Server
                        int rCommandThree = rCommand + 3;
                        photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandThree, sliderPosition);
                    }
                }
                else if (handleTransform.localPosition.z > 0 && handleTransform.localPosition.z < 1.066)
                {
                    handleTransform.localPosition = new Vector3(0, 0, 0.533f);

                    if (!isLocked)
                    {
                        sendLastStream = true;
                        isLocked = true;
                        //Lever changed positions
                        sliderPosition = 2;
                        //send command tapped to the Server
                        int rCommandTwo = rCommand + 2;
                        photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandTwo, sliderPosition);
                    }
                }
                else if (handleTransform.localPosition.z > -1.066 && handleTransform.localPosition.z < 0)
                {
                    handleTransform.localPosition = new Vector3(0, 0, -0.533f);

                    if (!isLocked)
                    {
                        sendLastStream = true;
                        isLocked = true;
                        //Lever changed positions
                        sliderPosition = 1;
                        //send command tapped to the Server
                        int rCommandOne = rCommand + 1;
                        photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandOne, sliderPosition);
                    }
                }
                else if (handleTransform.localPosition.z < -1.066)
                {
                    handleTransform.localPosition = new Vector3(0, 0, -1.6f);

                    if (!isLocked)
                    {
                        sendLastStream = true;
                        isLocked = true;
                        //Lever changed positions
                        sliderPosition = 0;
                        //send command tapped to the Server
                        int rCommandZero = rCommand + 0;
                        photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommandZero, sliderPosition);
                    }
                }
            }
        }
    }

    [PunRPC]
    void CmdSendTappedCommand(int sentRCommand, int sentSliderPosition)
    {
        sliderPosition = sentSliderPosition;
        mastermindScript.TappedWaitForSecondsOrTap(sentRCommand, playerNum);
    }

    [PunRPC]
    void RPCDestroy()
    {
        if (photonView.isMine)
            PhotonNetwork.Destroy(gameObject);
    }
}
