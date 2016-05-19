using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Console_Text_Script : MonoBehaviour {

    public string message = "";
    public int rCommand = -1;

    private bool isDisplayStart = true;
    private bool isTyping = false;
    public bool isDisplayingCommand = false;

    private float __gWaitSystem;
    private bool isTapped = false;
    private TextMesh textMesh;

    private int commandTimeoutSeconds = 10;

    public const int buttonCommand = 0;
    public const int lLeverCommand = 1;
    public const int lLeverUpCommand = lLeverCommand * 10 + 1;
    public const int lLeverDownCommand = lLeverCommand * 10 + 2;
    public const int wLeverCommand = 2;
    public const int wLeverUpCommand = wLeverCommand * 10 + 1;
    public const int wLeverDownCommand = wLeverCommand * 10 + 2;

    // Use this for initialization
    void Start ()
    {
        textMesh = GetComponent<TextMesh>();
        textMesh.text = "";
        StartCoroutine(DisplayStartText());
    }

    //Display "START!" for 2 seconds
    IEnumerator DisplayStartText()
    {
        isDisplayStart = true;
        message = "START!";
        StartCoroutine(TypeText());
        yield return new WaitForSeconds(2);
        isDisplayStart = false;
    }

    void Update()
    {
        //if we are NOT typing "START!" including waiting the 2 seconds
        //AND if we are NOT currently typing a command
        //AND if we are NOT currently waiting the 10 seconds for a command to pass
        //generate and display a new random command
        if (!isDisplayStart && !isTyping && !isDisplayingCommand)
            StartCoroutine(DisplayRandomCommand());
    }

    //Type out the text that is loaded into the "message" variable
    IEnumerator TypeText()
    {
        isTyping = true;
        foreach (char letter in message.ToCharArray())
        {
            textMesh.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        isTyping = false;
    }

    // Custom WaitForSeconds
    IEnumerator WaitForSecondsOrTap(float seconds)
    {
        __gWaitSystem = seconds;
        while (__gWaitSystem > 0.0)
        {
            __gWaitSystem -= Time.deltaTime;
            yield return 0;
        }

        //lower score if time reached (button was not tapped)
        if (!isTapped)
        {
            GameObject scoreTextObj = GameObject.Find("Score_Text");
            Score_Text_Script scoreTextScript = scoreTextObj.GetComponent<Score_Text_Script>();
            scoreTextScript.scoreDown();
        }

        //reset isTapped
        isTapped = false;
    }

    // End the waitForSeconds by setting the timer to zero AND signal that a button was tapped
    public void tappedWaitForSecondsOrTap(int inputCommand)
    {
        isTapped = true;

        //Update score in Canvas
        GameObject scoreTextObj = GameObject.Find("Score_Text");
        Score_Text_Script scoreTextScript = scoreTextObj.GetComponent<Score_Text_Script>();
        //Check to see if the current command is the correct button pressed. Update score accordingly
        if (rCommand == inputCommand)
        {
            scoreTextScript.scoreUp();
        }
        else
        {
            scoreTextScript.scoreDown();
        }

        //Set timer to 0 to get next command (Always)
        __gWaitSystem = 0.0f;
    }

    IEnumerator DisplayRandomCommand()
    {
        isDisplayingCommand = true;

        //Clear text
        GetComponent<TextMesh>().text = "";
        yield return 0;
        yield return new WaitForSeconds(1);

        //Roll random number to decide new command (button, l_lever, w_lever)
        rCommand = Random.Range(0, 3);

        //Get new command
        switch (rCommand)
        {
            case buttonCommand:
                //Button
                message = "Fire Lasers!";
                break;
            case lLeverCommand:
                //L_Lever
                GameObject L_Lever_Handle = GameObject.Find("L_Lever/Handle");
                L_Lever_Handle_Script lLeverHandleScript = L_Lever_Handle.GetComponent<L_Lever_Handle_Script>();
                message = "Turn ";
                if(lLeverHandleScript.isLLeverUp)
                {
                    message += "OFF";
                    rCommand = lLeverDownCommand;
                }
                else
                {
                    message += "ON";
                    rCommand = lLeverUpCommand;
                }
                message += " the lights";
                break;
            case wLeverCommand:
                //W_Lever
                GameObject W_Lever_Handle = GameObject.Find("W_Lever/Handle");
                W_Lever_Handle_Script wLeverHandleScript = W_Lever_Handle.GetComponent<W_Lever_Handle_Script>();
                message = "";
                if(wLeverHandleScript.isWLeverUp)
                {
                    message += "Lower";
                    rCommand = wLeverDownCommand;
                }
                else
                {
                    message += "Raise";
                    rCommand = wLeverUpCommand;
                }
                message += " the shields";
                break;
            default:
                break;
        }

        //Type out new command to console
        StartCoroutine(TypeText());

        //Wait for the commandTimeoutSeconds or if a button gets tapped
        yield return WaitForSecondsOrTap(commandTimeoutSeconds);
        isDisplayingCommand = false;
    }

}
