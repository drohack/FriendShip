using UnityEngine;
using System.Collections;
using System.Linq;

public class Mastermind_Script : Photon.MonoBehaviour
{
    public class moduleType
    {
        public GameObject module;
        public object[] data;
    }

    /** SINGLE VARIABLES **/
    private int numPlayers = 0;
    private GameObject[] playerModules;
    private bool[] playerPosOccupied = new bool[4] { false, false, false, false };
    private int totalScore = 0;
    private int levelScore = 0;
    private int level = 1;
    private bool isLoadingNextLevel = false;
    private int scoreToWin = 10;
    private int scoreToLose = -10;
    private float levelTimeoutSeconds = 100;
    private float commandTimeoutSeconds = 10;
    private System.DateTime levelStartTime = System.DateTime.Now;
    private Timer_Script timerScript;
    private bool isGameOver = false;
    private float easyPercent = 0.8f;
    private float mediumPercent = 0.2f;
    private float hardPercent = 0f;
    private const int commandTextRowLimit = 15;
    public const int pullcordCommand = 10000;
    public const int buttonCommand = 0;
    public const int dialCommand = 1;
    public const int lLeverCommand = 2;
    public const int lightswitchCommand = 3;
    public const int plutoniumBatteryCommand = 4;
    public const int shifterCommand = 5;
    public const int sliderCommand = 6;
    public const int valveCommand = 7;
    public const int wLeverCommand = 8;
    private int firstRandomModuleCommand = 0;
    private int lastRandomModuleCommandPlusOne = 9; // The number of different type of modules total to be used for random rolling of said modules
    private string pullcordCommandText;
    private ArrayList usedCommandArray;
    private ArrayList buttonCommandArray_EASY;
    private ArrayList dialCommandArray_EASY;
    private ArrayList lLeverCommandArray_EASY;
    private ArrayList lightswitchCommandArray_EASY;
    private ArrayList plutoniumBatteryCommandArray_EASY;
    private ArrayList shifterCommandArray_EASY;
    private ArrayList sliderCommandArray_EASY;
    private ArrayList valveCommandArray_EASY;
    private ArrayList wLeverCommandArray_EASY;
    private ArrayList buttonCommandArray_MEDIUM;
    private ArrayList dialCommandArray_MEDIUM;
    private ArrayList lLeverCommandArray_MEDIUM;
    private ArrayList lightswitchCommandArray_MEDIUM;
    private ArrayList plutoniumBatteryCommandArray_MEDIUM;
    private ArrayList shifterCommandArray_MEDIUM;
    private ArrayList sliderCommandArray_MEDIUM;
    private ArrayList valveCommandArray_MEDIUM;
    private ArrayList wLeverCommandArray_MEDIUM;
    private ArrayList buttonCommandArray_HARD;
    private ArrayList dialCommandArray_HARD;
    private ArrayList lLeverCommandArray_HARD;
    private ArrayList lightswitchCommandArray_HARD;
    private ArrayList plutoniumBatteryCommandArray_HARD;
    private ArrayList shifterCommandArray_HARD;
    private ArrayList sliderCommandArray_HARD;
    private ArrayList valveCommandArray_HARD;
    private ArrayList wLeverCommandArray_HARD;
    private moduleType[] moduleList; // The list of all modules in current round
    private Pullcord_Script[] pullcordScriptList; // The list of all pullcords in current round
    private int numFufilled = 0;
    private PhotonPlayer[] players;

    // Hazards
    private const int numHazards = 4;
    private GameObject[] hazardsList; // The list of all hazard modules in current round
    private bool[] activeHazardsList;
    private bool isWaitingForHazard = false;
    // ambient light
    private int ambientLightIndex = 0;
    private Color defaultAmbientLight;
    private float defaultDirectionalLightIntensity;
    private GameObject plutoniumLight;
    private Light directionalLight;
    // fog valve
    private bool isIncreasingFog = false;
    private int fogValveIndex = 1;
    private float defaultFogDensity;
    private GameObject fogValve;
    // resize button
    private int resizeButtonIndex = 2;
    private Vector3 defaultScale;
    private GameObject resizeButton;
    // static lever
    private int staticLeverIndex = 3;
    private float defaultStatic;
    private GameObject staticLever;
    private AudioSource staticAudio;
    // gravity
    //private int gravityIndex = 1;
    //private Vector3 defaultGravity;
    //private GameObject gravityLever;

    // Player Objects
    GameObject p1_PlayerControlDeck;
    GameObject p2_PlayerControlDeck;
    GameObject p3_PlayerControlDeck;
    GameObject p4_PlayerControlDeck;
    GameObject p1_BackPanel;
    GameObject p2_BackPanel;
    GameObject p3_BackPanel;
    GameObject p4_BackPanel;
    Command_Feedback_Script p1_CommandFeedbackScript;
    Command_Feedback_Script p2_CommandFeedbackScript;
    Command_Feedback_Script p3_CommandFeedbackScript;
    Command_Feedback_Script p4_CommandFeedbackScript;
    Abort_Reset_Rotate_Feedback_Script p1_AbortResetScript;
    Abort_Reset_Rotate_Feedback_Script p2_AbortResetScript;
    Abort_Reset_Rotate_Feedback_Script p3_AbortResetScript;
    Abort_Reset_Rotate_Feedback_Script p4_AbortResetScript;
    Score_Text_Script p1_scoreTextScript;
    Score_Text_Script p2_scoreTextScript;
    Score_Text_Script p3_scoreTextScript;
    Score_Text_Script p4_scoreTextScript;
    Console_Text_Script p1_consoleTextScript;
    Console_Text_Script p2_consoleTextScript;
    Console_Text_Script p3_consoleTextScript;
    Console_Text_Script p4_consoleTextScript;
    Console_Timer_Script p1_consoleTimerScript;
    Console_Timer_Script p2_consoleTimerScript;
    Console_Timer_Script p3_consoleTimerScript;
    Console_Timer_Script p4_consoleTimerScript;

    /** P1 VARIABLES **/
    public int p1_rCommand = -1;
    private bool p1_isDisplayStart = true;
    public bool p1_isDisplayingCommand = false;
    private float p1_gWaitSystem;         // Variables for the custom WaitForSeconds function
    private int p1_gridX = 4;           // The grid which the random game objects get placed
    private int p1_gridY = 2;           // The grid which the random game objects get placed
    public bool p1_Resetting = false;   // Is the player pressing their reset button?
    public bool p1_Aborting = false;    // Is the player pressing their abort button?    

    /** P2 VARIABLES **/
    public int p2_rCommand = -1;
    private bool p2_isDisplayStart = true;
    public bool p2_isDisplayingCommand = false;
    private float p2_gWaitSystem;         // Variables for the custom WaitForSeconds function
    private int p2_gridX = 4;           // The grid which the random game objects get placed
    private int p2_gridY = 2;           // The grid which the random game objects get placed
    public bool p2_Resetting = false;   // Is the player pressing their reset button?
    public bool p2_Aborting = false;    // Is the player pressing their abort button?    

    /** P3 VARIABLES **/
    public int p3_rCommand = -1;
    private bool p3_isDisplayStart = true;
    public bool p3_isDisplayingCommand = false;
    private float p3_gWaitSystem;         // Variables for the custom WaitForSeconds function
    private int p3_gridX = 4;           // The grid which the random game objects get placed
    private int p3_gridY = 2;           // The grid which the random game objects get placed
    public bool p3_Resetting = false;   // Is the player pressing their reset button?
    public bool p3_Aborting = false;    // Is the player pressing their abort button? 

    /** P4 VARIABLES **/
    public int p4_rCommand = -1;
    private bool p4_isDisplayStart = true;
    public bool p4_isDisplayingCommand = false;
    private float p4_gWaitSystem;         // Variables for the custom WaitForSeconds function
    private int p4_gridX = 4;           // The grid which the random game objects get placed
    private int p4_gridY = 2;           // The grid which the random game objects get placed
    public bool p4_Resetting = false;   // Is the player pressing their reset button?
    public bool p4_Aborting = false;    // Is the player pressing their abort button? 

    //##################################################################################################################################
    //                                              INITIAL LOAD OF MASTERMIND
    //##################################################################################################################################

    // Use this for initialization
    void Start()
    {
        isLoadingNextLevel = true;

        isGameOver = false;

        //Load Network data
        level = (int)PhotonNetwork.room.customProperties[PhotonConstants.level];

        //Get number of players by the NetwokManager.numPlayers
        numPlayers = PhotonNetwork.playerList.Length;
        Debug.Log("numPlayers: " + numPlayers);
        playerPosOccupied = (bool[])PhotonNetwork.room.customProperties[PhotonConstants.pPosOccupied];
        Debug.Log("pPosOccupied: " + playerPosOccupied[0] + ", " + playerPosOccupied[1] + ", " + playerPosOccupied[2] + ", " + playerPosOccupied[3]);

        //Initialize all variables at start of game
        Initialize();

        if (PhotonNetwork.isMasterClient)
        {
            //Wait for all the players to load into the scene before starting the game
            StartCoroutine(WaitForPlayersToSpawn());
        }
    }

    IEnumerator WaitForPlayersToSpawn()
    {
        //Wait for all players to load into the scene
        bool areAllPlayersReady = false;
        do
        {
            bool[] playerPosOccupied = (bool[])PhotonNetwork.room.customProperties[PhotonConstants.pPosOccupied];
            bool[] playerLoadedList = new bool[playerPosOccupied.Length];
            foreach (PhotonPlayer p in PhotonNetwork.playerList)
            {
                if(p.customProperties.ContainsKey(PhotonConstants.isLoadedIntoGame) && p.customProperties.ContainsKey(PhotonConstants.pPos))
                {
                    playerLoadedList[(int)p.customProperties[PhotonConstants.pPos]] = (bool)p.customProperties[PhotonConstants.isLoadedIntoGame];
                }
            }
            if (playerPosOccupied.SequenceEqual(playerLoadedList))
            {
                areAllPlayersReady = true;
            }
            Debug.Log("areAllPlayersReady: " + areAllPlayersReady);
            Debug.Log("pPosOccupied: " + playerPosOccupied[0] + ", " + playerPosOccupied[1] + ", " + playerPosOccupied[2] + ", " + playerPosOccupied[3]);
            Debug.Log("playerLoadedList: " + playerLoadedList[0] + ", " + playerLoadedList[1] + ", " + playerLoadedList[2] + ", " + playerLoadedList[3]);
            yield return new WaitForSeconds(0.1f);
        } while (!areAllPlayersReady);

            //Set up level variables for Custom Room Property "Level" (score to win this round, number of seconds for each command, number of modules to spawn per player)
            SetupLevel();

        isLoadingNextLevel = false;

        //Count down from 3 to next level
        //Generate new modules
        //And display "Start!" command
        StartCoroutine(StartNextRoundIn(3));
    }

    void Initialize()
    {
        totalScore = 0;
        levelScore = 0;

        //Get Timer object
        timerScript = GameObject.FindGameObjectWithTag("Timer").GetComponent<Timer_Script>();

        // Set command arrays from Command_Array.cs so we can take away items from the array list so we have no repeats
        Command_Array commandArray = GetComponent<Command_Array>();
        pullcordCommandText = Command_Array.pullcordText;
        buttonCommandArray_EASY = new ArrayList(commandArray.buttonCommandArray_EASY);
        dialCommandArray_EASY = new ArrayList(commandArray.dialCommandArray_EASY);
        lLeverCommandArray_EASY = new ArrayList(commandArray.lLeverCommandArray_EASY);
        lightswitchCommandArray_EASY = new ArrayList(commandArray.lightswitchCommandArray_EASY);
        plutoniumBatteryCommandArray_EASY = new ArrayList(commandArray.plutoniumBatteryCommandArray_EASY);
        shifterCommandArray_EASY = new ArrayList(commandArray.shifterCommandArray_EASY);
        sliderCommandArray_EASY = new ArrayList(commandArray.sliderCommandArray_EASY);
        valveCommandArray_EASY = new ArrayList(commandArray.valveCommandArray_EASY);
        wLeverCommandArray_EASY = new ArrayList(commandArray.wLeverCommandArray_EASY);
        buttonCommandArray_MEDIUM = new ArrayList(commandArray.buttonCommandArray_MEDIUM);
        dialCommandArray_MEDIUM = new ArrayList(commandArray.dialCommandArray_MEDIUM);
        lLeverCommandArray_MEDIUM = new ArrayList(commandArray.lLeverCommandArray_MEDIUM);
        lightswitchCommandArray_MEDIUM = new ArrayList(commandArray.lightswitchCommandArray_MEDIUM);
        plutoniumBatteryCommandArray_MEDIUM = new ArrayList(commandArray.plutoniumBatteryCommandArray_MEDIUM);
        shifterCommandArray_MEDIUM = new ArrayList(commandArray.shifterCommandArray_MEDIUM);
        sliderCommandArray_MEDIUM = new ArrayList(commandArray.sliderCommandArray_MEDIUM);
        valveCommandArray_MEDIUM = new ArrayList(commandArray.valveCommandArray_MEDIUM);
        wLeverCommandArray_MEDIUM = new ArrayList(commandArray.wLeverCommandArray_MEDIUM);
        buttonCommandArray_HARD = new ArrayList(commandArray.buttonCommandArray_HARD);
        dialCommandArray_HARD = new ArrayList(commandArray.dialCommandArray_HARD);
        lLeverCommandArray_HARD = new ArrayList(commandArray.lLeverCommandArray_HARD);
        lightswitchCommandArray_HARD = new ArrayList(commandArray.lightswitchCommandArray_HARD);
        plutoniumBatteryCommandArray_HARD = new ArrayList(commandArray.plutoniumBatteryCommandArray_HARD);
        shifterCommandArray_HARD = new ArrayList(commandArray.shifterCommandArray_HARD);
        sliderCommandArray_HARD = new ArrayList(commandArray.sliderCommandArray_HARD);
        valveCommandArray_HARD = new ArrayList(commandArray.valveCommandArray_HARD);
        wLeverCommandArray_HARD = new ArrayList(commandArray.wLeverCommandArray_HARD);

        // Set player objects
        if (playerPosOccupied[0] == true)
        {
            p1_PlayerControlDeck = GameObject.Find("Player Control Deck 1");
            p1_BackPanel = GameObject.Find("Back Panel 1");
            p1_CommandFeedbackScript = p1_PlayerControlDeck.transform.Find("Command Feedback").GetComponent<Command_Feedback_Script>();
            p1_scoreTextScript = p1_PlayerControlDeck.transform.Find("Score_Text").GetComponent<Score_Text_Script>();
            p1_consoleTextScript = p1_PlayerControlDeck.transform.Find("Console/Console_Text").GetComponent<Console_Text_Script>();
            p1_consoleTimerScript = p1_PlayerControlDeck.transform.Find("Console/Timer").GetComponent<Console_Timer_Script>();
            p1_AbortResetScript = p1_BackPanel.GetComponent<Abort_Reset_Rotate_Feedback_Script>();
        }
        if (playerPosOccupied[1] == true)
        {
            p2_PlayerControlDeck = GameObject.Find("Player Control Deck 2");
            p2_BackPanel = GameObject.Find("Back Panel 2");
            p2_scoreTextScript = p2_PlayerControlDeck.transform.Find("Score_Text").GetComponent<Score_Text_Script>();
            p2_consoleTextScript = p2_PlayerControlDeck.transform.Find("Console/Console_Text").GetComponent<Console_Text_Script>();
            p2_consoleTimerScript = p2_PlayerControlDeck.transform.Find("Console/Timer").GetComponent<Console_Timer_Script>();
            p2_CommandFeedbackScript = p2_PlayerControlDeck.transform.Find("Command Feedback").GetComponent<Command_Feedback_Script>();
            p2_AbortResetScript = p2_BackPanel.GetComponent<Abort_Reset_Rotate_Feedback_Script>();
        }
        if (playerPosOccupied[2] == true)
        {
            p3_PlayerControlDeck = GameObject.Find("Player Control Deck 3");
            p3_BackPanel = GameObject.Find("Back Panel 3");
            p3_scoreTextScript = p3_PlayerControlDeck.transform.Find("Score_Text").GetComponent<Score_Text_Script>();
            p3_consoleTextScript = p3_PlayerControlDeck.transform.Find("Console/Console_Text").GetComponent<Console_Text_Script>();
            p3_consoleTimerScript = p3_PlayerControlDeck.transform.Find("Console/Timer").GetComponent<Console_Timer_Script>();
            p3_CommandFeedbackScript = p3_PlayerControlDeck.transform.Find("Command Feedback").GetComponent<Command_Feedback_Script>();
            p3_AbortResetScript = p3_BackPanel.GetComponent<Abort_Reset_Rotate_Feedback_Script>();
        }
        if (playerPosOccupied[3] == true)
        {
            p4_PlayerControlDeck = GameObject.Find("Player Control Deck 4");
            p4_BackPanel = GameObject.Find("Back Panel 4");
            p4_scoreTextScript = p4_PlayerControlDeck.transform.Find("Score_Text").GetComponent<Score_Text_Script>();
            p4_consoleTextScript = p4_PlayerControlDeck.transform.Find("Console/Console_Text").GetComponent<Console_Text_Script>();
            p4_consoleTimerScript = p4_PlayerControlDeck.transform.Find("Console/Timer").GetComponent<Console_Timer_Script>();
            p4_CommandFeedbackScript = p4_PlayerControlDeck.transform.Find("Command Feedback").GetComponent<Command_Feedback_Script>();
            p4_AbortResetScript = p4_BackPanel.GetComponent<Abort_Reset_Rotate_Feedback_Script>();
        }

        //Hazards initialize
        hazardsList = new GameObject[numHazards] { plutoniumLight, fogValve, resizeButton, staticLever };
        activeHazardsList = new bool[numHazards] { false, false, false, false };

        directionalLight = GameObject.Find("Directional Light").GetComponent<Light>();
        defaultDirectionalLightIntensity = directionalLight.intensity;
        defaultAmbientLight = RenderSettings.ambientLight;
        defaultFogDensity = RenderSettings.fogDensity;
        //defaultScale = GameObject.FindGameObjectWithTag("Player").transform.localScale;
        defaultStatic = 0f;
        staticAudio = GameObject.Find("HazardsExtra").GetComponent<AudioSource>();
}

    //##################################################################################################################################
    //                                                      LEVEL SETUP
    //##################################################################################################################################

    IEnumerator LoadNextLevel()
    {
        isLoadingNextLevel = true;
        levelScore = 0;
        level += 1;
        levelStartTime = System.DateTime.Now;

        //Update Room Custom Property level
        ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable() { { PhotonConstants.level, level } };
        PhotonNetwork.room.SetCustomProperties(ht);

        //Destroy all Modules inside of moduleList
        DestroyAllModules();

        //Destroy all Hazard modules
        DestroyAllHazards();

        //Set all timers to 0
        if (playerPosOccupied[0] == true)
            p1_gWaitSystem = 0f;
        if (playerPosOccupied[1] == true)
            p2_gWaitSystem = 0f;
        if (playerPosOccupied[2] == true)
            p3_gWaitSystem = 0f;
        if (playerPosOccupied[3] == true)
            p4_gWaitSystem = 0f;

        if (playerPosOccupied[0] == true)
            p1_scoreTextScript.photonView.RPC("Win", PhotonTargets.All, true);
        if (playerPosOccupied[1] == true)
            p2_scoreTextScript.photonView.RPC("Win", PhotonTargets.All, true);
        if (playerPosOccupied[2] == true)
            p3_scoreTextScript.photonView.RPC("Win", PhotonTargets.All, true);
        if (playerPosOccupied[3] == true)
            p4_scoreTextScript.photonView.RPC("Win", PhotonTargets.All, true);

        //Set up level variables (score to win this round, number of seconds for each command, number of modules to spawn per player)
        SetupLevel();

        yield return new WaitForSeconds(3);

        //Count down from 3 to next level
        //Generate new modules
        //And display "Start!" command
        StartCoroutine(StartNextRoundIn(3));

        UpdateScore();

        isLoadingNextLevel = false;
    }

    void DestroyAllModules()
    {
        //Destroy all Modules on all clients
        if (moduleList != null)
        {
            foreach (moduleType moduleType in moduleList)
            {
                moduleType.module.GetPhotonView().RPC("RPCDestroy", PhotonTargets.All);
            }
        }
    }

    //Display a countdown till next round, call to generate the random modules, and display "START!"
    IEnumerator StartNextRoundIn(int count)
    {
        p1_isDisplayStart = true;
        p2_isDisplayStart = true;
        p3_isDisplayStart = true;
        p4_isDisplayStart = true;

        //Play countdown
        UpdateAllConsoles(" Ready in");
        yield return new WaitForSeconds(1);
        for (int i = count; i > 0; i--)
        {
            UpdateAllConsoles(i.ToString());
            yield return new WaitForSeconds(1);
        }

        //Generate the random modules
        GenerateAllPlayerModules();

        //Generate Hazards
        IntantiateHazards();

        //Start the next round!
        UpdateAllConsoles(" START!");
        yield return new WaitForSeconds(1);

        p1_isDisplayStart = false;
        p2_isDisplayStart = false;
        p3_isDisplayStart = false;
        p4_isDisplayStart = false;

        levelStartTime = System.DateTime.Now;

        //Start Timer object
        timerScript.StartTimer(levelTimeoutSeconds, levelStartTime);

        //Start Hazards
        StartCoroutine("RunHazards");
    }

    private void SetupLevel()
    {
        //Total score to win this round
        //At level ONE this score is 10, going up by 2 each level, by level 5 this is 20 commands, by level 10 this is 30 commands
        scoreToWin = 10 + (2 * (level - 1));
        //Total score to lose this round
        //At level ONE this score is -10, by level 5 this is -4, by level 10 this is -2 (converging to -2)
        scoreToLose = Mathf.RoundToInt(-Mathf.Pow(Mathf.Sqrt(8), (-0.3f * (level - 1)) + 2) - 2);
        //Number of seconds for the level before Game Over
        //At level ONE this is 100 seconds (10 seconds per command), by level 5 this is 85.195 seconds, by level 10 this is 76.493 seconds (converging to 70 seconds by level 25)
        levelTimeoutSeconds = Mathf.Pow(Mathf.Sqrt(30), (-0.1f * (level - 1)) + 2) + 70;
        //Number of seconds for each command before it times out
        //At level ONE this starts at 10 seconds, by level 5 this is 8 seconds, and by level 10 this is 7 seconds (converging to 6 second by level 30)
        commandTimeoutSeconds = Mathf.Pow(Mathf.Sqrt(4), (-0.2f * (level - 1)) + 2) + 6;
        //The base number of modules a player can start with (this number will be varied +/- 1
        //At level ONE this starts at 3 modules per player, by level 5 this is 7 modules, by level 8 this maxes out at 8 modules always for all players (converging to 8 modules)
        int baseNumModulesPerPlayer = Mathf.RoundToInt(-Mathf.Pow(Mathf.Sqrt(8), (-0.3f * (level - 1)) + 1.54f) + 8);
        int[] xyNumModules_p1 = GetNumXYModules(baseNumModulesPerPlayer);
        p1_gridX = xyNumModules_p1[0];
        p1_gridY = xyNumModules_p1[1];
        int[] xyNumModules_p2 = GetNumXYModules(baseNumModulesPerPlayer);
        p2_gridX = xyNumModules_p2[0];
        p2_gridY = xyNumModules_p2[1];
        int[] xyNumModules_p3 = GetNumXYModules(baseNumModulesPerPlayer);
        p3_gridX = xyNumModules_p3[0];
        p3_gridY = xyNumModules_p3[1];
        int[] xyNumModules_p4 = GetNumXYModules(baseNumModulesPerPlayer);
        p4_gridX = xyNumModules_p4[0];
        p4_gridY = xyNumModules_p4[1];

        //Reset the usedCommandArray
        usedCommandArray = new ArrayList();

        // Set percent of EASY, MEDIUM, and HARD modules to spawn by level
        switch (level)
        {
            case 1:
                easyPercent = 1.0f;
                mediumPercent = 0.0f;
                hardPercent = 0.0f;
                break;
            case 2:
                easyPercent = 1.0f;
                mediumPercent = 0.0f;
                hardPercent = 0.0f;
                break;
            case 3:
                easyPercent = 0.8f;
                mediumPercent = 0.2f;
                hardPercent = 0.0f;
                break;
            case 4:
                easyPercent = 0.6f;
                mediumPercent = 0.4f;
                hardPercent = 0.0f;
                break;
            case 5:
                easyPercent = 0.4f;
                mediumPercent = 0.5f;
                hardPercent = 0.1f;
                break;
            case 6:
                easyPercent = 0.1f;
                mediumPercent = 0.6f;
                hardPercent = 0.3f;
                break;
            case 7:
                easyPercent = 0.05f;
                mediumPercent = 0.55f;
                hardPercent = 0.4f;
                break;
            case 8:
                easyPercent = 0.05f;
                mediumPercent = 0.35f;
                hardPercent = 0.6f;
                break;
            case 9:
                easyPercent = 0.05f;
                mediumPercent = 0.25f;
                hardPercent = 0.7f;
                break;
            case 10:
                easyPercent = 0.05f;
                mediumPercent = 0.15f;
                hardPercent = 0.8f;
                break;
            default:
                easyPercent = 0.05f;
                mediumPercent = 0.15f;
                hardPercent = 0.8f;
                break;
        }
    }

    private int[] GetNumXYModules(int baseNumModulesPerPlayer)
    {
        int[] returnXY = new int[2];
        int totalNumModules = baseNumModulesPerPlayer + Random.Range(-1, 2);
        if (totalNumModules >= 7) //Can't display 7 modules, set to 8
            totalNumModules = 8;
        else if (totalNumModules == 5) //Can't display 5 modules set to 4
            totalNumModules = 4;

        if (totalNumModules >= 8) //if the total number of modules is 8 then do a full 4x2 grid of modules
        {
            returnXY[0] = 4;
            returnXY[1] = 2;
        }
        else if (totalNumModules == 6) //if the total number of modules is 6 then do a 3x2 grid of modules
        {
            returnXY[0] = 3;
            returnXY[1] = 2;
        }
        else if (totalNumModules <= 4 && totalNumModules > 0)
        {
            //if the total number of modules is even and less than equal to 4 flip a coin to see if we should display the modules on a single row or two rows
            if ((totalNumModules % 2) == 0 && Random.value < 0.5f)
            {
                returnXY[0] = totalNumModules / 2;
                returnXY[1] = 2;
            }
            else
            {
                returnXY[0] = totalNumModules;
                returnXY[1] = 1;
            }
        }
        else
        {
            //Should never get here
            Debug.LogError("ERROR trying to generate too many or few modules. Base: " + baseNumModulesPerPlayer + " total: " + totalNumModules);
        }

        return returnXY;
    }

    //##################################################################################################################################
    //                                                      GENERATE MODULES
    //##################################################################################################################################

    //Create new list of modules
    void GenerateAllPlayerModules()
    {
        //Create a list to hold all of the modules
        int moduleListSize = 0;
        if (playerPosOccupied[0] == true)
            moduleListSize += (p1_gridX * p1_gridY);
        if (playerPosOccupied[1] == true)
            moduleListSize += (p2_gridX * p2_gridY);
        if (playerPosOccupied[2] == true)
            moduleListSize += (p3_gridX * p3_gridY);
        if (playerPosOccupied[3] == true)
            moduleListSize += (p4_gridX * p4_gridY);
        //Create new moduleList with the size of all grid sizes + number of players for Global Modules (like pullcord)
        moduleList = new moduleType[moduleListSize + numPlayers];
        pullcordScriptList = new Pullcord_Script[numPlayers];

        int moduleCount = 0;
        if (playerPosOccupied[0] == true)
        {
            //Generate moduleList objects for PLAYER 1
            moduleList = GenerateRandomModules(moduleList, 0, p1_PlayerControlDeck, p1_gridX, p1_gridY, 1);
            Transform p1_moduleTransform = p1_PlayerControlDeck.transform.Find("Modules");
            p1_moduleTransform.localRotation = Quaternion.Euler(new Vector3(45f, 0f, 0f));
            for (int i = 0; i < p1_moduleTransform.childCount; i++)
            {
                GameObject moduleEmpty = p1_moduleTransform.GetChild(i).gameObject;
                GameObject module = PhotonNetwork.InstantiateSceneObject("Modules/" + moduleEmpty.name, moduleEmpty.transform.position, moduleEmpty.transform.rotation, 0, moduleList[moduleCount].data);
                moduleList[moduleCount].module = module;
                moduleCount++;
            }
        }
        if (playerPosOccupied[1] == true)
        {
            //Generate moduleList objects for PLAYER 2
            moduleList = GenerateRandomModules(moduleList, (p1_gridX * p1_gridY), p2_PlayerControlDeck, p2_gridX, p2_gridY, 2);
            Transform p2_moduleTransform = p2_PlayerControlDeck.transform.Find("Modules");
            p2_moduleTransform.localRotation = Quaternion.Euler(new Vector3(45f, 0f, 0f));
            for (int i = 0; i < p2_moduleTransform.childCount; i++)
            {
                GameObject moduleEmpty = p2_moduleTransform.GetChild(i).gameObject;
                GameObject module = PhotonNetwork.InstantiateSceneObject("Modules/" + moduleEmpty.name, moduleEmpty.transform.position, moduleEmpty.transform.rotation, 0, moduleList[moduleCount].data);
                moduleList[moduleCount].module = module;
                moduleCount++;
            }
        }
        if (playerPosOccupied[2] == true)
        {
            //Generate moduleList objects for PLAYER 3
            moduleList = GenerateRandomModules(moduleList, (p1_gridX * p1_gridY) + (p2_gridX * p2_gridY), p3_PlayerControlDeck, p3_gridX, p3_gridY, 3);
            Transform p3_moduleTransform = p3_PlayerControlDeck.transform.Find("Modules");
            p3_moduleTransform.localRotation = Quaternion.Euler(new Vector3(45f, 0f, 0f));
            for (int i = 0; i < p3_moduleTransform.childCount; i++)
            {
                GameObject moduleEmpty = p3_moduleTransform.GetChild(i).gameObject;
                GameObject module = PhotonNetwork.InstantiateSceneObject("Modules/" + moduleEmpty.name, moduleEmpty.transform.position, moduleEmpty.transform.rotation, 0, moduleList[moduleCount].data);
                moduleList[moduleCount].module = module;
                moduleCount++;
            }
        }
        if (playerPosOccupied[3] == true)
        {
            //Generate moduleList objects for PLAYER 4
            moduleList = GenerateRandomModules(moduleList, (p1_gridX * p1_gridY) + (p2_gridX * p2_gridY) + (p3_gridX * p3_gridY), p4_PlayerControlDeck, p4_gridX, p4_gridY, 4);
            Transform p4_moduleTransform = p4_PlayerControlDeck.transform.Find("Modules");
            p4_moduleTransform.localRotation = Quaternion.Euler(new Vector3(45f, 0f, 0f));
            for (int i = 0; i < p4_moduleTransform.childCount; i++)
            {
                GameObject moduleEmpty = p4_moduleTransform.GetChild(i).gameObject;
                GameObject module = PhotonNetwork.InstantiateSceneObject("Modules/" + moduleEmpty.name, moduleEmpty.transform.position, moduleEmpty.transform.rotation, 0, moduleList[moduleCount].data);
                moduleList[moduleCount].module = module;
                moduleCount++;
            }
        }

        //Spawn Pullcords
        int pullcordCount = 0;
        if (playerPosOccupied[0] == true)
        {
            object[] data = new object[3];
            data[0] = "test";
            data[1] = pullcordCommand;
            data[2] = 1;
            GameObject module = PhotonNetwork.InstantiateSceneObject("Modules/Pullcord", new Vector3(0.4f, 1f, -1.55f), Quaternion.Euler(new Vector3(0, 90, 0)), 0, data);
            moduleType moduleType = new moduleType();
            moduleType.module = module;
            moduleType.data = data;
            moduleList[moduleCount] = moduleType;
            pullcordScriptList[pullcordCount] = module.GetComponent<Pullcord_Script>();
            moduleCount++;
            pullcordCount++;
        }
        if (playerPosOccupied[1] == true)
        {
            object[] data = new object[3];
            data[0] = "";
            data[1] = pullcordCommand;
            data[2] = 2;
            GameObject module = PhotonNetwork.InstantiateSceneObject("Modules/Pullcord", new Vector3(1.55f, 1f, 0.4f), Quaternion.Euler(new Vector3(0, 0, 0)), 0, data);
            moduleType moduleType = new moduleType();
            moduleType.module = module;
            moduleType.data = data;
            moduleList[moduleCount] = moduleType;
            pullcordScriptList[pullcordCount] = module.GetComponent<Pullcord_Script>();
            moduleCount++;
            pullcordCount++;
        }
        if (playerPosOccupied[2] == true)
        {
            object[] data = new object[3];
            data[0] = "";
            data[1] = pullcordCommand;
            data[2] = 3;
            GameObject module = PhotonNetwork.InstantiateSceneObject("Modules/Pullcord", new Vector3(-0.4f, 1f, 1.55f), Quaternion.Euler(new Vector3(0, -90, 0)), 0, data);
            moduleType moduleType = new moduleType();
            moduleType.module = module;
            moduleType.data = data;
            moduleList[moduleCount] = moduleType;
            pullcordScriptList[pullcordCount] = module.GetComponent<Pullcord_Script>();
            moduleCount++;
            pullcordCount++;
        }
        if (playerPosOccupied[3] == true)
        {
            object[] data = new object[3];
            data[0] = "";
            data[1] = pullcordCommand;
            data[2] = 4;
            GameObject module = PhotonNetwork.InstantiateSceneObject("Modules/Pullcord", new Vector3(-1.55f, 1f, -0.4f), Quaternion.Euler(new Vector3(0, 180, 0)), 0, data);
            moduleType moduleType = new moduleType();
            moduleType.module = module;
            moduleType.data = data;
            moduleList[moduleCount] = moduleType;
            pullcordScriptList[pullcordCount] = module.GetComponent<Pullcord_Script>();
            moduleCount++;
            pullcordCount++;
        }

        //Debug.Log("moduleCount: " + moduleCount);
    }

    moduleType[] GenerateRandomModules(moduleType[] inModuleList, int intModuleListSize, GameObject playerControlDeck, int gridX, int gridY, int playerNum)
    {
        //Destroy all objects within the players Modules list
        Transform moduleTransform = playerControlDeck.transform.Find("Modules");
        for (int i = moduleTransform.childCount - 1; i >= 0; --i)
        {
            GameObject.Destroy(moduleTransform.GetChild(i).gameObject);
        }
        moduleTransform.DetachChildren();
        moduleTransform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));

        moduleType[] outModuleList = inModuleList;

        string newCommandText;

        GameObject buttonInstance = (GameObject)Resources.Load("Modules/Button");

        float xQuaternion = buttonInstance.transform.rotation.eulerAngles.x + playerControlDeck.transform.rotation.eulerAngles.x;
        float yQuaternion = buttonInstance.transform.rotation.eulerAngles.y + playerControlDeck.transform.rotation.eulerAngles.y;
        float zQuaternion = buttonInstance.transform.rotation.eulerAngles.z + playerControlDeck.transform.rotation.eulerAngles.z;

        //for each grid position generate a module and add it to the module list
        for (int x = 0; x < gridX; x++)
        {
            if (x > 4)
                break;

            float xOffset = x;
            if (gridX == 1)
                xOffset += 1.5f;
            else if (gridX == 2)
                xOffset += 1f;
            else if (gridX == 3)
                xOffset += 0.5f;

            for (int y = 0; y < gridY; y++)
            {
                if (y > 2)
                    break;

                float yOffset = y;
                if (gridY == 1)
                    yOffset += 0.5f;

                moduleType moduleType = new moduleType();
                GameObject randModuleEmpty;
                object[] data = new object[3];
                //roll for a module
                int commandType = Random.Range(firstRandomModuleCommand, lastRandomModuleCommandPlusOne);

                Vector3 vector3 = new Vector3();
                //for the given module create a copy of it to randModule
                switch (commandType)
                {
                    case buttonCommand:
                        //get a random command text based on the easy/medium/hard arrays
                        newCommandText = GetRandomCommandText(commandType, ref buttonCommandArray_EASY, ref buttonCommandArray_MEDIUM, ref buttonCommandArray_HARD, ref usedCommandArray);
                        //set modules position/rotation based on which player it's for
                        if (playerNum == 1)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.45f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.08f);
                        if (playerNum == 2)
                            vector3 = new Vector3(playerControlDeck.transform.position.x + 0.08f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.45f + (0.3f * xOffset));
                        if (playerNum == 3)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.45f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z + 0.08f);
                        if (playerNum == 4)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.08f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.45f + (0.3f * xOffset));
                        randModuleEmpty = new GameObject();
                        randModuleEmpty.transform.position = vector3;
                        randModuleEmpty.transform.rotation = Quaternion.Euler(new Vector3(xQuaternion, yQuaternion, zQuaternion));
                        randModuleEmpty.name = "Button";
                        randModuleEmpty.transform.parent = playerControlDeck.transform.Find("Modules").transform;
                        break;
                    case dialCommand:
                        //get a random command text based on the easy/medium/hard arrays
                        newCommandText = GetRandomCommandText(commandType, ref dialCommandArray_EASY, ref dialCommandArray_MEDIUM, ref dialCommandArray_HARD, ref usedCommandArray);
                        //set modules position/rotation based on which player it's for
                        if (playerNum == 1)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.45f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.05f);
                        if (playerNum == 2)
                            vector3 = new Vector3(playerControlDeck.transform.position.x + 0.05f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.45f + (0.3f * xOffset));
                        if (playerNum == 3)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.45f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z + 0.05f);
                        if (playerNum == 4)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.05f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.45f + (0.3f * xOffset));
                        randModuleEmpty = new GameObject();
                        randModuleEmpty.transform.position = vector3;
                        randModuleEmpty.transform.rotation = Quaternion.Euler(new Vector3(xQuaternion - 90, yQuaternion + 90, zQuaternion));
                        randModuleEmpty.name = "Dial";
                        randModuleEmpty.transform.parent = playerControlDeck.transform.Find("Modules").transform;
                        break;
                    case lLeverCommand:
                        //get a random command text based on the easy/medium/hard arrays
                        newCommandText = GetRandomCommandText(commandType, ref lLeverCommandArray_EASY, ref lLeverCommandArray_MEDIUM, ref lLeverCommandArray_HARD, ref usedCommandArray);
                        //set modules position/rotation based on which player it's for
                        if (playerNum == 1)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.45f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.147f);
                        if (playerNum == 2)
                            vector3 = new Vector3(playerControlDeck.transform.position.x + 0.147f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.45f + (0.3f * xOffset));
                        if (playerNum == 3)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.45f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z + 0.147f);
                        if (playerNum == 4)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.147f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.45f + (0.3f * xOffset));
                        randModuleEmpty = new GameObject();
                        randModuleEmpty.transform.position = vector3;
                        randModuleEmpty.transform.rotation = Quaternion.Euler(new Vector3(xQuaternion, yQuaternion, zQuaternion));
                        randModuleEmpty.name = "L_Lever";
                        randModuleEmpty.transform.parent = playerControlDeck.transform.Find("Modules").transform;
                        break;
                    case lightswitchCommand:
                        //get a random command text based on the easy/medium/hard arrays
                        newCommandText = GetRandomCommandText(commandType, ref lightswitchCommandArray_EASY, ref lightswitchCommandArray_MEDIUM, ref lightswitchCommandArray_HARD, ref usedCommandArray);
                        //set modules position/rotation based on which player it's for
                        if (playerNum == 1)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.41f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.05f);
                        if (playerNum == 2)
                            vector3 = new Vector3(playerControlDeck.transform.position.x + 0.05f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.41f + (0.3f * xOffset));
                        if (playerNum == 3)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.41f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z + 0.05f);
                        if (playerNum == 4)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.05f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.41f + (0.3f * xOffset));
                        randModuleEmpty = new GameObject();
                        randModuleEmpty.transform.position = vector3;
                        randModuleEmpty.transform.rotation = Quaternion.Euler(new Vector3(xQuaternion, yQuaternion + 90, zQuaternion));
                        randModuleEmpty.name = "Lightswitch";
                        randModuleEmpty.transform.parent = playerControlDeck.transform.Find("Modules").transform;
                        break;
                    case plutoniumBatteryCommand:
                        //get a random command text based on the easy/medium/hard arrays
                        newCommandText = GetRandomCommandText(commandType, ref plutoniumBatteryCommandArray_EASY, ref plutoniumBatteryCommandArray_MEDIUM, ref plutoniumBatteryCommandArray_HARD, ref usedCommandArray);
                        //set modules position/rotation based on which player it's for
                        if (playerNum == 1)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.45f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.045f);
                        if (playerNum == 2)
                            vector3 = new Vector3(playerControlDeck.transform.position.x + 0.05f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.45f + (0.3f * xOffset));
                        if (playerNum == 3)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.45f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z + 0.05f);
                        if (playerNum == 4)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.05f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.45f + (0.3f * xOffset));
                        randModuleEmpty = new GameObject();
                        randModuleEmpty.transform.position = vector3;
                        randModuleEmpty.transform.rotation = Quaternion.Euler(new Vector3(xQuaternion, yQuaternion + 180, zQuaternion - 90));
                        randModuleEmpty.name = "Plutonium_Case";
                        randModuleEmpty.transform.parent = playerControlDeck.transform.Find("Modules").transform;
                        break;
                    case shifterCommand:
                        //get a random command text based on the easy/medium/hard arrays
                        newCommandText = GetRandomCommandText(commandType, ref shifterCommandArray_EASY, ref shifterCommandArray_MEDIUM, ref shifterCommandArray_HARD, ref usedCommandArray);
                        //set modules position/rotation based on which player it's for
                        if (playerNum == 1)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.45f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.05f);
                        if (playerNum == 2)
                            vector3 = new Vector3(playerControlDeck.transform.position.x + 0.05f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.45f + (0.3f * xOffset));
                        if (playerNum == 3)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.45f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z + 0.05f);
                        if (playerNum == 4)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.05f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.45f + (0.3f * xOffset));
                        randModuleEmpty = new GameObject();
                        randModuleEmpty.transform.position = vector3;
                        randModuleEmpty.transform.rotation = Quaternion.Euler(new Vector3(xQuaternion + 180, yQuaternion + 90, zQuaternion));
                        randModuleEmpty.name = "Shifter";
                        randModuleEmpty.transform.parent = playerControlDeck.transform.Find("Modules").transform;
                        break;
                    case sliderCommand:
                        //get a random command text based on the easy/medium/hard arrays
                        newCommandText = GetRandomCommandText(commandType, ref sliderCommandArray_EASY, ref sliderCommandArray_MEDIUM, ref sliderCommandArray_HARD, ref usedCommandArray);
                        //set modules position/rotation based on which player it's for
                        if (playerNum == 1)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.45f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.05f);
                        if (playerNum == 2)
                            vector3 = new Vector3(playerControlDeck.transform.position.x + 0.05f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.45f + (0.3f * xOffset));
                        if (playerNum == 3)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.45f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z + 0.05f);
                        if (playerNum == 4)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.05f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.45f + (0.3f * xOffset));
                        randModuleEmpty = new GameObject();
                        randModuleEmpty.transform.position = vector3;
                        randModuleEmpty.transform.rotation = Quaternion.Euler(new Vector3(xQuaternion + 270, yQuaternion + 90, zQuaternion));
                        randModuleEmpty.name = "Slider";
                        randModuleEmpty.transform.parent = playerControlDeck.transform.Find("Modules").transform;
                        break;
                    case valveCommand:
                        //get a random command text based on the easy/medium/hard arrays
                        newCommandText = GetRandomCommandText(commandType, ref valveCommandArray_EASY, ref valveCommandArray_MEDIUM, ref valveCommandArray_HARD, ref usedCommandArray);
                        //set modules position/rotation based on which player it's for
                        if (playerNum == 1)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.45f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.096f);
                        if (playerNum == 2)
                            vector3 = new Vector3(playerControlDeck.transform.position.x + 0.096f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.45f + (0.3f * xOffset));
                        if (playerNum == 3)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.45f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z + 0.096f);
                        if (playerNum == 4)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.096f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.45f + (0.3f * xOffset));
                        randModuleEmpty = new GameObject();
                        randModuleEmpty.transform.position = vector3;
                        randModuleEmpty.transform.rotation = Quaternion.Euler(new Vector3(xQuaternion, yQuaternion, zQuaternion + 90));
                        randModuleEmpty.name = "Valve";
                        randModuleEmpty.transform.parent = playerControlDeck.transform.Find("Modules").transform;
                        break;
                    case wLeverCommand:
                        //get a random command text based on the easy/medium/hard arrays
                        newCommandText = GetRandomCommandText(commandType, ref wLeverCommandArray_EASY, ref wLeverCommandArray_MEDIUM, ref wLeverCommandArray_HARD, ref usedCommandArray);
                        //set modules position/rotation based on which player it's for
                        if (playerNum == 1)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.45f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.152f);
                        if (playerNum == 2)
                            vector3 = new Vector3(playerControlDeck.transform.position.x + 0.152f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.45f + (0.3f * xOffset));
                        if (playerNum == 3)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.45f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z + 0.152f);
                        if (playerNum == 4)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.152f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.45f + (0.3f * xOffset));
                        randModuleEmpty = new GameObject();
                        randModuleEmpty.transform.position = vector3;
                        randModuleEmpty.transform.rotation = Quaternion.Euler(new Vector3(xQuaternion, yQuaternion, zQuaternion));
                        randModuleEmpty.name = "W_Lever";
                        randModuleEmpty.transform.parent = playerControlDeck.transform.Find("Modules").transform;
                        break;
                    default:
                        //get a random command text based on the easy/medium/hard arrays
                        newCommandText = GetRandomCommandText(commandType, ref buttonCommandArray_EASY, ref buttonCommandArray_MEDIUM, ref buttonCommandArray_HARD, ref usedCommandArray);
                        //set modules position/rotation based on which player it's for
                        if (playerNum == 1)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.45f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.09f);
                        if (playerNum == 2)
                            vector3 = new Vector3(playerControlDeck.transform.position.x + 0.09f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.45f + (0.3f * xOffset));
                        if (playerNum == 3)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.45f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z + 0.09f);
                        if (playerNum == 4)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.09f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.45f + (0.3f * xOffset));
                        randModuleEmpty = new GameObject();
                        randModuleEmpty.transform.position = vector3;
                        randModuleEmpty.transform.rotation = Quaternion.Euler(new Vector3(xQuaternion, yQuaternion, zQuaternion));
                        randModuleEmpty.name = "Button";
                        randModuleEmpty.transform.parent = playerControlDeck.transform.Find("Modules").transform;
                        break;
                }

                //Split command text into 2 lines if needed
                if (newCommandText.Length > commandTextRowLimit)
                {
                    string[] parts = newCommandText.Split(' ');
                    newCommandText = "";
                    int currentLineLength = 0;
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if ((currentLineLength + 1 + parts[i].Length) > commandTextRowLimit)
                        {
                            newCommandText += System.Environment.NewLine;
                            currentLineLength = 0;
                        }
                        newCommandText += " " + parts[i];
                        currentLineLength += parts[i].Length;
                    }
                }

                data[0] = newCommandText;
                data[1] = (intModuleListSize + ((x * gridY) + y)) * 100;
                data[2] = playerNum;

                //add randomModule to grid
                moduleType.module = randModuleEmpty;
                moduleType.data = data;
                outModuleList[intModuleListSize + ((x * gridY) + y)] = moduleType;
            }
        }

        return outModuleList;
    }

    private string GetRandomCommandText(int commandType, ref ArrayList commandArray_EASY, ref ArrayList commandArray_MEDIUM, ref ArrayList commandArray_HARD, ref ArrayList usedCommandArray)
    {
        //All arrays have been used for this command this game. Reload that command's arrays
        //Remove all used Commands in this level
        if (commandArray_EASY.Count == 0 && commandArray_MEDIUM.Count == 0 && commandArray_HARD.Count == 0)
        {
            Command_Array commandArray = GetComponent<Command_Array>();
            //Re-load the command arrays for the given commandType
            switch (commandType)
            {
                case buttonCommand:
                    //Button
                    commandArray_EASY = new ArrayList(commandArray.buttonCommandArray_EASY);
                    commandArray_MEDIUM = new ArrayList(commandArray.buttonCommandArray_MEDIUM);
                    commandArray_HARD = new ArrayList(commandArray.buttonCommandArray_HARD);
                    break;
                case dialCommand:
                    //Dial
                    commandArray_EASY = new ArrayList(commandArray.dialCommandArray_EASY);
                    commandArray_MEDIUM = new ArrayList(commandArray.dialCommandArray_MEDIUM);
                    commandArray_HARD = new ArrayList(commandArray.dialCommandArray_HARD);
                    break;
                case lLeverCommand:
                    //L_Lever
                    commandArray_EASY = new ArrayList(commandArray.lLeverCommandArray_EASY);
                    commandArray_MEDIUM = new ArrayList(commandArray.lLeverCommandArray_MEDIUM);
                    commandArray_HARD = new ArrayList(commandArray.lLeverCommandArray_HARD);
                    break;
                case lightswitchCommand:
                    //Lightswitch
                    commandArray_EASY = new ArrayList(commandArray.lightswitchCommandArray_EASY);
                    commandArray_MEDIUM = new ArrayList(commandArray.lightswitchCommandArray_MEDIUM);
                    commandArray_HARD = new ArrayList(commandArray.lightswitchCommandArray_HARD);
                    break;
                case plutoniumBatteryCommand:
                    //Plutonium Battery
                    plutoniumBatteryCommandArray_EASY = new ArrayList(commandArray.plutoniumBatteryCommandArray_EASY);
                    plutoniumBatteryCommandArray_MEDIUM = new ArrayList(commandArray.plutoniumBatteryCommandArray_MEDIUM);
                    plutoniumBatteryCommandArray_HARD = new ArrayList(commandArray.plutoniumBatteryCommandArray_HARD);
                    break;
                case shifterCommand:
                    //Dial
                    commandArray_EASY = new ArrayList(commandArray.shifterCommandArray_EASY);
                    commandArray_MEDIUM = new ArrayList(commandArray.shifterCommandArray_MEDIUM);
                    commandArray_HARD = new ArrayList(commandArray.shifterCommandArray_HARD);
                    break;
                case sliderCommand:
                    //Dial
                    commandArray_EASY = new ArrayList(commandArray.sliderCommandArray_EASY);
                    commandArray_MEDIUM = new ArrayList(commandArray.sliderCommandArray_MEDIUM);
                    commandArray_HARD = new ArrayList(commandArray.sliderCommandArray_HARD);
                    break;
                case valveCommand:
                    //Valve
                    commandArray_EASY = new ArrayList(commandArray.valveCommandArray_EASY);
                    commandArray_MEDIUM = new ArrayList(commandArray.valveCommandArray_MEDIUM);
                    commandArray_HARD = new ArrayList(commandArray.valveCommandArray_HARD);
                    break;
                case wLeverCommand:
                    //W_Lever
                    commandArray_EASY = new ArrayList(commandArray.wLeverCommandArray_EASY);
                    commandArray_MEDIUM = new ArrayList(commandArray.wLeverCommandArray_MEDIUM);
                    commandArray_HARD = new ArrayList(commandArray.wLeverCommandArray_HARD);
                    break;
                default:
                    Debug.LogError("Error - Command Type not found");
                    break;
            }

            //Remove all currently used commands this level so there are no repeats
            foreach (string usedCommand in usedCommandArray)
            {
                if (commandArray_EASY.Contains(usedCommand))
                    commandArray_EASY.Remove(usedCommand);
                else if (commandArray_MEDIUM.Contains(usedCommand))
                    commandArray_MEDIUM.Remove(usedCommand);
                else if (commandArray_HARD.Contains(usedCommand))
                    commandArray_HARD.Remove(usedCommand);
            }
        }

        string commandText = "";
        // roll to see the difficulty of the command to roll
        float difficulty = Random.Range(0.0f, 1.0f);
        // If the command array is empty for a given difficulty roll up to the next highest difficulty. 
        // If HARD is empty attempt to roll down to the next lowest difficulty
        if (commandArray_EASY.Count > 0 && difficulty <= easyPercent)
        {
            commandText = GenerateRandomCommandText(ref commandArray_EASY, ref usedCommandArray);
        }
        else if (commandArray_MEDIUM.Count > 0 && difficulty <= (easyPercent + mediumPercent))
        {
            commandText = GenerateRandomCommandText(ref commandArray_MEDIUM, ref usedCommandArray);
        }
        else if (commandArray_HARD.Count > 0)
        {
            commandText = GenerateRandomCommandText(ref commandArray_HARD, ref usedCommandArray);
        }
        else if (commandArray_MEDIUM.Count > 0)
        {
            commandText = GenerateRandomCommandText(ref commandArray_MEDIUM, ref usedCommandArray);
        }
        else if (commandArray_EASY.Count > 0)
        {
            commandText = GenerateRandomCommandText(ref commandArray_EASY, ref usedCommandArray);
        }
        else
        {
            //Should not get here
            //Should only be possible if all commands for the commandType are being used in the current level and you're trying to get more
            Debug.LogError("No command found");
            commandText = "Error";
        }

        return commandText;
    }

    private string GenerateRandomCommandText(ref ArrayList commandArray_DIFFICULTY, ref ArrayList usedCommandArray)
    {
        //roll to get random command
        int commandIndex = Random.Range(0, commandArray_DIFFICULTY.Count);
        string commandText = (string)commandArray_DIFFICULTY[commandIndex];
        usedCommandArray.Add(commandText);
        //remove selected command from the array so it won't be used again this level
        commandArray_DIFFICULTY.RemoveAt(commandIndex);

        return commandText;
    }

    //##################################################################################################################################
    //                                                      GAME OVER
    //##################################################################################################################################

    public void GameOver()
    {
        isGameOver = true;

        StopCoroutine("RunHazards");

        //Stop Timer object
        timerScript.StopTimer(true);

        UpdateAllConsoles("Level: " + level + " \n Final Score: " + totalScore);

        if (playerPosOccupied[0] == true)
            p1_scoreTextScript.photonView.RPC("GameOver", PhotonTargets.All, true);
        if (playerPosOccupied[1] == true)
            p2_scoreTextScript.photonView.RPC("GameOver", PhotonTargets.All, true);
        if (playerPosOccupied[2] == true)
            p3_scoreTextScript.photonView.RPC("GameOver", PhotonTargets.All, true);
        if (playerPosOccupied[3] == true)
            p4_scoreTextScript.photonView.RPC("GameOver", PhotonTargets.All, true);
    }

    //##################################################################################################################################
    //                                                        UPDATE
    //##################################################################################################################################

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.isMasterClient && !isLoadingNextLevel && !isGameOver)
        {
            //If the user scores grater than or equal to the score to win change text to Green and to say "YOU WIN~"
            //Else if the score is less than or equal to the score to lose change the text to Red and say "Game Over"
            if (levelScore >= scoreToWin)
            {
                //Stop Timer
                timerScript.StopTimer(false);

                StopCoroutine("RunHazards");
                ResetHazards();

                //Increase total score by (level * 10) & % time remaning in round
                totalScore += level * 10;
                System.TimeSpan ts = (System.DateTime.Now - levelStartTime);
                totalScore += 100 - (int)((ts.Seconds / levelTimeoutSeconds) * 100);

                StartCoroutine(LoadNextLevel());
            }
            else if (levelScore <= scoreToLose)
            {
                Debug.Log("Game Over; score: " + levelScore + ", scoreToLose: " + scoreToLose);
                GameOver();
            }
            else if (levelStartTime.AddSeconds(levelTimeoutSeconds) <= System.DateTime.Now)
            {
                Debug.Log("Game Over; levelTimeoutSeconds: " + levelTimeoutSeconds + ", levelStartTime: " + levelStartTime + ", Now: " + System.DateTime.Now);
                GameOver();
            }

            //If we are not loading the next level
            //AND if we are NOT typing "START!"
            //AND if we are NOT currently typing a command
            //AND if we are NOT currently waiting the 10 seconds for a command to pass
            //generate and display a new random command
            if (!isGameOver)
            {
                if (playerPosOccupied[0] == true)
                {
                    if (!p1_isDisplayStart && !p1_consoleTextScript.isTyping && !p1_isDisplayingCommand)
                        StartCoroutine(P1_DisplayRandomCommand());
                }
                if (playerPosOccupied[1] == true)
                {
                    if (!p2_isDisplayStart && !p2_consoleTextScript.isTyping && !p2_isDisplayingCommand)
                        StartCoroutine(P2_DisplayRandomCommand());
                }
                if (playerPosOccupied[2] == true)
                {
                    if (!p3_isDisplayStart && !p3_consoleTextScript.isTyping && !p3_isDisplayingCommand)
                        StartCoroutine(P3_DisplayRandomCommand());
                }
                if (playerPosOccupied[3] == true)
                {
                    if (!p4_isDisplayStart && !p4_consoleTextScript.isTyping && !p4_isDisplayingCommand)
                        StartCoroutine(P4_DisplayRandomCommand());
                }
            }
        }
    }

    public void ScoreUp()
    {
        if (!isGameOver)
        {
            levelScore++;
            totalScore++;
            UpdateScore();
        }
    }

    public void ScoreDown()
    {
        if (!isGameOver)
        {
            levelScore--;
            totalScore--;
            UpdateScore();
        }
    }

    public void UpdateScore()
    {
        if (playerPosOccupied[0] == true)
            p1_scoreTextScript.photonView.RPC("UpdateScore", PhotonTargets.All, level, levelScore, scoreToWin);
        if (playerPosOccupied[1] == true)
            p2_scoreTextScript.photonView.RPC("UpdateScore", PhotonTargets.All, level, levelScore, scoreToWin);
        if (playerPosOccupied[2] == true)
            p3_scoreTextScript.photonView.RPC("UpdateScore", PhotonTargets.All, level, levelScore, scoreToWin);
        if (playerPosOccupied[3] == true)
            p4_scoreTextScript.photonView.RPC("UpdateScore", PhotonTargets.All, level, levelScore, scoreToWin);
    }

    public void UpdateAllConsoles(string msg)
    {
        if (playerPosOccupied[0] == true)
            p1_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, msg);
        if (playerPosOccupied[1] == true)
            p2_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, msg);
        if (playerPosOccupied[2] == true)
            p3_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, msg);
        if (playerPosOccupied[3] == true)
            p4_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, msg);
    }

    //##################################################################################################################################
    //                                                  GENERATE NEXT COMMAND
    //##################################################################################################################################

    IEnumerator P1_DisplayRandomCommand()
    {
        p1_isDisplayingCommand = true;

        //Clear text
        p1_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, "");
        yield return 0;
        yield return new WaitForSeconds(1);

        //Roll random number to decide new command from the module list
        p1_rCommand = Random.Range(0, moduleList.Length);

        //get module from module list
        GameObject module = moduleList[p1_rCommand].module;

        bool isCommandSet = false;
        try
        {
            string[] rCommandList = GetRandomCommand(module, p1_rCommand);
            p1_rCommand = int.Parse(rCommandList[0]);
            string message = rCommandList[1];

            //Type out new command to console
            p1_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, message);

            p1_CommandFeedbackScript.Reset();

            isCommandSet = true;
        }
        catch
        {
            Debug.LogWarning("P1_DisplayRandomCommand error, unable to get command");
            isCommandSet = false;
        }

        //Only wait for the command if it was set correctly
        if (isCommandSet)
        {
            //Wait for the commandTimeoutSeconds or if a button gets tapped
            yield return WaitForSecondsOrTap(1, commandTimeoutSeconds);
        }

        p1_isDisplayingCommand = false;
    }

    IEnumerator P2_DisplayRandomCommand()
    {
        p2_isDisplayingCommand = true;

        //Clear text
        p2_consoleTextScript.GetComponent<TextMesh>().text = "";
        yield return 0;
        yield return new WaitForSeconds(1);

        //Roll random number to decide new command from the module list
        p2_rCommand = Random.Range(0, moduleList.Length);

        //get module from module list
        GameObject module = moduleList[p2_rCommand].module;

        bool isCommandSet = false;
        try
        {
            string[] rCommandList = GetRandomCommand(module, p2_rCommand);
            p2_rCommand = int.Parse(rCommandList[0]);
            string message = rCommandList[1];

            //Type out new command to console
            p2_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, message);

            p2_CommandFeedbackScript.Reset();

            isCommandSet = true;
        }
        catch
        {
            Debug.LogWarning("P2_DisplayRandomCommand error, unable to get command");
            isCommandSet = false;
        }

        //Only wait for the command if it was set correctly
        if (isCommandSet)
        {
            //Wait for the commandTimeoutSeconds or if a button gets tapped
            yield return WaitForSecondsOrTap(2, commandTimeoutSeconds);
        }

        p2_isDisplayingCommand = false;
    }

    IEnumerator P3_DisplayRandomCommand()
    {
        p3_isDisplayingCommand = true;

        //Clear text
        p3_consoleTextScript.GetComponent<TextMesh>().text = "";
        yield return 0;
        yield return new WaitForSeconds(1);

        //Roll random number to decide new command from the module list
        p3_rCommand = Random.Range(0, moduleList.Length);

        //get module from module list
        GameObject module = moduleList[p3_rCommand].module;

        bool isCommandSet = false;
        try
        {
            string[] rCommandList = GetRandomCommand(module, p3_rCommand);
            p3_rCommand = int.Parse(rCommandList[0]);
            string message = rCommandList[1];

            //Type out new command to console
            p3_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, message);

            p3_CommandFeedbackScript.Reset();

            isCommandSet = true;
        }
        catch
        {
            Debug.LogWarning("P3_DisplayRandomCommand error, unable to get command");
            isCommandSet = false;
        }

        //Only wait for the command if it was set correctly
        if (isCommandSet)
        {
            //Wait for the commandTimeoutSeconds or if a button gets tapped
            yield return WaitForSecondsOrTap(3, commandTimeoutSeconds);
        }

        p3_isDisplayingCommand = false;
    }

    IEnumerator P4_DisplayRandomCommand()
    {
        p4_isDisplayingCommand = true;

        //Clear text
        p4_consoleTextScript.GetComponent<TextMesh>().text = "";
        yield return 0;
        yield return new WaitForSeconds(1);

        //Roll random number to decide new command from the module list
        p4_rCommand = Random.Range(0, moduleList.Length);

        //get module from module list
        GameObject module = moduleList[p4_rCommand].module;

        bool isCommandSet = false;
        try
        {
            string[] rCommandList = GetRandomCommand(module, p4_rCommand);
            p4_rCommand = int.Parse(rCommandList[0]);
            string message = rCommandList[1];

            //Type out new command to console
            p4_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, message);

            p4_CommandFeedbackScript.Reset();

            isCommandSet = true;
        }
        catch
        {
            Debug.LogWarning("P4_DisplayRandomCommand error, unable to get command");
            isCommandSet = false;
        }

        //Only wait for the command if it was set correctly
        if (isCommandSet)
        {
            //Wait for the commandTimeoutSeconds or if a button gets tapped
            yield return WaitForSecondsOrTap(4, commandTimeoutSeconds);
        }

        p4_isDisplayingCommand = false;
    }

    string[] GetRandomCommand(GameObject module, int rCommand)
    {
        int commandType = buttonCommand;
        if (module.name.Contains("Button"))
            commandType = buttonCommand;
        else if (module.name.Contains("Dial"))
            commandType = dialCommand;
        else if (module.name.Contains("L_Lever"))
            commandType = lLeverCommand;
        else if (module.name.Contains("Lightswitch"))
            commandType = lightswitchCommand;
        else if (module.name.Contains("Plutonium"))
            commandType = plutoniumBatteryCommand;
        else if (module.name.Contains("Pullcord"))
            commandType = pullcordCommand;
        else if (module.name.Contains("Shifter"))
            commandType = shifterCommand;
        else if (module.name.Contains("Slider"))
            commandType = sliderCommand;
        else if (module.name.Contains("Valve"))
            commandType = valveCommand;
        else if (module.name.Contains("W_Lever"))
            commandType = wLeverCommand;

        string message = "";
        int newRCommand;
        rCommand = rCommand * 100;
        //Get new command
        switch (commandType)
        {
            case buttonCommand:
                //Button
                string buttonText = module.GetComponent<Button_Script>().newName;
                message = "Engage " + buttonText;
                break;
            case dialCommand:
                //Dial
                Dial_Script dialScript = module.GetComponent<Dial_Script>();
                string dialText = dialScript.newName;
                newRCommand = Random.Range(0, 6);
                while (newRCommand == dialScript.dialPosition)
                {
                    newRCommand = Random.Range(0, 6);
                }
                message = "Change " + dialText + " to Ch. " + newRCommand;
                rCommand += newRCommand;
                break;
            case lLeverCommand:
                //L_Lever
                L_Lever_Script lLeverScript = module.GetComponent<L_Lever_Script>();
                string lLeverText = lLeverScript.newName;
                message = "Turn ";
                if (lLeverScript.isLLeverUp)
                {
                    message += "OFF ";
                    rCommand += 1;
                }
                else
                {
                    message += "ON ";
                    rCommand += 2;
                }
                message += lLeverText;
                break;
            case lightswitchCommand:
                //Lightswitch
                Lightswitch_Script lightswitchScript = module.GetComponent<Lightswitch_Script>();
                string lightswitchText = lightswitchScript.newName;
                message = "Turn ";
                if (lightswitchScript.isLightswitchOn)
                {
                    message += "OFF ";
                    rCommand += 1;
                }
                else
                {
                    message += "ON ";
                    rCommand += 2;
                }
                message += lightswitchText;
                break;
            case plutoniumBatteryCommand:
                //Plutonium Battery
                Plutonium_Case_Script plutoniumCaseScript = module.GetComponent<Plutonium_Case_Script>();
                string plutoniumBatteryText = plutoniumCaseScript.newName;
                message = "Power " + plutoniumBatteryText;
                break;
            case pullcordCommand:
                message = pullcordCommandText;
                rCommand = pullcordCommand;
                break;
            case shifterCommand:
                //Dial
                Shifter_Script shifterScript = module.GetComponent<Shifter_Script>();
                string shifterText = shifterScript.newName;
                newRCommand = Random.Range(0, 3);
                while (newRCommand == shifterScript.shifterPosition)
                {
                    newRCommand = Random.Range(0, 3);
                }
                message = "";
                if (newRCommand < shifterScript.shifterPosition)
                {
                    message += "Decrease ";
                }
                else
                {
                    message += "Increase ";
                }
                message += shifterText + " to " + newRCommand;
                rCommand += newRCommand;
                break;
            case sliderCommand:
                //Dial
                Slider_Script sliderScript = module.GetComponent<Slider_Script>();
                string sliderText = sliderScript.newName;
                newRCommand = Random.Range(0, 4);
                while (newRCommand == sliderScript.sliderPosition)
                {
                    newRCommand = Random.Range(0, 4);
                }
                message = "";
                if (newRCommand < sliderScript.sliderPosition)
                {
                    message += "Reduce ";
                }
                else
                {
                    message += "Boost ";
                }
                message += sliderText + " to " + newRCommand;
                rCommand += newRCommand;
                break;
            case valveCommand:
                //Valve
                Valve_Script valveScript = module.GetComponent<Valve_Script>();
                string valveText = valveScript.newName;
                newRCommand = Random.Range(0, 2);
                message = "";
                if (newRCommand == 0)
                {
                    message += "Tighten ";
                    rCommand += 1;
                }
                else
                {
                    message += "Loosen ";
                    rCommand += 2;
                }
                message += valveText;
                break;
            case wLeverCommand:
                //W_Lever
                string wLeverText = module.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>().text;
                W_Lever_Script wLeverHandleScript = module.GetComponent<W_Lever_Script>();
                message = "";
                if (wLeverHandleScript.isWLeverUp)
                {
                    message += "Lower ";
                    rCommand += 1;
                }
                else
                {
                    message += "Raise ";
                    rCommand += 2;
                }
                message += wLeverText;
                break;
            default:
                Debug.LogError("Error - name: " + module.name);
                break;
        }

        return new string[2] { rCommand.ToString(), message };
    }

    //##################################################################################################################################
    //                                                      WAIT FOR COMMAND
    //##################################################################################################################################

    // Custom WaitForSeconds
    // This will either wait for the given seconds, or until the TappedWaitForSecondsOrTap forces the timer to zero
    IEnumerator WaitForSecondsOrTap(int player, float seconds)
    {
        if (!isGameOver)
        {
            if (player == 1)
            {
                p1_gWaitSystem = seconds;
                p1_consoleTimerScript.StartTimer(seconds);
                while (p1_gWaitSystem > 0.0)
                {
                    p1_gWaitSystem -= Time.deltaTime;
                    if (p1_gWaitSystem == 0)
                    {
                        p1_CommandFeedbackScript.PlayFailFeedback();
                    }
                    yield return 0;
                }
                p1_consoleTimerScript.StopTimer(true);
            }
            else if (player == 2)
            {
                p2_gWaitSystem = seconds;
                p2_consoleTimerScript.StartTimer(seconds);
                while (p2_gWaitSystem > 0.0)
                {
                    p2_gWaitSystem -= Time.deltaTime;
                    if (p2_gWaitSystem == 0)
                    {
                        p2_CommandFeedbackScript.PlayFailFeedback();
                    }
                    yield return 0;
                }
                p2_consoleTimerScript.StopTimer(true);
            }
            else if (player == 3)
            {
                p3_gWaitSystem = seconds;
                p3_consoleTimerScript.StartTimer(seconds);
                while (p3_gWaitSystem > 0.0)
                {
                    p3_gWaitSystem -= Time.deltaTime;
                    if (p3_gWaitSystem == 0)
                    {
                        p3_CommandFeedbackScript.PlayFailFeedback();
                    }
                    yield return 0;
                }
                p3_consoleTimerScript.StopTimer(true);
            }
            else if (player == 4)
            {
                p4_gWaitSystem = seconds;
                p4_consoleTimerScript.StartTimer(seconds);
                while (p4_gWaitSystem > 0.0)
                {
                    p4_gWaitSystem -= Time.deltaTime;
                    if (p4_gWaitSystem == 0)
                    {
                        p4_CommandFeedbackScript.PlayFailFeedback();
                    }
                    yield return 0;
                }
                p4_consoleTimerScript.StopTimer(true);
            }

            //lower score if time reached (button was not tapped)
            if (!isGameOver && !isLoadingNextLevel)
            {
                if (numFufilled == 0)
                {
                    ScoreDown();
                }
                else
                {
                    numFufilled -= 1;
                }
            }
        }
    }

    // End the waitForSeconds by setting the timer to zero AND signal that a button was tapped (isTapped = true)
    public void TappedWaitForSecondsOrTap(int inputCommand, int playerNum)
    {
        if (!isGameOver)
        {
            numFufilled = 0;
            bool isActivePullcordCommand = false;
            bool isAllPullcordDown = false;

            //Debug.Log("p1_command = " + p1_rCommand + " inputCommand = " + inputCommand);

            //Check to see if command is Pullcord (global), at least one players console wants you to pull the cord, and that all players are pulling down
            if (inputCommand == pullcordCommand && (p1_rCommand == pullcordCommand || p2_rCommand == pullcordCommand || p3_rCommand == pullcordCommand || p4_rCommand == pullcordCommand))
            {
                isActivePullcordCommand = true;
                //Loop through all scripts and see if all players are pulling down
                int numDown = 0;
                foreach (Pullcord_Script pScript in pullcordScriptList)
                {
                    if (pScript.isDown)
                        numDown++;
                }
                if (numDown == pullcordScriptList.Length)
                    isAllPullcordDown = true;
            }

            //Check to see if the current command is the correct button pressed. Update score accordingly
            //if command is for a pullcord make sure all players have pulled down
            if ((inputCommand != pullcordCommand && p1_rCommand == inputCommand) ||
                (inputCommand == pullcordCommand && p1_rCommand == inputCommand && isAllPullcordDown))
            {
                p1_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, "");
                ScoreUp();
                //Set timer for that player to 0 to get next command
                p1_gWaitSystem = 0.0f;
                numFufilled += 1;
            }

            if ((inputCommand != pullcordCommand && p2_rCommand == inputCommand) ||
                (inputCommand == pullcordCommand && p2_rCommand == inputCommand && isAllPullcordDown))
            {
                p2_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, "");
                ScoreUp();
                //Set timer for that player to 0 to get next command
                p2_gWaitSystem = 0.0f;
                numFufilled += 1;
            }

            if ((inputCommand != pullcordCommand && p3_rCommand == inputCommand) ||
                (inputCommand == pullcordCommand && p3_rCommand == inputCommand && isAllPullcordDown))
            {
                p3_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, "");
                ScoreUp();
                //Set timer for that player to 0 to get next command
                p3_gWaitSystem = 0.0f;
                numFufilled += 1;
            }

            if ((inputCommand != pullcordCommand && p4_rCommand == inputCommand) ||
                (inputCommand == pullcordCommand && p4_rCommand == inputCommand && isAllPullcordDown))
            {
                p4_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, "");
                ScoreUp();
                //Set timer for that player to 0 to get next command
                p4_gWaitSystem = 0.0f;
                numFufilled += 1;
            }

            // If no command matched (and someone is not trying to fufill a pullcord) lower score
            if (numFufilled == 0 && !isActivePullcordCommand)
            {
                ScoreDown();
                if (playerNum == 1)
                    p1_CommandFeedbackScript.PlayFailFeedback();
                else if (playerNum == 2)
                    p2_CommandFeedbackScript.PlayFailFeedback();
                else if (playerNum == 3)
                    p3_CommandFeedbackScript.PlayFailFeedback();
                else if (playerNum == 4)
                    p4_CommandFeedbackScript.PlayFailFeedback();
            }
            else
            {
                if (isActivePullcordCommand)
                {
                    // Only play success feedback if all pullcards are down
                    // else don't play anything as we're still waiting for all to be down
                    if (isAllPullcordDown)
                    {
                        if (playerPosOccupied[0] == true)
                            p1_CommandFeedbackScript.PlaySuccessFeedback();
                        if (playerPosOccupied[1] == true)
                            p2_CommandFeedbackScript.PlaySuccessFeedback();
                        if (playerPosOccupied[2] == true)
                            p3_CommandFeedbackScript.PlaySuccessFeedback();
                        if (playerPosOccupied[3] == true)
                            p4_CommandFeedbackScript.PlaySuccessFeedback();
                    }
                }
                else if (playerNum == 1)
                    p1_CommandFeedbackScript.PlaySuccessFeedback();
                else if (playerNum == 2)
                    p2_CommandFeedbackScript.PlaySuccessFeedback();
                else if (playerNum == 3)
                    p3_CommandFeedbackScript.PlaySuccessFeedback();
                else if (playerNum == 4)
                    p4_CommandFeedbackScript.PlaySuccessFeedback();
            }
        }
    }

    public void AbortCheck(bool abortStatus, int playerNum)
    {
        Debug.Log("Starting AbortCheck");
        int abortCount = 0;
        int playerCount = PhotonNetwork.playerList.Length;
        if (abortStatus)
        {
            Debug.Log("Entered Abort Status Check");
            if (playerNum == 1)
            {
                p1_AbortResetScript.p1_Aborting();
                p1_Aborting = true;
            }
            else if (playerNum == 2)
            {
                p2_AbortResetScript.p2_Aborting();
                p2_Aborting = true;
            }
            else if (playerNum == 3)
            {
                p3_AbortResetScript.p3_Aborting();
                p3_Aborting = true;
            }
            else if (playerNum == 4)
            {
                p4_AbortResetScript.p4_Aborting();
                p4_Aborting = true;
            }
        }
        else
        {
            if (playerNum == 1)
            {
                p1_AbortResetScript.p1_NotAborting();
                p1_Aborting = false;
            }
            else if (playerNum == 2)
            {
                p2_AbortResetScript.p2_NotAborting();
                p2_Aborting = false;
            }
            else if (playerNum == 3)
            {
                p3_AbortResetScript.p3_NotAborting();
                p3_Aborting = false;
            }
            else if (playerNum == 4)
            {
                p4_AbortResetScript.p4_NotAborting();
                p4_Aborting = false;
            }
        }

        Debug.Log("Player 1 Abort Status: " + p1_Aborting);
        Debug.Log("Player 2 Abort Status: " + p2_Aborting);
        Debug.Log("Player 3 Abort Status: " + p3_Aborting);
        Debug.Log("Player 4 Abort Status: " + p4_Aborting);

        if (p1_Aborting == true)
        {
            abortCount += 1;
        }
        if (p2_Aborting == true)
        {
            abortCount += 1;
        }
        if (p3_Aborting == true)
        {
            abortCount += 1;
        }
        if (p4_Aborting == true)
        {
            abortCount += 1;
        }

        Debug.Log("Abort Count: " + abortCount);
        Debug.Log("Player Count: " + playerCount);

        if (abortCount == playerCount)
        {
            AbortGame();
        }
    }

    public void ResetCheck(bool resetStatus, int playerNum)
    {
        Debug.Log("Starting ResetCheck");
        int ResetCount = 0;
        int playerCount = PhotonNetwork.playerList.Length;
        if (resetStatus)
        {
            if (playerNum == 1)
            {
                p1_AbortResetScript.p1_Resetting();
                p1_Resetting = true;
            }
            else if (playerNum == 2)
            {
                p2_AbortResetScript.p2_Resetting();
                p2_Resetting = true;
            }
            else if (playerNum == 3)
            {
                p3_AbortResetScript.p3_Resetting();
                p3_Resetting = true;
            }
            else if (playerNum == 4)
            {
                p4_AbortResetScript.p4_Resetting();
                p4_Resetting = true;
            }
        }
        else
        {
            if (playerNum == 1)
            {
                p1_AbortResetScript.p1_NotResetting();
                p1_Resetting = false;
            }
            else if (playerNum == 2)
            {
                p2_AbortResetScript.p2_NotResetting();
                p2_Resetting = false;
            }
            else if (playerNum == 3)
            {
                p3_AbortResetScript.p3_NotResetting();
                p3_Resetting = false;
            }
            else if (playerNum == 4)
            {
                p4_AbortResetScript.p4_NotResetting();
                p4_Resetting = false;
            }
        }

        Debug.Log("Player 1 Reset Status: " + p1_Resetting);
        Debug.Log("Player 2 Reset Status: " + p2_Resetting);
        Debug.Log("Player 3 Reset Status: " + p3_Resetting);
        Debug.Log("Player 4 Reset Status: " + p4_Resetting);

        if (p1_Resetting == true)
        {
            ResetCount += 1;
        }
        if (p2_Resetting == true)
        {
            ResetCount += 1;
        }
        if (p3_Resetting == true)
        {
            ResetCount += 1;
        }
        if (p4_Resetting == true)
        {
            ResetCount += 1;
        }

        Debug.Log("Reset Count: " + ResetCount);
        Debug.Log("Player Count: " + playerCount);

        if (ResetCount == playerCount)
        {
            ResetGame();
        }
    }

    public void ResetGame()
    {
        //PhotonNetwork.LeaveRoom();
        if (PhotonNetwork.isMasterClient)
        {
            ResetHazards();

            GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
            foreach (GameObject o in objects)
            {
                if (o.GetComponent<PhotonView>() != null && !o.tag.Equals("Player") && !o.tag.Equals("EarsMouth") && !o.tag.Equals("MainCamera") && !o.tag.Equals("Console_Text"))
                {
                    if (!o.GetPhotonView().isMine)
                    {
                        o.GetPhotonView().RPC("RPCDestroy", PhotonTargets.Others, null);
                    }
                    PhotonNetwork.Destroy(o);
                }
            }
            PhotonNetwork.LoadLevel(PhotonMainMenu.SceneNameLobbyRoom);
        }
    }

    public void AbortGame()
    {
        ResetHazards();
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.GetPhotonView().RPC("RpcLeaveRoom", PhotonTargets.Others);
        }
        PhotonNetwork.LeaveRoom();
    }

    //##################################################################################################################################
    //                                                          Hazards
    //##################################################################################################################################

    private void IntantiateHazards()
    {
        //Create a list of transforms for the different hazards
        System.Collections.Generic.List<Transform> plutoniumLights = new System.Collections.Generic.List<Transform>();
        System.Collections.Generic.List<Transform> fogValves = new System.Collections.Generic.List<Transform>();
        System.Collections.Generic.List<Transform> resizeButtons = new System.Collections.Generic.List<Transform>();
        System.Collections.Generic.List<Transform> staticLevers = new System.Collections.Generic.List<Transform>();

        //Populate the hazard transform lists with only the transfroms from the players in the game
        if (playerPosOccupied[0] == true)
        {
            plutoniumLights.Add(p1_BackPanel.transform.Find("Hazards/Hazard Controls/Plutonium Lights"));
            fogValves.Add(p1_BackPanel.transform.Find("Hazards/Hazard Controls/Fog Valve"));
            resizeButtons.Add(p1_BackPanel.transform.Find("Hazards/Hazard Controls/RESIZE Button"));
            staticLevers.Add(p1_BackPanel.transform.Find("Hazards/Hazard Controls/Static Lever"));
        }
        if (playerPosOccupied[1] == true)
        {
            plutoniumLights.Add(p2_BackPanel.transform.Find("Hazards/Hazard Controls/Plutonium Lights"));
            fogValves.Add(p2_BackPanel.transform.Find("Hazards/Hazard Controls/Fog Valve"));
            resizeButtons.Add(p2_BackPanel.transform.Find("Hazards/Hazard Controls/RESIZE Button"));
            staticLevers.Add(p2_BackPanel.transform.Find("Hazards/Hazard Controls/Static Lever"));
        }
        if (playerPosOccupied[2] == true)
        {
            plutoniumLights.Add(p3_BackPanel.transform.Find("Hazards/Hazard Controls/Plutonium Lights"));
            fogValves.Add(p3_BackPanel.transform.Find("Hazards/Hazard Controls/Fog Valve"));
            resizeButtons.Add(p3_BackPanel.transform.Find("Hazards/Hazard Controls/RESIZE Button"));
            staticLevers.Add(p3_BackPanel.transform.Find("Hazards/Hazard Controls/Static Lever"));
        }
        if (playerPosOccupied[3] == true)
        {
            plutoniumLights.Add(p4_BackPanel.transform.Find("Hazards/Hazard Controls/Plutonium Lights"));
            fogValves.Add(p4_BackPanel.transform.Find("Hazards/Hazard Controls/Fog Valve"));
            resizeButtons.Add(p4_BackPanel.transform.Find("Hazards/Hazard Controls/RESIZE Button"));
            staticLevers.Add(p4_BackPanel.transform.Find("Hazards/Hazard Controls/Static Lever"));
        }

        //Instantiate one of the hazards on an existing player's Back Panel
        int plPosition = Random.Range(0, numPlayers);
        int fvPosition = Random.Range(0, numPlayers);
        int rbPosition = Random.Range(0, numPlayers);
        int slPosition = Random.Range(0, numPlayers);

        //Get player num so the Hazard can get the correct Back Panel parent
        object[] plData = new object[1];
        object[] fvData = new object[1];
        object[] rbData = new object[1];
        object[] slData = new object[1];
        int count = -1;
        int plPlayerNum = -1;
        int fvPlayerNum = -1;
        int rbPlayerNum = -1;
        int slPlayerNum = -1;
        for (int i=0; i<playerPosOccupied.Length; i++)
        {
            if (playerPosOccupied[i])
                count++;
            if (plPlayerNum == -1 && count == plPosition)
                plPlayerNum = i + 1;
            if (fvPlayerNum == -1 && count == fvPosition)
                fvPlayerNum = i + 1;
            if (rbPlayerNum == -1 && count == rbPosition)
                rbPlayerNum = i + 1;
            if (slPlayerNum == -1 && count == slPosition)
                slPlayerNum = i + 1;
        }
        plData[0] = plPlayerNum;
        fvData[0] = fvPlayerNum;
        rbData[0] = rbPlayerNum;
        slData[0] = slPlayerNum;

        plutoniumLight = PhotonNetwork.InstantiateSceneObject("Hazards/Plutonium Lights", plutoniumLights[plPosition].position, plutoniumLights[plPosition].rotation, 0, plData);
        fogValve = PhotonNetwork.InstantiateSceneObject("Hazards/Fog Valve", fogValves[fvPosition].position, fogValves[fvPosition].rotation, 0, fvData);
        resizeButton = PhotonNetwork.InstantiateSceneObject("Hazards/RESIZE Button", resizeButtons[rbPosition].position, resizeButtons[rbPosition].rotation, 0, rbData);
        staticLever = PhotonNetwork.InstantiateSceneObject("Hazards/Static Lever", staticLevers[slPosition].position, staticLevers[slPosition].rotation, 0, slData);

        isWaitingForHazard = false;
    }

    void DestroyAllHazards()
    {
        //Destroy all Modules on all clients
        if (hazardsList != null)
        {
            foreach (GameObject hazard in hazardsList)
            {
                hazard.GetPhotonView().RPC("RPCDestroy", PhotonTargets.All);
            }
        }
    }

    IEnumerator RunHazards()
    {
        // Continously run starting a new Hazard at random intervals based on level difficulty
        while(true)
        {
            if (!isWaitingForHazard && !IsAllHazardsActive())
            {
                isWaitingForHazard = true;

                //Get maximum amount of time to wait for the next hazard (minimum of 20 seconds)
                float maxWait = (1f - hardPercent) * levelTimeoutSeconds;
                if (maxWait < 20)
                    maxWait = 20f;
                
                float randWait = Random.Range((maxWait / 3f), maxWait);

                //Wait a random amount of time before firing off the next Hazard
                yield return new WaitForSeconds(randWait);

                //Roll for which Hazard to start (don't start one that's already going)
                int randHazardIndex = 0;
                if (!IsAllHazardsActive())
                {
                    do
                    {
                        randHazardIndex = Random.Range(0, numHazards);
                    } while (activeHazardsList[randHazardIndex]);

                    isWaitingForHazard = false;

                    photonView.RPC("StartHazards", PhotonTargets.All, randHazardIndex);
                }
            }
            else
            {
                // Hazard already running, wait 1 second before you check again
                yield return new WaitForSeconds(1);
            }
        }
    }

    private bool IsAllHazardsActive()
    {
        bool isAllActive = true;
        foreach (bool h in activeHazardsList)
        {
            if (!h)
            {
                isAllActive = false;
                break;
            }
        }
        return isAllActive;
    }

    [PunRPC]
    void StartHazards(int randHazardIndex)
    {
        Debug.Log("StartHazards: " + randHazardIndex);
        activeHazardsList[randHazardIndex] = true;

        if (randHazardIndex == ambientLightIndex)
        {
            //Turn off ambient lights
            RenderSettings.ambientLight = Color.black;
            directionalLight.intensity = 0f;
        }
        else if (randHazardIndex == fogValveIndex)
        {
            //Increase the fog density
            isIncreasingFog = true;
            StartCoroutine("IncreaseFog");
            //RenderSettings.fogDensity = defaultFogDensity * 50;
        }
        else if (randHazardIndex == resizeButtonIndex)
        {
            //Increase the scale of all Player objects by 3x
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                player.transform.localScale = player.transform.localScale * 2.5f;
        }
        else if (randHazardIndex == staticLeverIndex)
        {
            // Turn up static (lower the static lever)
            if (PhotonNetwork.isMasterClient)
                staticLever.GetPhotonView().RPC("RPCLowerHandle", PhotonTargets.All, null);
            // For each Main Camera that has the script "NoiseAndGrain" turn it on
            foreach (GameObject camera in GameObject.FindGameObjectsWithTag("MainCamera"))
            {
                if (camera.GetComponent<UnityStandardAssets.ImageEffects.NoiseAndGrain>() != null)
                {
                    camera.GetComponent<UnityStandardAssets.ImageEffects.NoiseAndGrain>().enabled = true;
                }
            }
            staticAudio.Play();
        }
        //else if (randHazardIndex == gravityIndex)
        //{
        //    //Turn off gravity (set the lever as OFF)
        //    if(PhotonNetwork.isMasterClient)
        //        gravityLever.GetPhotonView().RPC("RPCLowerHandle", PhotonTargets.All, null);
        //    Physics.gravity = Vector3.zero;
        //}
    }

    IEnumerator IncreaseFog()
    {
        float elapsedTime = 0f;
        float totalTime = 6.0f;
        float startingFogDensity = RenderSettings.fogDensity;
        while (RenderSettings.fogDensity <= (defaultFogDensity * 50) && isIncreasingFog)
        {
            elapsedTime += Time.deltaTime;
            RenderSettings.fogDensity = Mathf.Lerp(startingFogDensity, (defaultFogDensity * 50), (elapsedTime / totalTime));
            yield return null;
        }
    }

    IEnumerator DecreaseFog()
    {
        float elapsedTime = 0f;
        float totalTime = 3.0f;
        float startingFogDensity = RenderSettings.fogDensity;
        while (RenderSettings.fogDensity >= defaultFogDensity && !isIncreasingFog)
        {
            elapsedTime += Time.deltaTime;
            RenderSettings.fogDensity = Mathf.Lerp(startingFogDensity, defaultFogDensity, (elapsedTime / totalTime));
            yield return null;
        }
    }

    private void ResetHazards()
    {
        photonView.RPC("TurnOnAmbientLight", PhotonTargets.All, null);
        photonView.RPC("VentFog", PhotonTargets.All, null);
        photonView.RPC("RESIZENormal", PhotonTargets.All, null);
        photonView.RPC("TurnOffStatic", PhotonTargets.All, null);
        staticLever.GetPhotonView().RPC("RPCRaiseHandle", PhotonTargets.All, null);
        //photonView.RPC("TurnOnGravity", PhotonTargets.All, null);
        //gravityLever.GetPhotonView().RPC("RPCRaiseHandle", PhotonTargets.All, null);

        for (int i = 0; i < activeHazardsList.Length; i++)
            activeHazardsList[i] = false;
    }

    [PunRPC]
    public void TurnOnAmbientLight()
    {
        if (activeHazardsList[ambientLightIndex])
        {
            RenderSettings.ambientLight = defaultAmbientLight;
            directionalLight.intensity = defaultDirectionalLightIntensity;
            activeHazardsList[ambientLightIndex] = false;
        }
    }

    [PunRPC]
    public void VentFog()
    {
        if (activeHazardsList[fogValveIndex])
        {
            isIncreasingFog = false;
            StopCoroutine("IncreaseFog");
            StartCoroutine("DecreaseFog");
            //RenderSettings.fogDensity = defaultFogDensity;
            activeHazardsList[fogValveIndex] = false;
        }
    }

    [PunRPC]
    public void RESIZENormal()
    {
        if (activeHazardsList[resizeButtonIndex])
        {
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                player.transform.localScale = defaultScale;
            activeHazardsList[resizeButtonIndex] = false;
        }
    }

    [PunRPC]
    void TurnOffStatic()
    {
        if (activeHazardsList[staticLeverIndex])
        {
            // For each Main Camera that has the script "NoiseAndGrain" turn it on
            foreach (GameObject camera in GameObject.FindGameObjectsWithTag("MainCamera"))
            {
                if (camera.GetComponent<UnityStandardAssets.ImageEffects.NoiseAndGrain>() != null)
                {
                    camera.GetComponent<UnityStandardAssets.ImageEffects.NoiseAndGrain>().enabled = false;
                }
            }
            staticAudio.Stop();
            activeHazardsList[staticLeverIndex] = false;
        }
    }

    //[PunRPC]
    //void TurnOnGravity()
    //{
    //    if (activeHazardsList[gravityIndex])
    //    {
    //        Physics.gravity = defaultGravity;
    //        activeHazardsList[gravityIndex] = false;
    //    }
    //}
}
