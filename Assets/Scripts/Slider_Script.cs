using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Slider_Script : NetworkBehaviour {

    Transform handleTransform;
    private Highlight_Handle_Top_Script handleScript;
    public int sliderPosition;
    public int rCommand = -1;
    
    private bool isLocked = true;

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
        handleTransform.localPosition = new Vector3(0, 0, -1.6f);
        sliderPosition = 0;
        isLocked = true;
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
                    //send command tapped to the Console_Text_Script with the lLeverUpCommand
                    int rCommandUp = (rCommand * 100) + 3;
                    mastermindScript.TappedWaitForSecondsOrTap(rCommandUp);
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
                    //send command tapped to the Console_Text_Script with the lLeverDownCommand
                    int rCommandDown = (rCommand * 100) + 2;
                    mastermindScript.TappedWaitForSecondsOrTap(rCommandDown);
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
                    //send command tapped to the Console_Text_Script with the lLeverDownCommand
                    int rCommandDown = (rCommand * 100) + 1;
                    mastermindScript.TappedWaitForSecondsOrTap(rCommandDown);
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
                    //send command tapped to the Console_Text_Script with the lLeverDownCommand
                    int rCommandDown = (rCommand * 100) + 0;
                    mastermindScript.TappedWaitForSecondsOrTap(rCommandDown);
                }
            }
        }
    }
}
