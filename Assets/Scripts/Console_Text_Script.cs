using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Console_Text_Script : MonoBehaviour {

    private TextMesh textMesh;
    public string message = "";
    public int rCommand = -1;

    private bool isDisplayStart = true;
    private bool isTyping = false;
    public bool isDisplayingCommand = false;

    // Variables for the custom WaitForSeconds function
    private float __gWaitSystem;
    private bool isTapped = false;

    // The number of different type of game objects total to be used for random rolling of said game objects
    private int numOfDiffGameObjects = 3;

    // The grid which the random game objects get placed
    private GameObject[,] grid;
    private int gridX = 3;
    private int gridY = 3;

    private int commandTimeoutSeconds = 10;

    public const int buttonCommand = 0;
    public const int lLeverCommand = 1;
    public const int wLeverCommand = 2;

    private const string button1Text = "Button1";
    private const string button2Text = "Button2";
    private const string button3Text = "Button3";
    private const string button4Text = "Button4";
    private const string button5Text = "Button5";
    private const string button6Text = "Button6";
    private const string button7Text = "Button7";
    private const string button8Text = "Button8";
    private const string button9Text = "Button9";
    private const string lLever1Text = "L 1";
    private const string lLever2Text = "L 2";
    private const string lLever3Text = "L 3";
    private const string lLever4Text = "L 4";
    private const string lLever5Text = "L 5";
    private const string lLever6Text = "L 6";
    private const string lLever7Text = "L 7";
    private const string lLever8Text = "L 8";
    private const string lLever9Text = "L 9";
    private const string wLever1Text = "W 1";
    private const string wLever2Text = "W 2";
    private const string wLever3Text = "W 3";
    private const string wLever4Text = "W 4";
    private const string wLever5Text = "W 5";
    private const string wLever6Text = "W 6";
    private const string wLever7Text = "W 7";
    private const string wLever8Text = "W 8";
    private const string wLever9Text = "W 9";

    private ArrayList buttonCommandArray = new ArrayList { button1Text, button2Text, button3Text, button4Text, button5Text, button6Text, button7Text, button8Text, button9Text };
    private ArrayList lLeverCommandArray = new ArrayList { lLever1Text, lLever2Text, lLever3Text, lLever4Text, lLever5Text, lLever6Text, lLever7Text, lLever8Text, lLever9Text };
    private ArrayList wLeverCommandArray = new ArrayList { wLever1Text, wLever2Text, wLever3Text, wLever4Text, wLever5Text, wLever6Text, wLever7Text, wLever8Text, wLever9Text };

    // Use this for initialization
    void Start ()
    {
        textMesh = GetComponent<TextMesh>();
        textMesh.text = "";

        //Create a grid to hold all of the random game objects
        grid = new GameObject[gridX,gridY];
        GameObject button = GameObject.Find("Button");
        GameObject lLever = GameObject.Find("L_Lever");
        GameObject wLever = GameObject.Find("W_Lever");
        int commandIndex;
        string newCommandText;

        //for each grid position generate a random object and add it to the grid
        for (int x=0; x<gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                GameObject randObject;
                //roll for a random game object
                int objNum = Random.Range(0, numOfDiffGameObjects);

                //for the given random game object create a copy of it to randObject
                switch (objNum)
                {
                    case buttonCommand:
                        //roll for a random Button command from the buttonCommandArray
                        commandIndex = Random.Range(0, buttonCommandArray.Count);
                        newCommandText = (string)buttonCommandArray[commandIndex];
                        //remove selected button command from buttonCommandArray so it won't be used again
                        buttonCommandArray.RemoveAt(commandIndex);
                        //copy randomObject from the default wLever
                        randObject = (GameObject)Instantiate(button, new Vector3(-3 + (3 * x), 3 + (4 * y), button.transform.position.z), button.transform.rotation);
                        //add new command text to the new randomObject
                        randObject.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>().text = newCommandText;
                        //set randObject's rCommand in it's Script
                        randObject.GetComponent<Button_Press_Script>().rCommand = (x * gridX) + y;
                        break;
                    case lLeverCommand:
                        //roll for a random Button command from the lLeverCommandArray
                        commandIndex = Random.Range(0, lLeverCommandArray.Count);
                        newCommandText = (string)lLeverCommandArray[commandIndex];
                        //remove selected button command from lLeverCommandArray so it won't be used again
                        lLeverCommandArray.RemoveAt(commandIndex);
                        //copy randomObject from the default wLever
                        randObject = (GameObject)Instantiate(lLever, new Vector3(-3 + (3 * x), 3 + (4 * y), lLever.transform.position.z), lLever.transform.rotation);
                        //add new command text to the new randomObject
                        randObject.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>().text = newCommandText;
                        //set randObject's rCommand in it's Script
                        randObject.transform.GetChild(2).GetComponent<L_Lever_Handle_Script>().rCommand = (x * gridX) + y;
                        break;
                    case wLeverCommand:
                        //roll for a random Button command from the wLeverCommandArray
                        commandIndex = Random.Range(0, wLeverCommandArray.Count);
                        newCommandText = (string)wLeverCommandArray[commandIndex];
                        //remove selected button command from wLeverCommandArray so it won't be used again
                        wLeverCommandArray.RemoveAt(commandIndex);
                        //copy randomObject from the default wLever
                        randObject = (GameObject)Instantiate(wLever, new Vector3(-3 + (3 * x), 3 + (4 * y), wLever.transform.position.z), wLever.transform.rotation);
                        //add new command text to the new randomObject
                        randObject.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>().text = newCommandText;
                        //set randObject's rCommand in it's Script
                        randObject.transform.GetChild(2).GetComponent<W_Lever_Handle_Script>().rCommand = (x * gridX) + y;
                        break;
                    default:
                        //roll for a random Button command from the buttonCommandArray
                        commandIndex = Random.Range(0, buttonCommandArray.Count);
                        newCommandText = (string)buttonCommandArray[commandIndex];
                        //remove selected button command from buttonCommandArray so it won't be used again
                        buttonCommandArray.RemoveAt(commandIndex);
                        //copy randomObject from the default wLever
                        randObject = (GameObject)Instantiate(button, new Vector3(-3 + (3 * x), 3 + (4 * y), button.transform.position.z), button.transform.rotation);
                        //add new command text to the new randomObject
                        randObject.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>().text = newCommandText;
                        //set randObject's rCommand in it's Script
                        randObject.GetComponent<Button_Press_Script>().rCommand = (x * gridX) + y;
                        break;
                }

                //add randomObject to grid
                grid[x, y] = randObject;
            }
        }

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
    // This will either wait for the given seconds, or until the isTapped boolean is set to TRUE
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

    // End the waitForSeconds by setting the timer to zero AND signal that a button was tapped (isTapped = true)
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

        //Roll random number to decide new command from the grid
        int rX = Random.Range(0, gridX);
        int rY = Random.Range(0, gridY);
        rCommand = (rX * gridX) + rY;

        //get random game object from grid
        GameObject rObj = grid[rX, rY];

        int commandType = buttonCommand;
        if (rObj.name.Contains("Button"))
            commandType = buttonCommand;
        else if (rObj.name.Contains("L_Lever"))
            commandType = lLeverCommand;
        else if (rObj.name.Contains("W_Lever"))
            commandType = wLeverCommand;


        //Get new command
        switch (commandType)
        {
            case buttonCommand:
                //Button
                string buttonText = rObj.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>().text;
                message = "Engage " + buttonText;
                break;
            case lLeverCommand:
                //L_Lever
                string lLeverText = rObj.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>().text;
                L_Lever_Handle_Script lLeverHandleScript = rObj.transform.GetChild(2).GetComponent<L_Lever_Handle_Script>();
                message = "Turn ";
                if(lLeverHandleScript.isLLeverUp)
                {
                    message += "OFF ";
                    rCommand = (rCommand * 100) + 2;
                }
                else
                {
                    message += "ON ";
                    rCommand = (rCommand * 100) + 1;
                }
                message += lLeverText;
                break;
            case wLeverCommand:
                //W_Lever
                string wLeverText = rObj.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>().text;
                W_Lever_Handle_Script wLeverHandleScript = rObj.transform.GetChild(2).GetComponent<W_Lever_Handle_Script>();
                message = "";
                if(wLeverHandleScript.isWLeverUp)
                {
                    message += "Lower ";
                    rCommand = (rCommand * 100) + 2;
                }
                else
                {
                    message += "Raise ";
                    rCommand = (rCommand * 100) + 1;
                }
                message += wLeverText;
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
