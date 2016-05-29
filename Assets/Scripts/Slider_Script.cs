using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Slider_Script : NetworkBehaviour {

    Transform handleTransform;
    private Highlight_Handle_Top_Script handleScript;
    public int sliderPosition;
    
    private bool isLocked = true;

    Mastermind_Script mastermindScript;

    //Network variables
    [SyncVar(hook = "UpdateQuaternion")]
    public Quaternion newQuaternion;
    [SyncVar(hook = "UpdateName")]
    public string newName;
    [SyncVar(hook = "UpdateRCommand")]
    public int rCommand = -1;

    private void UpdateQuaternion(Quaternion newQuaternion)
    {
        transform.rotation = newQuaternion;
    }
    private void UpdateName(string name)
    {
        transform.Find("Labels/Name").GetComponent<TextMesh>().text = name;
    }
    private void UpdateRCommand(int command)
    {
        rCommand = command;
    }

    void Start()
    {
        handleTransform = transform.Find("Handle");
        handleScript = handleTransform.GetComponent<Highlight_Handle_Top_Script>();

        //Add configurable joint to Handle
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
        softJointLimit.limit = 1.13f;
        handleTransform.GetComponent<ConfigurableJoint>().linearLimit = softJointLimit;

        //Set Handle to 0
        handleTransform.localPosition = new Vector3(0, 0, -1.6f);
        sliderPosition = 0;
        isLocked = true;

        if (isServer)
            mastermindScript = GameObject.Find("Mastermind").GetComponent<Mastermind_Script>();
    }

    private void Update()
    {
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
                    isLocked = true;
                    //Lever changed positions
                    sliderPosition = 3;
                    //send command tapped to the Server
                    int rCommandThree = (rCommand * 100) + 3;
                    CmdSendTappedCommand(rCommandThree, sliderPosition);
                }
            }
            else if (handleTransform.localPosition.z > 0 && handleTransform.localPosition.z < 1.066)
            {
                handleTransform.localPosition = new Vector3(0, 0, 0.533f);

                if (!isLocked)
                {
                    isLocked = true;
                    //Lever changed positions
                    sliderPosition = 2;
                    //send command tapped to the Server
                    int rCommandTwo = (rCommand * 100) + 2;
                    CmdSendTappedCommand(rCommandTwo, sliderPosition);
                }
            }
            else if (handleTransform.localPosition.z > -1.066 && handleTransform.localPosition.z < 0)
            {
                handleTransform.localPosition = new Vector3(0,  0, -0.533f);

                if (!isLocked)
                {
                    isLocked = true;
                    //Lever changed positions
                    sliderPosition = 1;
                    //send command tapped to the Server
                    int rCommandOne = (rCommand * 100) + 1;
                    CmdSendTappedCommand(rCommandOne, sliderPosition);
                }
            }
            else if (handleTransform.localPosition.z < -1.066)
            {
                handleTransform.localPosition = new Vector3(0, 0, -1.6f);

                if (!isLocked)
                {
                    isLocked = true;
                    //Lever changed positions
                    sliderPosition = 0;
                    //send command tapped to the Server
                    int rCommandZero = (rCommand * 100) + 0;
                    CmdSendTappedCommand(rCommandZero, sliderPosition);
                }
            }
        }
    }

    [Command]
    void CmdSendTappedCommand(int sentRCommand, int sentSliderPosition)
    {
        sliderPosition = sentSliderPosition;
        mastermindScript.TappedWaitForSecondsOrTap(sentRCommand);
    }
}
