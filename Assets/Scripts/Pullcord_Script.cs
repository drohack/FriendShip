using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Pullcord_Script : NetworkBehaviour {

    Transform handleTransform;
    private Highlight_Handle_Top_Script handleScript;
    private ConfigurableJoint handleJoint;
    
    private bool isDown = false;
    
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

        if (isServer)
            mastermindScript = GameObject.Find("Mastermind").GetComponent<Mastermind_Script>();
    }

    private void Update()
    {
        // If you are holding the handle and it is all the way down send the tapped command once
        if (handleScript.isGrabbing && handleTransform.localPosition.y <= -handleJoint.linearLimit.limit && !isDown)
        {
            isDown = true;
            //send command tapped to the Server
            CmdSendTappedCommand(rCommand, isDown);
        }

        // If not holding the handle and it's at the maximum, set handle just above maximum so it bounces back to the center (it locks at maximum)
        if (!handleScript.isGrabbing && handleTransform.localPosition.y <= -handleJoint.linearLimit.limit)
        {
            handleTransform.localPosition = new Vector3(handleTransform.localPosition.x, -1.99f, handleTransform.localPosition.z);
            isDown = false;
        }
    }

    [Command]
    void CmdSendTappedCommand(int sentRCommand, bool sentIsDown)
    {
        isDown = sentIsDown;
        mastermindScript.TappedWaitForSecondsOrTap(sentRCommand);
    }
}
