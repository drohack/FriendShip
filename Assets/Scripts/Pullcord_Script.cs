using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Pullcord_Script : NetworkBehaviour {

    Transform handleTransform;
    private Highlight_Handle_Top_Script handleScript;
    private ConfigurableJoint handleJoint;

    public int rCommand = -1;
    
    private bool isDown = false;
    
    Mastermind_Script mastermindScript;

    [SyncVar(hook = "UpdateName")]
    public string newName;

    private void UpdateName(string name)
    {
        transform.Find("Labels/Name").GetComponent<TextMesh>().text = name;
    }

    void Start()
    {
        handleTransform = transform.Find("Handle");
        handleScript = handleTransform.GetComponent<Highlight_Handle_Top_Script>();
        handleJoint = handleTransform.GetComponent<ConfigurableJoint>();
        isDown = false;
        mastermindScript = GameObject.Find("Mastermind").GetComponent<Mastermind_Script>();
    }

    private void Update()
    {
        // If you are holding the handle and it is all the way down send the tapped command once
        if (handleScript.isGrabbing && handleTransform.localPosition.y <= -handleJoint.linearLimit.limit && !isDown)
        {
            isDown = true;
            //send command tapped to the Console_Text_Script
            mastermindScript.TappedWaitForSecondsOrTap(rCommand);
        }

        // If not holding the handle and it's at the maximum, set handle just above maximum so it bounces back to the center (it locks at maximum)
        if (!handleScript.isGrabbing && handleTransform.localPosition.y <= -handleJoint.linearLimit.limit)
        {
            handleTransform.localPosition = new Vector3(handleTransform.localPosition.x, -1.99f, handleTransform.localPosition.z);
            isDown = false;
        }
    }
}
