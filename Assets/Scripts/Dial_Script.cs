using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Dial_Script : NetworkBehaviour {

    Transform handleTransform;
    private Highlight_Handle_Top_Script handleScript;
    public int dialPosition;
    public int rCommand = -1;

    private bool isLocked = false;

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
        dialPosition = 0;
        isLocked = true;
        mastermindScript = GameObject.Find("Mastermind").GetComponent<Mastermind_Script>();
    }

    private void Update()
    {
        handleTransform.localPosition = new Vector3(0, 0, 0);

        if(handleScript.isGrabbing)
        {
            isLocked = false;
        }
        else
        {
            //snap lever into place near edges 
            if (handleTransform.localEulerAngles.y > 162)
            {
                handleTransform.localEulerAngles = new Vector3(
                    handleTransform.localEulerAngles.x,
                    179.9f,
                    handleTransform.localEulerAngles.z
                );

                if (!isLocked)
                {
                    isLocked = true;
                    //Lever changed positions
                    dialPosition = 5;
                    //send command tapped to the Console_Text_Script with the lLeverUpCommand
                    int rCommandFive = (rCommand * 100) + 5;
                    mastermindScript.TappedWaitForSecondsOrTap(rCommandFive);
                }
            }
            else if (handleTransform.localEulerAngles.y > 126 && handleTransform.localEulerAngles.y < 162)
            {
                handleTransform.localEulerAngles = new Vector3(
                    handleTransform.localEulerAngles.x,
                    144,
                    handleTransform.localEulerAngles.z
                );

                if (!isLocked)
                {
                    isLocked = true;
                    //Lever changed positions
                    dialPosition = 4;
                    //send command tapped to the Console_Text_Script with the lLeverDownCommand
                    int rCommandFour = (rCommand * 100) + 4;
                    mastermindScript.TappedWaitForSecondsOrTap(rCommandFour);
                }
            }
            else if (handleTransform.localEulerAngles.y > 90 && handleTransform.localEulerAngles.y < 126)
            {
                handleTransform.localEulerAngles = new Vector3(
                    handleTransform.localEulerAngles.x,
                    108,
                    handleTransform.localEulerAngles.z
                );

                if (!isLocked)
                {
                    isLocked = true;
                    //Lever changed positions
                    dialPosition = 3;
                    //send command tapped to the Console_Text_Script with the lLeverDownCommand
                    int rCommandThree = (rCommand * 100) + 3;
                    mastermindScript.TappedWaitForSecondsOrTap(rCommandThree);
                }
            }
            else if (handleTransform.localEulerAngles.y > 54 && handleTransform.localEulerAngles.y < 90)
            {
                handleTransform.localEulerAngles = new Vector3(
                    handleTransform.localEulerAngles.x,
                    72,
                    handleTransform.localEulerAngles.z
                );

                if (!isLocked)
                {
                    isLocked = true;
                    //Lever changed positions
                    dialPosition = 2;
                    //send command tapped to the Console_Text_Script with the lLeverDownCommand
                    int rCommandTwo = (rCommand * 100) + 2;
                    mastermindScript.TappedWaitForSecondsOrTap(rCommandTwo);
                }
            }
            else if (handleTransform.localEulerAngles.y > 18 && handleTransform.localEulerAngles.y < 54)
            {
                handleTransform.localEulerAngles = new Vector3(
                    handleTransform.localEulerAngles.x,
                    36,
                    handleTransform.localEulerAngles.z
                );

                if (!isLocked)
                {
                    isLocked = true;
                    //Lever changed positions
                    dialPosition = 1;
                    //send command tapped to the Console_Text_Script with the lLeverDownCommand
                    int rCommandOne = (rCommand * 100) + 1;
                    mastermindScript.TappedWaitForSecondsOrTap(rCommandOne);
                }
            }
            else if (handleTransform.localEulerAngles.y < 18)
            {
                handleTransform.localEulerAngles = new Vector3(
                    handleTransform.localEulerAngles.x,
                    0,
                    handleTransform.localEulerAngles.z
                );

                if (!isLocked)
                {
                    isLocked = true;
                    //Lever changed positions
                    dialPosition = 0;
                    //send command tapped to the Console_Text_Script with the lLeverDownCommand
                    int rCommandZero = (rCommand * 100) + 0;
                    mastermindScript.TappedWaitForSecondsOrTap(rCommandZero);
                }
            }
        }
    }
}
