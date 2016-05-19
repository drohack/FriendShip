using UnityEngine;
using System.Collections;

public class Mastermind_Script : MonoBehaviour {

    /** SINGLE VARIABLES **/
    private         int numPlayers = 1;
    private         int score = 0;
    private         int numOfDiffGameObjects = 3; // The number of different type of game objects total to be used for random rolling of said game objects
    private const   int commandTimeoutSeconds = 10;
    public const    int buttonCommand = 0;
    public const    int lLeverCommand = 1;
    public const    int wLeverCommand = 2;
    private ArrayList buttonCommandArray;
    private ArrayList lLeverCommandArray;
    private ArrayList wLeverCommandArray;

    // Player Objects
    GameObject          p1_PlayerControlDeck;
    GameObject          p2_PlayerControlDeck;
    GameObject          p3_PlayerControlDeck;
    GameObject          p4_PlayerControlDeck;
    Score_Text_Script   p1_scoreTextScript;
    Score_Text_Script   p2_scoreTextScript;
    Score_Text_Script   p3_scoreTextScript;
    Score_Text_Script   p4_scoreTextScript;
    Console_Text_Script p1_consoleTextScript;
    Console_Text_Script p2_consoleTextScript;
    Console_Text_Script p3_consoleTextScript;
    Console_Text_Script p4_consoleTextScript;

    /** P1 VARIABLES **/
    public  int     p1_rCommand = -1;
    private bool    p1_isDisplayStart = true;
    private bool    p1_isTyping = false;
    public  bool    p1_isDisplayingCommand = false;
    private float   p1_gWaitSystem;         // Variables for the custom WaitForSeconds function
    private bool    p1_isTapped = false;    // Variables for the custom WaitForSeconds function
    private GameObject[,] p1_grid;          // The grid which the random game objects get placed
    private int     p1_gridX = 3;           // The grid which the random game objects get placed
    private int     p1_gridY = 3;           // The grid which the random game objects get placed

    /** P2 VARIABLES **/
    public int      p2_rCommand = -1;
    private bool    p2_isDisplayStart = true;
    private bool    p2_isTyping = false;
    public bool     p2_isDisplayingCommand = false;
    private float   p2_gWaitSystem;         // Variables for the custom WaitForSeconds function
    private bool    p2_isTapped = false;    // Variables for the custom WaitForSeconds function
    private GameObject[,] p2_grid;          // The grid which the random game objects get placed
    private int     p2_gridX = 3;           // The grid which the random game objects get placed
    private int     p2_gridY = 3;           // The grid which the random game objects get placed

    /** P3 VARIABLES **/
    public int      p3_rCommand = -1;
    private bool    p3_isDisplayStart = true;
    private bool    p3_isTyping = false;
    public bool     p3_isDisplayingCommand = false;
    private float   p3_gWaitSystem;         // Variables for the custom WaitForSeconds function
    private bool    p3_isTapped = false;    // Variables for the custom WaitForSeconds function
    private GameObject[,] p3_grid;          // The grid which the random game objects get placed
    private int     p3_gridX = 3;           // The grid which the random game objects get placed
    private int     p3_gridY = 3;           // The grid which the random game objects get placed

    /** P4 VARIABLES **/
    public int      p4_rCommand = -1;
    private bool    p4_isDisplayStart = true;
    private bool    p4_isTyping = false;
    public bool     p4_isDisplayingCommand = false;
    private float   p4_gWaitSystem;         // Variables for the custom WaitForSeconds function
    private bool    p4_isTapped = false;    // Variables for the custom WaitForSeconds function
    private GameObject[,] p4_grid;          // The grid which the random game objects get placed
    private int     p4_gridX = 3;           // The grid which the random game objects get placed
    private int     p4_gridY = 3;           // The grid which the random game objects get placed

    // Use this for initialization
    void Start () {
        Initialize();

        //Create a grid to hold all of the random game objects
        p1_grid = new GameObject[p1_gridX, p1_gridY];
        int commandIndex;
        string newCommandText;

        float xBQuaternion = ((GameObject)Resources.Load("Prefabs/Button")).transform.rotation.eulerAngles.x + p1_PlayerControlDeck.transform.rotation.eulerAngles.x;
        float yBQuaternion = ((GameObject)Resources.Load("Prefabs/Button")).transform.rotation.eulerAngles.y + p1_PlayerControlDeck.transform.rotation.eulerAngles.y;
        float zBQuaternion = ((GameObject)Resources.Load("Prefabs/Button")).transform.rotation.eulerAngles.z + p1_PlayerControlDeck.transform.rotation.eulerAngles.z;
        float xLQuaternion = ((GameObject)Resources.Load("Prefabs/L_Lever")).transform.rotation.eulerAngles.x + p1_PlayerControlDeck.transform.rotation.eulerAngles.x;
        float yLQuaternion = ((GameObject)Resources.Load("Prefabs/L_Lever")).transform.rotation.eulerAngles.y + p1_PlayerControlDeck.transform.rotation.eulerAngles.y;
        float zLQuaternion = ((GameObject)Resources.Load("Prefabs/L_Lever")).transform.rotation.eulerAngles.z + p1_PlayerControlDeck.transform.rotation.eulerAngles.z;
        float xWQuaternion = ((GameObject)Resources.Load("Prefabs/W_Lever")).transform.rotation.eulerAngles.x + p1_PlayerControlDeck.transform.rotation.eulerAngles.x;
        float yWQuaternion = ((GameObject)Resources.Load("Prefabs/W_Lever")).transform.rotation.eulerAngles.y + p1_PlayerControlDeck.transform.rotation.eulerAngles.y;
        float zWQuaternion = ((GameObject)Resources.Load("Prefabs/W_Lever")).transform.rotation.eulerAngles.z + p1_PlayerControlDeck.transform.rotation.eulerAngles.z;

        //for each grid position generate a random object and add it to the grid
        for (int x = 0; x < p1_gridX; x++)
        {
            for (int y = 0; y < p1_gridY; y++)
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
                        randObject = (GameObject)Instantiate(Resources.Load("Prefabs/Button"),
                            new Vector3(p1_PlayerControlDeck.transform.position.x - 3 + (3 * x), 3 + (4 * y), (p1_PlayerControlDeck.transform.position.z - 0.7f)),
                            Quaternion.Euler(new Vector3(xBQuaternion, yBQuaternion, zBQuaternion)));
                        randObject.transform.parent = p1_PlayerControlDeck.transform;
                        //add new command text to the new randomObject
                        randObject.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>().text = newCommandText;
                        //set randObject's rCommand in it's Script
                        randObject.GetComponent<Button_Press_Script>().rCommand = (x * p1_gridX) + y;
                        break;
                    case lLeverCommand:
                        //roll for a random Button command from the lLeverCommandArray
                        commandIndex = Random.Range(0, lLeverCommandArray.Count);
                        newCommandText = (string)lLeverCommandArray[commandIndex];
                        //remove selected button command from lLeverCommandArray so it won't be used again
                        lLeverCommandArray.RemoveAt(commandIndex);
                        //copy randomObject from the default wLever
                        randObject = (GameObject)Instantiate(Resources.Load("Prefabs/L_Lever"),
                            new Vector3(p1_PlayerControlDeck.transform.position.x - 3 + (3 * x), 3 + (4 * y), (p1_PlayerControlDeck.transform.position.z - 1.47f)),
                            Quaternion.Euler(new Vector3(xLQuaternion, yLQuaternion, zLQuaternion)));
                        randObject.transform.parent = p1_PlayerControlDeck.transform;
                        //add new command text to the new randomObject
                        randObject.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>().text = newCommandText;
                        //set randObject's rCommand in it's Script
                        randObject.transform.GetChild(2).GetComponent<L_Lever_Handle_Script>().rCommand = (x * p1_gridX) + y;
                        break;
                    case wLeverCommand:
                        //roll for a random Button command from the wLeverCommandArray
                        commandIndex = Random.Range(0, wLeverCommandArray.Count);
                        newCommandText = (string)wLeverCommandArray[commandIndex];
                        //remove selected button command from wLeverCommandArray so it won't be used again
                        wLeverCommandArray.RemoveAt(commandIndex);
                        //copy randomObject from the default wLever
                        randObject = (GameObject)Instantiate(Resources.Load("Prefabs/W_Lever"),
                            new Vector3(p1_PlayerControlDeck.transform.position.x - 3 + (3 * x), 3 + (4 * y), (p1_PlayerControlDeck.transform.position.z - 1.52f)),
                            Quaternion.Euler(new Vector3(xWQuaternion, yWQuaternion, zWQuaternion)));
                        randObject.transform.parent = p1_PlayerControlDeck.transform;
                        //add new command text to the new randomObject
                        randObject.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>().text = newCommandText;
                        //set randObject's rCommand in it's Script
                        randObject.transform.GetChild(2).GetComponent<W_Lever_Handle_Script>().rCommand = (x * p1_gridX) + y;
                        break;
                    default:
                        //roll for a random Button command from the buttonCommandArray
                        commandIndex = Random.Range(0, buttonCommandArray.Count);
                        newCommandText = (string)buttonCommandArray[commandIndex];
                        //remove selected button command from buttonCommandArray so it won't be used again
                        buttonCommandArray.RemoveAt(commandIndex);
                        //copy randomObject from the default wLever
                        randObject = (GameObject)Instantiate(Resources.Load("Prefabs/Button"),
                            new Vector3(p1_PlayerControlDeck.transform.position.x - 3 + (3 * x), 3 + (4 * y), (p1_PlayerControlDeck.transform.position.z - 0.7f)),
                            Quaternion.Euler(new Vector3(xBQuaternion, yBQuaternion, zBQuaternion)));
                        randObject.transform.parent = p1_PlayerControlDeck.transform;
                        //add new command text to the new randomObject
                        randObject.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>().text = newCommandText;
                        //set randObject's rCommand in it's Script
                        randObject.GetComponent<Button_Press_Script>().rCommand = (x * p1_gridX) + y;
                        break;
                }

                //add randomObject to grid
                p1_grid[x, y] = randObject;
            }
        }

        StartCoroutine(DisplayStartText());
    }

    void Initialize()
    {
        score = 0;

        // Set command arrays from Command_Array.cs
        Command_Array commandArray = GetComponent<Command_Array>();
        buttonCommandArray = commandArray.buttonCommandArray;
        lLeverCommandArray = commandArray.lLeverCommandArray;
        wLeverCommandArray = commandArray.wLeverCommandArray;

        // Set player objects
        p1_PlayerControlDeck = GameObject.Find("Player Control Deck 1");
        p1_scoreTextScript = p1_PlayerControlDeck.transform.Find("Score_Text").GetComponent<Score_Text_Script>();
        p1_consoleTextScript = p1_PlayerControlDeck.transform.Find("Console/Console_Text").GetComponent<Console_Text_Script>();
        if (numPlayers > 1)
        {
            p2_PlayerControlDeck = GameObject.Find("Player Control Deck 2");
            p2_scoreTextScript = p2_PlayerControlDeck.transform.Find("Score_Text").GetComponent<Score_Text_Script>();
            p2_consoleTextScript = p2_PlayerControlDeck.transform.Find("Console/Console_Text").GetComponent<Console_Text_Script>();
        }
        if (numPlayers > 2)
        {
            p3_PlayerControlDeck = GameObject.Find("Player Control Deck 3");
            p3_scoreTextScript = p3_PlayerControlDeck.transform.Find("Score_Text").GetComponent<Score_Text_Script>();
            p3_consoleTextScript = p3_PlayerControlDeck.transform.Find("Console/Console_Text").GetComponent<Console_Text_Script>();
        }
        if (numPlayers > 3)
        {
            p4_PlayerControlDeck = GameObject.Find("Player Control Deck 4");
            p4_scoreTextScript = p4_PlayerControlDeck.transform.Find("Score_Text").GetComponent<Score_Text_Script>();
            p4_consoleTextScript = p4_PlayerControlDeck.transform.Find("Console/Console_Text").GetComponent<Console_Text_Script>();
        }
    }
	
	// Update is called once per frame
	void Update () {
        //Quit if the user presses the esc key
        if (Input.GetKey("escape"))
            Application.Quit();

        //If the user scores 10 or more, change text to Green and to say "YOU WIN~"
        if (score >= 10)
        {
            p1_scoreTextScript.Win();
            if (numPlayers > 1)
                p2_scoreTextScript.Win();
            if (numPlayers > 2)
                p3_scoreTextScript.Win();
            if (numPlayers > 3)
                p4_scoreTextScript.Win();
        }

        //if we are NOT typing "START!" including waiting the 2 seconds
        //AND if we are NOT currently typing a command
        //AND if we are NOT currently waiting the 10 seconds for a command to pass
        //generate and display a new random command
        if (!p1_isDisplayStart && !p1_consoleTextScript.isTyping && !p1_isDisplayingCommand)
            StartCoroutine(P1_DisplayRandomCommand());
    }

    public void ScoreUp()
    {
        score++;
        p1_scoreTextScript.ScoreUp(score);
        if (numPlayers > 1)
            p2_scoreTextScript.ScoreUp(score);
        if (numPlayers > 2)
            p3_scoreTextScript.ScoreUp(score);
        if (numPlayers > 3)
            p4_scoreTextScript.ScoreUp(score);
    }

    public void ScoreDown()
    {
        score--;
        p1_scoreTextScript.ScoreDown(score);
        if (numPlayers > 1)
            p2_scoreTextScript.ScoreDown(score);
        if (numPlayers > 2)
            p3_scoreTextScript.ScoreDown(score);
        if (numPlayers > 3)
            p4_scoreTextScript.ScoreDown(score);
    }

    //Display "START!" for 2 seconds
    IEnumerator DisplayStartText()
    {
        p1_isDisplayStart = true;
        StartCoroutine(p1_consoleTextScript.TypeText(" START!"));
        if (numPlayers > 1)
        {
            p2_isDisplayStart = true;
            StartCoroutine(p2_consoleTextScript.TypeText(" START!"));
        }
        if (numPlayers > 2)
        {
            p3_isDisplayStart = true;
            StartCoroutine(p3_consoleTextScript.TypeText(" START!"));
        }
        if (numPlayers > 3)
        {
            p4_isDisplayStart = true;
            StartCoroutine(p4_consoleTextScript.TypeText(" START!"));
        }
        yield return new WaitForSeconds(2);
        p1_isDisplayStart = false;
        p2_isDisplayStart = false;
        p3_isDisplayStart = false;
        p4_isDisplayStart = false;
    }

    // Custom WaitForSeconds
    // This will either wait for the given seconds, or until the isTapped boolean is set to TRUE
    IEnumerator WaitForSecondsOrTap(float seconds)
    {
        p1_gWaitSystem = seconds;
        while (p1_gWaitSystem > 0.0)
        {
            p1_gWaitSystem -= Time.deltaTime;
            yield return 0;
        }

        //lower score if time reached (button was not tapped)
        if (!p1_isTapped)
        {
            ScoreDown();
        }

        //reset isTapped
        p1_isTapped = false;
    }

    // End the waitForSeconds by setting the timer to zero AND signal that a button was tapped (isTapped = true)
    public void TappedWaitForSecondsOrTap(int inputCommand)
    {
        p1_isTapped = true;
        
        //Check to see if the current command is the correct button pressed. Update score accordingly
        if (p1_rCommand == inputCommand)
        {
            ScoreUp();
        }
        else
        {
            ScoreDown();
        }

        //Set timer to 0 to get next command (Always)
        p1_gWaitSystem = 0.0f;
    }

    IEnumerator P1_DisplayRandomCommand()
    {
        p1_isDisplayingCommand = true;

        //Clear text
        p1_consoleTextScript.GetComponent<TextMesh>().text = "";
        yield return 0;
        yield return new WaitForSeconds(1);

        //Roll random number to decide new command from the grid
        int rX = Random.Range(0, p1_gridX);
        int rY = Random.Range(0, p1_gridY);
        p1_rCommand = (rX * p1_gridX) + rY;

        //get random game object from grid
        GameObject rObj = p1_grid[rX, rY];

        int commandType = buttonCommand;
        if (rObj.name.Contains("Button"))
            commandType = buttonCommand;
        else if (rObj.name.Contains("L_Lever"))
            commandType = lLeverCommand;
        else if (rObj.name.Contains("W_Lever"))
            commandType = wLeverCommand;


        string message = "";
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
                if (lLeverHandleScript.isLLeverUp)
                {
                    message += "OFF ";
                    p1_rCommand = (p1_rCommand * 100) + 2;
                }
                else
                {
                    message += "ON ";
                    p1_rCommand = (p1_rCommand * 100) + 1;
                }
                message += lLeverText;
                break;
            case wLeverCommand:
                //W_Lever
                string wLeverText = rObj.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>().text;
                W_Lever_Handle_Script wLeverHandleScript = rObj.transform.GetChild(2).GetComponent<W_Lever_Handle_Script>();
                message = "";
                if (wLeverHandleScript.isWLeverUp)
                {
                    message += "Lower ";
                    p1_rCommand = (p1_rCommand * 100) + 2;
                }
                else
                {
                    message += "Raise ";
                    p1_rCommand = (p1_rCommand * 100) + 1;
                }
                message += wLeverText;
                break;
            default:
                break;
        }

        //Type out new command to console
        StartCoroutine(p1_consoleTextScript.TypeText(message));

        //Wait for the commandTimeoutSeconds or if a button gets tapped
        yield return WaitForSecondsOrTap(commandTimeoutSeconds);
        p1_isDisplayingCommand = false;
    }
}
