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

    public const int buttonCommand = 0;
    public const int lLeverCommand = 1;
    public const int wLeverCommand = 2;

    // Use this for initialization
    void Start ()
    {
        textMesh = GetComponent<TextMesh>();
        textMesh.text = "";
        StartCoroutine(DisplayStartText());
    }

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
        if (!isDisplayStart && !isTyping && !isDisplayingCommand)
            StartCoroutine(DisplayRandomCommand());
    }

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
            GameObject canvasTextObj = GameObject.Find("Canvas_Text");
            Canvas_Text_Script canvasTextScript = canvasTextObj.GetComponent<Canvas_Text_Script>();
            canvasTextScript.scoreDown();
        }

        //reset isTapped
        isTapped = false;
    }

    // End the waitForSeconds by setting the timer to zero
    public void tappedWaitForSecondsOrTap(int inputCommand)
    {
        isTapped = true;

        //Update score in Canvas
        GameObject canvasTextObj = GameObject.Find("Canvas_Text");
        Canvas_Text_Script canvasTextScript = canvasTextObj.GetComponent<Canvas_Text_Script>();
        //Check to see if the current comman is the correct button pressed. Update score accordingly
        if (rCommand == inputCommand)
        {
            canvasTextScript.scoreUp();
        }
        else
        {
            canvasTextScript.scoreDown();
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
                GameObject L_Lever = GameObject.Find("L_Lever");
                L_Lever_Script lLeverScript = L_Lever.GetComponent<L_Lever_Script>();
                message = "Turn ";
                if(lLeverScript.isLLeverUp)
                {
                    message += "OFF";
                }
                else
                {
                    message += "ON";
                }
                message += " the lights";
                break;
            case wLeverCommand:
                //W_Lever
                GameObject W_Lever = GameObject.Find("W_Lever");
                W_Lever_Script wLeverScript = W_Lever.GetComponent<W_Lever_Script>();
                message = "";
                if(wLeverScript.isWLeverUp)
                {
                    message += "Lower";
                }
                else
                {
                    message += "Raise";
                }
                message += " the shields";
                break;
            default:
                break;
        }

        StartCoroutine(TypeText());

        yield return WaitForSecondsOrTap(10);
        isDisplayingCommand = false;
    }

}
