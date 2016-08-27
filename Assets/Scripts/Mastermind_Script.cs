using UnityEngine;
using System.Collections;

public class Mastermind_Script : Photon.MonoBehaviour
{
    public class rObjectType
    {
        public GameObject rObject;
        public object[] data;
    }

    /** SINGLE VARIABLES **/
    private         int numPlayers = 0;
    private         int score = 0;
    private         int level = 1;
    private         bool isLoadingNextLevel = false;
    private         int scoreToWin = 10;
    private         int scoreToLose = -10;
    private         float levelTimeoutSeconds = 100;
    private         float commandTimeoutSeconds = 10;
    private         System.DateTime levelStartTime = System.DateTime.Now;
    private         bool isGameOver = false;
    private         int numOfDiffGameObjects = 8; // The number of different type of game objects total to be used for random rolling of said game objects
    public const    int buttonCommand = 0;
    public const    int dialCommand = 1;
    public const    int lLeverCommand = 2;
    public const    int lightswitchCommand = 3;
    public const    int shifterCommand = 4;
    public const    int sliderCommand = 5;
    public const    int valveCommand = 6;
    public const    int wLeverCommand = 7;
    private ArrayList buttonCommandArray;
    private ArrayList dialCommandArray;
    private ArrayList lLeverCommandArray;
    private ArrayList lightswitchCommandArray;
    private string pullcordCommandText;
    private ArrayList shifterCommandArray;
    private ArrayList sliderCommandArray;
    private ArrayList valveCommandArray;
    private ArrayList wLeverCommandArray;
    private rObjectType[] rObjList; // The list of all random game objects get placed in current round
    private bool  isTapped = false; // Variables for the custom WaitForSeconds function
    private int numFufilled = 0;
    private PhotonPlayer[] players;

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
    public  bool    p1_isDisplayingCommand = false;
    private float   p1_gWaitSystem;         // Variables for the custom WaitForSeconds function
    private int     p1_gridX = 4;           // The grid which the random game objects get placed
    private int     p1_gridY = 2;           // The grid which the random game objects get placed

    /** P2 VARIABLES **/
    public int      p2_rCommand = -1;
    private bool    p2_isDisplayStart = true;
    public bool     p2_isDisplayingCommand = false;
    private float   p2_gWaitSystem;         // Variables for the custom WaitForSeconds function
    private int     p2_gridX = 4;           // The grid which the random game objects get placed
    private int     p2_gridY = 2;           // The grid which the random game objects get placed

    /** P3 VARIABLES **/
    public int      p3_rCommand = -1;
    private bool    p3_isDisplayStart = true;
    public bool     p3_isDisplayingCommand = false;
    private float   p3_gWaitSystem;         // Variables for the custom WaitForSeconds function
    private int     p3_gridX = 4;           // The grid which the random game objects get placed
    private int     p3_gridY = 2;           // The grid which the random game objects get placed

    /** P4 VARIABLES **/
    public int      p4_rCommand = -1;
    private bool    p4_isDisplayStart = true;
    public bool     p4_isDisplayingCommand = false;
    private float   p4_gWaitSystem;         // Variables for the custom WaitForSeconds function
    private int     p4_gridX = 4;           // The grid which the random game objects get placed
    private int     p4_gridY = 2;           // The grid which the random game objects get placed

    // Use this for initialization
    void Start () {
        if (PhotonNetwork.isMasterClient)
        {
            isLoadingNextLevel = true;

            isGameOver = false;

            //Load Network data
            object[] data = photonView.instantiationData;
            if (data != null)
            {
                level = (int)data[0];
            }

            //Get number of players by the NetwokManager.numPlayers
            numPlayers = PhotonNetwork.playerList.Length;
            Debug.Log("numPlayers: " + numPlayers);

            //Set up level variables (score to win this round, number of seconds for each command, number of random objects to spawn per player)
            SetupLevel();

            isLoadingNextLevel = false;

            StartCoroutine(WaitForPlayersToSpawn());
        }
    }

    private void SetupLevel()
    {
        //Total score to win this round
        //At level ONE this score is 10, going up by 2 each level, by level 5 this is 20 commands, by level 10 this is 30 commands
        scoreToWin = 10 + (2 * (level - 1));
        //Total score to lose this round
        //At level ONE this score is -10, by level 5 this is -3, by level 10 this is -1 (converging to -1)
        scoreToLose = Mathf.RoundToInt(-Mathf.Pow(Mathf.Sqrt(9), (-0.3f * (level - 1)) + 2) - 1);
        //Number of seconds for the level before Game Over
        //At level ONE this is 100 seconds (10 seconds per command), by level 5 this is 54.2 seconds, by level 10 this is 38.367 seconds (converging to 30 seconds by level 25)
        levelTimeoutSeconds = Mathf.Pow(Mathf.Sqrt(70), (-0.1f * (level - 1)) + 2) + 30;
        //Number of seconds for each command before it times out
        //At level ONE this starts at 10 seconds, by level 5 this is 6.8 seconds, and by level 10 this is 4.737 seconds (converging to 1 second by level 35)
        commandTimeoutSeconds = Mathf.Pow(Mathf.Sqrt(9), (-0.08f * (level - 1)) + 2) + 1;
        //The base number of objects a player can start with (this number will be varied +/- 1
        //At level ONE this starts at 3 objects per player, by level 5 this is 7 objects, by level 8 this maxes out at 8 objects always for all players (converging to 8 objects)
        int baseNumObjPerPlayer = Mathf.RoundToInt(-Mathf.Pow(Mathf.Sqrt(8), (-0.3f * (level - 1)) + 1.54f) + 8);
        int[] xyNumObj_p1 = GetNumXYObjects(baseNumObjPerPlayer);
        p1_gridX = xyNumObj_p1[0];
        p1_gridY = xyNumObj_p1[1];
        int[] xyNumObj_p2 = GetNumXYObjects(baseNumObjPerPlayer);
        p2_gridX = xyNumObj_p2[0];
        p2_gridY = xyNumObj_p2[1];
        int[] xyNumObj_p3 = GetNumXYObjects(baseNumObjPerPlayer);
        p3_gridX = xyNumObj_p3[0];
        p3_gridY = xyNumObj_p3[1];
        int[] xyNumObj_p4 = GetNumXYObjects(baseNumObjPerPlayer);
        p4_gridX = xyNumObj_p4[0];
        p4_gridY = xyNumObj_p4[1];
    }

    private int[] GetNumXYObjects(int baseNumObjPerPlayer)
    {
        int[] returnXY = new int[2];
        int totalNumObj = baseNumObjPerPlayer + Random.Range(-1, 2);
        if (totalNumObj >= 7) //Can't display 7 objects, set to 8
            totalNumObj = 8;
        else if (totalNumObj == 5) //Can't display 5 objects set to 4
            totalNumObj = 4;

        if (totalNumObj >= 8) //if the total number of objects is 8 then do a full 4x2 grid of objects
        {
            returnXY[0] = 4;
            returnXY[1] = 2;
        }
        else if (totalNumObj == 6) //if the total number of objects is 6 then do a 3x2 grid of objects
        {
            returnXY[0] = 3;
            returnXY[1] = 2;
        }
        else if (totalNumObj <= 4 && totalNumObj > 0)
        {
            //if the total number of objects is even and less than equal to 4 flip a coin to see if we should display the objects on a single row or two rows
            if ((totalNumObj % 2) == 0 && Random.value < 0.5f) 
            {
                returnXY[0] = totalNumObj / 2;
                returnXY[1] = 2;
            }
            else
            {
                returnXY[0] = totalNumObj;
                returnXY[1] = 1;
            }
        }
        else
        {
            //Should never get here
            Debug.LogError("ERROR trying to generate too many or few objects. Base: " + baseNumObjPerPlayer + " total: " + totalNumObj);
        }

        return returnXY;
    }

    IEnumerator WaitForPlayersToSpawn()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("numPlayers: " + numPlayers + "playerObjects.Length: " + playerObjects.Length);
        while (playerObjects.Length < numPlayers)
        {
            playerObjects = GameObject.FindGameObjectsWithTag("Player");
            Debug.Log("numPlayers: " + numPlayers + " playerObjects.Length: " + playerObjects.Length);
            yield return new WaitForSeconds(0.1f);
        }

        foreach (GameObject player in playerObjects)
        {
            Debug.Log("name: " + player.name);
        }

        Initialize();

        //Count down from 3 to next level
        //Generate new objects
        //And display "Start!" command
        StartCoroutine(StartNextRoundIn(3));
    }

    void Initialize()
    {
        score = 0;
        
        //Find all Player Objects
        players = PhotonNetwork.playerList;
        
        foreach (PhotonPlayer player in players)
        {
            if (player != null)
                Debug.Log("name: " + player.name);
            else
                break;
        }

        //numPlayers = 4;

        // Set command arrays from Command_Array.cs
        Command_Array commandArray = GetComponent<Command_Array>();
        buttonCommandArray = commandArray.buttonCommandArray;
        dialCommandArray = commandArray.dialCommandArray;
        lLeverCommandArray = commandArray.lLeverCommandArray;
        lightswitchCommandArray = commandArray.lightswitchCommandArray;
        pullcordCommandText = Command_Array.pullcordText;
        shifterCommandArray = commandArray.shifterCommandArray;
        sliderCommandArray = commandArray.sliderCommandArray;
        valveCommandArray = commandArray.valveCommandArray;
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

    //Create new list of random objects
    void GenerateRandomObjects ()
    {
        //Create a list to hold all of the random game objects
        int rObjListSize = (p1_gridX * p1_gridY);
        if (numPlayers > 1)
            rObjListSize += (p2_gridX * p2_gridY);
        if (numPlayers > 2)
            rObjListSize += (p3_gridX * p3_gridY);
        if (numPlayers > 3)
            rObjListSize += (p4_gridX * p4_gridY);
        rObjList = new rObjectType[rObjListSize];

        //Generate rObjList objects for PLAYER 1
        rObjList = GenerateRandomObjects(rObjList, 0, p1_PlayerControlDeck, p1_gridX, p1_gridY, 1);
        Transform p1_RObjectTransform = p1_PlayerControlDeck.transform.Find("RObjects");
        p1_RObjectTransform.localRotation = Quaternion.Euler(new Vector3(45f, 0f, 0f));
        int rObjectCount = 0;
        for(int i=0; i< p1_RObjectTransform.childCount; i++)
        {
            GameObject rObjectEmpty = p1_RObjectTransform.GetChild(i).gameObject;
            GameObject rObject = PhotonNetwork.InstantiateSceneObject("Prefabs/" + rObjectEmpty.name, rObjectEmpty.transform.position, rObjectEmpty.transform.rotation, 0, rObjList[rObjectCount].data);
            rObjList[rObjectCount].rObject = rObject;
            rObjectCount++;
        }
        if (numPlayers > 1)
        {
            //Generate rObjList objects for PLAYER 2
            rObjList = GenerateRandomObjects(rObjList, (p2_gridX * p2_gridY), p2_PlayerControlDeck, p2_gridX, p2_gridY, 2);
            Transform p2_RObjectTransform = p2_PlayerControlDeck.transform.Find("RObjects");
            p2_RObjectTransform.localRotation = Quaternion.Euler(new Vector3(45f, 0f, 0f));
            for (int i = 0; i < p2_RObjectTransform.childCount; i++)
            {
                GameObject rObjectEmpty = p2_RObjectTransform.GetChild(i).gameObject;
                GameObject rObject = PhotonNetwork.InstantiateSceneObject("Prefabs/" + rObjectEmpty.name, rObjectEmpty.transform.position, rObjectEmpty.transform.rotation, 0, rObjList[rObjectCount].data);
                rObjList[rObjectCount].rObject = rObject;
                rObjectCount++;
            }
        }
        if (numPlayers > 2)
        {
            //Generate rObjList objects for PLAYER 3
            rObjList = GenerateRandomObjects(rObjList, (p3_gridX * p3_gridY) + (p2_gridX * p2_gridY), p3_PlayerControlDeck, p3_gridX, p3_gridY, 3);
            Transform p3_RObjectTransform = p3_PlayerControlDeck.transform.Find("RObjects");
            p3_RObjectTransform.localRotation = Quaternion.Euler(new Vector3(45f, 0f, 0f));
            for (int i = 0; i < p3_RObjectTransform.childCount; i++)
            {
                GameObject rObjectEmpty = p3_RObjectTransform.GetChild(i).gameObject;
                GameObject rObject = PhotonNetwork.InstantiateSceneObject("Prefabs/" + rObjectEmpty.name, rObjectEmpty.transform.position, rObjectEmpty.transform.rotation, 0, rObjList[rObjectCount].data);
                rObjList[rObjectCount].rObject = rObject;
                rObjectCount++;
            }
        }
        if (numPlayers > 3)
        {
            //Generate rObjList objects for PLAYER 4
            rObjList = GenerateRandomObjects(rObjList, (p4_gridX * p4_gridY) + (p2_gridX * p2_gridY) + (p3_gridX * p3_gridY), p4_PlayerControlDeck, p4_gridX, p4_gridY, 4);
            Transform p4_RObjectTransform = p4_PlayerControlDeck.transform.Find("RObjects");
            p4_RObjectTransform.localRotation = Quaternion.Euler(new Vector3(45f, 0f, 0f));
            for (int i = 0; i < p4_RObjectTransform.childCount; i++)
            {
                GameObject rObjectEmpty = p4_RObjectTransform.GetChild(i).gameObject;
                GameObject rObject = PhotonNetwork.InstantiateSceneObject("Prefabs/" + rObjectEmpty.name, rObjectEmpty.transform.position, rObjectEmpty.transform.rotation, 0, rObjList[rObjectCount].data);
                rObjList[rObjectCount].rObject = rObject;
                rObjectCount++;
            }
        }
    }

    rObjectType[] GenerateRandomObjects(rObjectType[] inRObjList, int intRObjListSize, GameObject playerControlDeck, int gridX, int gridY, int playerNum)
    {
        //Destroy all objects within the players RObjects list
        Transform rObjectTransform = playerControlDeck.transform.Find("RObjects");
        for (int i = rObjectTransform.childCount - 1; i >= 0; --i)
        {
            GameObject.Destroy(rObjectTransform.GetChild(i).gameObject);
        }
        rObjectTransform.DetachChildren();
        rObjectTransform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));

        rObjectType[] outRObjList = inRObjList;

        int commandIndex;
        string newCommandText;

        GameObject buttonInstance = (GameObject)Resources.Load("Prefabs/Button");

        float xQuaternion = buttonInstance.transform.rotation.eulerAngles.x + playerControlDeck.transform.rotation.eulerAngles.x;
        float yQuaternion = buttonInstance.transform.rotation.eulerAngles.y + playerControlDeck.transform.rotation.eulerAngles.y;
        float zQuaternion = buttonInstance.transform.rotation.eulerAngles.z + playerControlDeck.transform.rotation.eulerAngles.z;

        //for each grid position generate a random object and add it to the random object list
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

                rObjectType rObjectType = new rObjectType();
                GameObject randObjectEmpty;
                object[] data = new object[2];
                //roll for a random game object
                int objNum = Random.Range(0, numOfDiffGameObjects);

                Vector3 vector3 = new Vector3();
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
                        if (playerNum == 1)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.43f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.09f);
                        if (playerNum == 2)
                            vector3 = new Vector3(playerControlDeck.transform.position.x + 0.09f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.43f + (0.3f * xOffset));
                        if (playerNum == 3)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.43f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z + 0.09f);
                        if (playerNum == 4)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.09f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.43f + (0.3f * xOffset));
                        //randObject = (GameObject)Instantiate(Resources.Load("Prefabs/Button"),
                        //    vector3,
                        //    Quaternion.Euler(new Vector3(xQuaternion, yQuaternion, zQuaternion)));
                        randObjectEmpty = new GameObject();
                        randObjectEmpty.transform.position = vector3;
                        randObjectEmpty.transform.rotation = Quaternion.Euler(new Vector3(xQuaternion, yQuaternion, zQuaternion));
                        randObjectEmpty.name = "Button";
                        randObjectEmpty.transform.parent = playerControlDeck.transform.Find("RObjects").transform;
                        break;
                    case dialCommand:
                        //roll for a random Button command from the buttonCommandArray
                        commandIndex = Random.Range(0, dialCommandArray.Count);
                        newCommandText = (string)dialCommandArray[commandIndex];
                        //remove selected button command from buttonCommandArray so it won't be used again
                        dialCommandArray.RemoveAt(commandIndex);
                        //copy randomObject from the default wLever
                        if (playerNum == 1)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.43f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.05f);
                        if (playerNum == 2)
                            vector3 = new Vector3(playerControlDeck.transform.position.x + 0.05f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.43f + (0.3f * xOffset));
                        if (playerNum == 3)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.43f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z + 0.05f);
                        if (playerNum == 4)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.05f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.43f + (0.3f * xOffset));
                        //randObject = (GameObject)Instantiate(Resources.Load("Prefabs/Dial"),
                        //    vector3,
                        //    Quaternion.Euler(new Vector3(xQuaternion - 90, yQuaternion + 90, zQuaternion)));
                        randObjectEmpty = new GameObject();
                        randObjectEmpty.transform.position = vector3;
                        randObjectEmpty.transform.rotation = Quaternion.Euler(new Vector3(xQuaternion - 90, yQuaternion + 90, zQuaternion));
                        randObjectEmpty.name = "Dial";
                        randObjectEmpty.transform.parent = playerControlDeck.transform.Find("RObjects").transform;
                        break;
                    case lLeverCommand:
                        //roll for a random Button command from the lLeverCommandArray
                        commandIndex = Random.Range(0, lLeverCommandArray.Count);
                        newCommandText = (string)lLeverCommandArray[commandIndex];
                        //remove selected button command from lLeverCommandArray so it won't be used again
                        lLeverCommandArray.RemoveAt(commandIndex);
                        //copy randomObject from the default wLever
                        if (playerNum == 1)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.43f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.147f);
                        if (playerNum == 2)
                            vector3 = new Vector3(playerControlDeck.transform.position.x + 0.147f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.43f + (0.3f * xOffset));
                        if (playerNum == 3)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.43f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z + 0.147f);
                        if (playerNum == 4)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.147f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.43f + (0.3f * xOffset));
                        //randObject = (GameObject)Instantiate(Resources.Load("Prefabs/L_Lever"),
                        //    vector3,
                        //    Quaternion.Euler(new Vector3(xQuaternion, yQuaternion, zQuaternion)));
                        randObjectEmpty = new GameObject();
                        randObjectEmpty.transform.position = vector3;
                        randObjectEmpty.transform.rotation = Quaternion.Euler(new Vector3(xQuaternion, yQuaternion, zQuaternion));
                        randObjectEmpty.name = "L_Lever";
                        randObjectEmpty.transform.parent = playerControlDeck.transform.Find("RObjects").transform;
                        break;
                    case lightswitchCommand:
                        //roll for a random Button command from the lLeverCommandArray
                        commandIndex = Random.Range(0, lightswitchCommandArray.Count);
                        newCommandText = (string)lightswitchCommandArray[commandIndex];
                        //remove selected button command from lLeverCommandArray so it won't be used again
                        lightswitchCommandArray.RemoveAt(commandIndex);
                        //copy randomObject from the default wLever
                        if (playerNum == 1)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.43f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.05f);
                        if (playerNum == 2)
                            vector3 = new Vector3(playerControlDeck.transform.position.x + 0.05f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.43f + (0.3f * xOffset));
                        if (playerNum == 3)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.43f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z + 0.05f);
                        if (playerNum == 4)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.05f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.43f + (0.3f * xOffset));
                        //randObject = (GameObject)Instantiate(Resources.Load("Prefabs/Lightswitch"),
                        //    vector3,
                        //    Quaternion.Euler(new Vector3(xQuaternion, yQuaternion + 90, zQuaternion)));
                        randObjectEmpty = new GameObject();
                        randObjectEmpty.transform.position = vector3;
                        randObjectEmpty.transform.rotation = Quaternion.Euler(new Vector3(xQuaternion, yQuaternion + 90, zQuaternion));
                        randObjectEmpty.name = "Lightswitch";
                        randObjectEmpty.transform.parent = playerControlDeck.transform.Find("RObjects").transform;
                        break;
                    case shifterCommand:
                        //roll for a random Button command from the lLeverCommandArray
                        commandIndex = Random.Range(0, shifterCommandArray.Count);
                        newCommandText = (string)shifterCommandArray[commandIndex];
                        //remove selected button command from lLeverCommandArray so it won't be used again
                        shifterCommandArray.RemoveAt(commandIndex);
                        //copy randomObject from the default wLever
                        if (playerNum == 1)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.43f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.05f);
                        if (playerNum == 2)
                            vector3 = new Vector3(playerControlDeck.transform.position.x + 0.05f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.43f + (0.3f * xOffset));
                        if (playerNum == 3)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.43f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z + 0.05f);
                        if (playerNum == 4)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.05f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.43f + (0.3f * xOffset));
                        //randObject = (GameObject)Instantiate(Resources.Load("Prefabs/Shifter"),
                        //    vector3,
                        //    Quaternion.Euler(new Vector3(xQuaternion + 180, yQuaternion + 90, zQuaternion)));
                        randObjectEmpty = new GameObject();
                        randObjectEmpty.transform.position = vector3;
                        randObjectEmpty.transform.rotation = Quaternion.Euler(new Vector3(xQuaternion + 180, yQuaternion + 90, zQuaternion));
                        randObjectEmpty.name = "Shifter";
                        randObjectEmpty.transform.parent = playerControlDeck.transform.Find("RObjects").transform;
                        break;
                    case sliderCommand:
                        //roll for a random Button command from the lLeverCommandArray
                        commandIndex = Random.Range(0, sliderCommandArray.Count);
                        newCommandText = (string)sliderCommandArray[commandIndex];
                        //remove selected button command from lLeverCommandArray so it won't be used again
                        sliderCommandArray.RemoveAt(commandIndex);
                        //copy randomObject from the default wLever
                        if (playerNum == 1)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.43f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.05f);
                        if (playerNum == 2)
                            vector3 = new Vector3(playerControlDeck.transform.position.x + 0.05f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.43f + (0.3f * xOffset));
                        if (playerNum == 3)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.43f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z + 0.05f);
                        if (playerNum == 4)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.05f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.43f + (0.3f * xOffset));
                        //randObject = (GameObject)Instantiate(Resources.Load("Prefabs/Slider"),
                        //    vector3,
                        //    Quaternion.Euler(new Vector3(xQuaternion + 270, yQuaternion + 90, zQuaternion)));
                        randObjectEmpty = new GameObject();
                        randObjectEmpty.transform.position = vector3;
                        randObjectEmpty.transform.rotation = Quaternion.Euler(new Vector3(xQuaternion + 270, yQuaternion + 90, zQuaternion));
                        randObjectEmpty.name = "Slider";
                        randObjectEmpty.transform.parent = playerControlDeck.transform.Find("RObjects").transform;
                        break;
                    case valveCommand:
                        //roll for a random Button command from the lLeverCommandArray
                        commandIndex = Random.Range(0, valveCommandArray.Count);
                        newCommandText = (string)valveCommandArray[commandIndex];
                        //remove selected button command from lLeverCommandArray so it won't be used again
                        valveCommandArray.RemoveAt(commandIndex);
                        //copy randomObject from the default wLever
                        if (playerNum == 1)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.43f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.096f);
                        if (playerNum == 2)
                            vector3 = new Vector3(playerControlDeck.transform.position.x + 0.096f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.43f + (0.3f * xOffset));
                        if (playerNum == 3)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.43f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z + 0.096f);
                        if (playerNum == 4)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.096f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.43f + (0.3f * xOffset));
                        //randObject = (GameObject)Instantiate(Resources.Load("Prefabs/Valve"),
                        //    vector3,
                        //    Quaternion.Euler(new Vector3(xQuaternion, yQuaternion, zQuaternion + 90)));
                        randObjectEmpty = new GameObject();
                        randObjectEmpty.transform.position = vector3;
                        randObjectEmpty.transform.rotation = Quaternion.Euler(new Vector3(xQuaternion, yQuaternion, zQuaternion + 90));
                        randObjectEmpty.name = "Valve";
                        randObjectEmpty.transform.parent = playerControlDeck.transform.Find("RObjects").transform;
                        break;
                    case wLeverCommand:
                        //roll for a random Button command from the wLeverCommandArray
                        commandIndex = Random.Range(0, wLeverCommandArray.Count);
                        newCommandText = (string)wLeverCommandArray[commandIndex];
                        //remove selected button command from wLeverCommandArray so it won't be used again
                        wLeverCommandArray.RemoveAt(commandIndex);
                        //copy randomObject from the default wLever
                        if (playerNum == 1)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.43f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.152f);
                        if (playerNum == 2)
                            vector3 = new Vector3(playerControlDeck.transform.position.x + 0.152f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.43f + (0.3f * xOffset));
                        if (playerNum == 3)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.43f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z + 0.152f);
                        if (playerNum == 4)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.152f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.43f + (0.3f * xOffset));
                        //randObject = (GameObject)Instantiate(Resources.Load("Prefabs/W_Lever"),
                        //    vector3,
                        //    Quaternion.Euler(new Vector3(xQuaternion, yQuaternion, zQuaternion)));
                        randObjectEmpty = new GameObject();
                        randObjectEmpty.transform.position = vector3;
                        randObjectEmpty.transform.rotation = Quaternion.Euler(new Vector3(xQuaternion, yQuaternion, zQuaternion));
                        randObjectEmpty.name = "W_Lever";
                        randObjectEmpty.transform.parent = playerControlDeck.transform.Find("RObjects").transform;
                        break;
                    default:
                        //roll for a random Button command from the buttonCommandArray
                        commandIndex = Random.Range(0, buttonCommandArray.Count);
                        newCommandText = (string)buttonCommandArray[commandIndex];
                        //remove selected button command from buttonCommandArray so it won't be used again
                        buttonCommandArray.RemoveAt(commandIndex);
                        //copy randomObject from the default wLever
                        if (playerNum == 1)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.43f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.09f);
                        if (playerNum == 2)
                            vector3 = new Vector3(playerControlDeck.transform.position.x + 0.09f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.43f + (0.3f * xOffset));
                        if (playerNum == 3)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.43f + (0.3f * xOffset), 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z + 0.09f);
                        if (playerNum == 4)
                            vector3 = new Vector3(playerControlDeck.transform.position.x - 0.09f, 0.6f + (0.4f * yOffset), playerControlDeck.transform.position.z - 0.43f + (0.3f * xOffset));
                        //randObject = (GameObject)Instantiate(Resources.Load("Prefabs/Button"),
                        //    vector3,
                        //    Quaternion.Euler(new Vector3(xQuaternion, yQuaternion, zQuaternion)));
                        randObjectEmpty = new GameObject();
                        randObjectEmpty.transform.position = vector3;
                        randObjectEmpty.transform.rotation = Quaternion.Euler(new Vector3(xQuaternion, yQuaternion, zQuaternion));
                        randObjectEmpty.name = "Button";
                        randObjectEmpty.transform.parent = playerControlDeck.transform.Find("RObjects").transform;
                        break;
                }

                data[0] = newCommandText;
                data[1] = intRObjListSize + ((x * gridY) + y);

                //add randomObject to grid
                rObjectType.rObject = randObjectEmpty;
                rObjectType.data = data;
                outRObjList[intRObjListSize + ((x * gridY) + y)] = rObjectType;
            }
        }

        return outRObjList;
    }

    void DestroyRandomObjects()
    {
        //Destroy all rObjects on all clients
        if (rObjList != null)
        {
            foreach (rObjectType rObjectType in rObjList)
            {
                PhotonNetwork.Destroy(rObjectType.rObject);
                Destroy(rObjectType.rObject);
            }
        }
    }

    // Update is called once per frame
    void Update () {
        if (PhotonNetwork.isMasterClient)
        {
            //If the user scores grater than or equal to the score to win change text to Green and to say "YOU WIN~"
            //Else if the score is less than or equal to the score to lose change the text to Red and say "Game Over"
            if (score >= scoreToWin && !isLoadingNextLevel)
            {
                StartCoroutine(LoadNextLevel());
            }
            else if (score <= scoreToLose && !isLoadingNextLevel)
            {
                GameOver();
            }

            //If the game times out change the text to Red and say "Game Over"
            if (levelStartTime.AddSeconds(levelTimeoutSeconds) <= System.DateTime.Now)
            {
                GameOver();   
            }

            //If we are not loading the next level
            //AND if we are NOT typing "START!"
            //AND if we are NOT currently typing a command
            //AND if we are NOT currently waiting the 10 seconds for a command to pass
            //generate and display a new random command
            if (!isLoadingNextLevel && !isGameOver)
            {
                if (!p1_isDisplayStart && !p1_consoleTextScript.isTyping && !p1_isDisplayingCommand)
                    StartCoroutine(P1_DisplayRandomCommand());
                if (numPlayers > 1)
                {
                    if (!p2_isDisplayStart && !p2_consoleTextScript.isTyping && !p2_isDisplayingCommand)
                        StartCoroutine(P2_DisplayRandomCommand());
                }
                if (numPlayers > 2)
                {
                    if (!p3_isDisplayStart && !p3_consoleTextScript.isTyping && !p3_isDisplayingCommand)
                        StartCoroutine(P3_DisplayRandomCommand());
                }
                if (numPlayers > 3)
                {
                    if (!p4_isDisplayStart && !p4_consoleTextScript.isTyping && !p4_isDisplayingCommand)
                        StartCoroutine(P4_DisplayRandomCommand());
                }
            }
        }
    }

    IEnumerator LoadNextLevel()
    {
        isLoadingNextLevel = true;
        score = 0;
        level += 1;

        //Destroy all rObjects inside of rObjList
        DestroyRandomObjects();

        p1_scoreTextScript.photonView.RPC("Win", PhotonTargets.All, true);
        if (numPlayers > 1)
            p2_scoreTextScript.photonView.RPC("Win", PhotonTargets.All, true);
        if (numPlayers > 2)
            p3_scoreTextScript.photonView.RPC("Win", PhotonTargets.All, true);
        if (numPlayers > 3)
            p4_scoreTextScript.photonView.RPC("Win", PhotonTargets.All, true);

        //Set up level variables (score to win this round, number of seconds for each command, number of random objects to spawn per player)
        SetupLevel();

        yield return new WaitForSeconds(3);

        //Count down from 3 to next level
        //Generate new objects
        //And display "Start!" command
        StartCoroutine(StartNextRoundIn(3));

        UpdateScore();

        isLoadingNextLevel = false;
    }

    public void GameOver()
    {
        isGameOver = true;

        p1_scoreTextScript.photonView.RPC("GameOver", PhotonTargets.All, true);
        if (numPlayers > 1)
            p2_scoreTextScript.photonView.RPC("GameOver", PhotonTargets.All, true);
        if (numPlayers > 2)
            p3_scoreTextScript.photonView.RPC("GameOver", PhotonTargets.All, true);
        if (numPlayers > 3)
            p4_scoreTextScript.photonView.RPC("GameOver", PhotonTargets.All, true);
    }

    public void UpdateScore()
    {
        p1_scoreTextScript.photonView.RPC("ScoreUp", PhotonTargets.All, score);
        if (numPlayers > 1)
            p2_scoreTextScript.photonView.RPC("ScoreUp", PhotonTargets.All, score);
        if (numPlayers > 2)
            p3_scoreTextScript.photonView.RPC("ScoreUp", PhotonTargets.All, score);
        if (numPlayers > 3)
            p4_scoreTextScript.photonView.RPC("ScoreUp", PhotonTargets.All, score);
    }

    public void ScoreUp()
    {
        score++;
        UpdateScore();
    }

    public void ScoreDown()
    {
        score--;
        UpdateScore();
    }

    //Display a countdown till next round, call to generate the random objects, and display "START!"
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

        //Generate the random objects
        GenerateRandomObjects();

        //Start the next round!
        UpdateAllConsoles(" START!");
        yield return new WaitForSeconds(1);

        p1_isDisplayStart = false;
        p2_isDisplayStart = false;
        p3_isDisplayStart = false;
        p4_isDisplayStart = false;

        levelStartTime = System.DateTime.Now;
    }

    public void UpdateAllConsoles(string msg)
    {
        p1_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, msg);
        if (numPlayers > 1)
        {
            p2_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, msg);
        }
        if (numPlayers > 2)
        {
            p3_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, msg);
        }
        if (numPlayers > 3)
        {
            p4_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, msg);
        }
    }

    // Custom WaitForSeconds
    // This will either wait for the given seconds, or until the isTapped boolean is set to TRUE
    IEnumerator WaitForSecondsOrTap(int player, float seconds)
    {
        if (player == 1)
        {
            p1_gWaitSystem = seconds;
            while (p1_gWaitSystem > 0.0)
            {
                p1_gWaitSystem -= Time.deltaTime;
                yield return 0;
            }
        }
        else if (player == 2)
        {
            p2_gWaitSystem = seconds;
            while (p2_gWaitSystem > 0.0)
            {
                p2_gWaitSystem -= Time.deltaTime;
                yield return 0;
            }
        }
        else if (player == 3)
        {
            p3_gWaitSystem = seconds;
            while (p3_gWaitSystem > 0.0)
            {
                p3_gWaitSystem -= Time.deltaTime;
                yield return 0;
            }
        }
        else if (player == 4)
        {
            p4_gWaitSystem = seconds;
            while (p4_gWaitSystem > 0.0)
            {
                p4_gWaitSystem -= Time.deltaTime;
                yield return 0;
            }
        }

        //lower score if time reached (button was not tapped)
        if (numFufilled == 0)
        {
            ScoreDown();
        }
        else
        {
            numFufilled -= 1;
        }

        //reset isTapped
        isTapped = false;
    }

    // End the waitForSeconds by setting the timer to zero AND signal that a button was tapped (isTapped = true)
    public void TappedWaitForSecondsOrTap(int inputCommand)
    {
        isTapped = true;
        numFufilled = 0;

        //Debug.Log("p1_command = " + p1_rCommand + " inputCommand = " + inputCommand);
                
        //Check to see if the current command is the correct button pressed. Update score accordingly
        if (p1_rCommand == inputCommand)
        {
            p1_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, "");
            ScoreUp();
            //Set timer for that player to 0 to get next command
            p1_gWaitSystem = 0.0f;
            numFufilled += 1;
        }
        if (p2_rCommand == inputCommand)
        {
            p2_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, "");
            ScoreUp();
            //Set timer for that player to 0 to get next command
            p2_gWaitSystem = 0.0f;
            numFufilled += 1;
        }
        if (p3_rCommand == inputCommand)
        {
            p3_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, "");
            ScoreUp();
            //Set timer for that player to 0 to get next command
            p3_gWaitSystem = 0.0f;
            numFufilled += 1;
        }
        if (p4_rCommand == inputCommand)
        {
            p4_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, "");
            ScoreUp();
            //Set timer for that player to 0 to get next command
            p4_gWaitSystem = 0.0f;
            numFufilled += 1;
        }

        // If no command matched lower score
        if (numFufilled == 0)
        {
            ScoreDown();
        }
    }

    IEnumerator P1_DisplayRandomCommand()
    {
        p1_isDisplayingCommand = true;

        //Clear text
        p1_consoleTextScript.GetComponent<TextMesh>().text = "";
        yield return 0;
        yield return new WaitForSeconds(1);

        //Roll random number to decide new command from the random object list
        p1_rCommand = Random.Range(0, rObjList.Length);

        //get random game object from random object list
        GameObject rObj = rObjList[p1_rCommand].rObject;

        string[] rCommandList = GetRandomCommand(rObj, p1_rCommand);
        p1_rCommand = int.Parse(rCommandList[0]);
        string message = rCommandList[1];

        //Type out new command to console
        p1_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, message);

        //Wait for the commandTimeoutSeconds or if a button gets tapped
        yield return WaitForSecondsOrTap(1, commandTimeoutSeconds);
        p1_isDisplayingCommand = false;
    }

    IEnumerator P2_DisplayRandomCommand()
    {
        p2_isDisplayingCommand = true;

        //Clear text
        p2_consoleTextScript.GetComponent<TextMesh>().text = "";
        yield return 0;
        yield return new WaitForSeconds(1);

        //Roll random number to decide new command from the random object list
        p2_rCommand = Random.Range(0, rObjList.Length);

        //get random game object from random object list
        GameObject rObj = rObjList[p2_rCommand].rObject;

        string[] rCommandList = GetRandomCommand(rObj, p2_rCommand);
        p2_rCommand = int.Parse(rCommandList[0]);
        string message = rCommandList[1];

        //Type out new command to console
        p2_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, message);

        //Wait for the commandTimeoutSeconds or if a button gets tapped
        yield return WaitForSecondsOrTap(2, commandTimeoutSeconds);
        p2_isDisplayingCommand = false;
    }

    IEnumerator P3_DisplayRandomCommand()
    {
        p3_isDisplayingCommand = true;

        //Clear text
        p3_consoleTextScript.GetComponent<TextMesh>().text = "";
        yield return 0;
        yield return new WaitForSeconds(1);

        //Roll random number to decide new command from the random object list
        p3_rCommand = Random.Range(0, rObjList.Length);

        //get random game object from random object list
        GameObject rObj = rObjList[p3_rCommand].rObject;

        string[] rCommandList = GetRandomCommand(rObj, p3_rCommand);
        p3_rCommand = int.Parse(rCommandList[0]);
        string message = rCommandList[1];

        //Type out new command to console
        p3_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, message);

        //Wait for the commandTimeoutSeconds or if a button gets tapped
        yield return WaitForSecondsOrTap(3, commandTimeoutSeconds);
        p3_isDisplayingCommand = false;
    }

    IEnumerator P4_DisplayRandomCommand()
    {
        p4_isDisplayingCommand = true;

        //Clear text
        p4_consoleTextScript.GetComponent<TextMesh>().text = "";
        yield return 0;
        yield return new WaitForSeconds(1);

        //Roll random number to decide new command from the random object list
        p4_rCommand = Random.Range(0, rObjList.Length);

        //get random game object from random object list
        GameObject rObj = rObjList[p4_rCommand].rObject;

        string[] rCommandList = GetRandomCommand(rObj, p4_rCommand);
        p4_rCommand = int.Parse(rCommandList[0]);
        string message = rCommandList[1];

        //Type out new command to console
        p4_consoleTextScript.photonView.RPC("RpcTypeText", PhotonTargets.All, message);

        //Wait for the commandTimeoutSeconds or if a button gets tapped
        yield return WaitForSecondsOrTap(4, commandTimeoutSeconds);
        p4_isDisplayingCommand = false;
    }

    string[] GetRandomCommand (GameObject rObj, int rCommand)
    {
        int commandType = buttonCommand;
        if (rObj.name.Contains("Button"))
            commandType = buttonCommand;
        else if (rObj.name.Contains("Dial"))
            commandType = dialCommand;
        else if (rObj.name.Contains("L_Lever"))
            commandType = lLeverCommand;
        else if (rObj.name.Contains("Lightswitch"))
            commandType = lightswitchCommand;
        else if (rObj.name.Contains("Shifter"))
            commandType = shifterCommand;
        else if (rObj.name.Contains("Slider"))
            commandType = sliderCommand;
        else if (rObj.name.Contains("Valve"))
            commandType = valveCommand;
        else if (rObj.name.Contains("W_Lever"))
            commandType = wLeverCommand;

        string message = "";
        int newRCommand;
        //Get new command
        switch (commandType)
        {
            case buttonCommand:
                //Button
                string buttonText = rObj.GetComponent<Button_Script>().newName;
                message = "Engage " + buttonText;
                break;
            case dialCommand:
                //Dial
                Dial_Script dialScript = rObj.GetComponent<Dial_Script>();
                string dialText = dialScript.newName;
                newRCommand = Random.Range(0, 5);
                while (newRCommand == dialScript.dialPosition)
                {
                    newRCommand = Random.Range(0, 5);
                }
                message = "Change " + dialText + " to Ch. " + newRCommand;
                rCommand = (rCommand * 100) + newRCommand;
                break;
            case lLeverCommand:
                //L_Lever
                L_Lever_Script lLeverScript = rObj.GetComponent<L_Lever_Script>();
                string lLeverText = lLeverScript.newName;
                message = "Turn ";
                if (lLeverScript.isLLeverUp)
                {
                    message += "OFF ";
                    rCommand = (rCommand * 100) + 1;
                }
                else
                {
                    message += "ON ";
                    rCommand = (rCommand * 100) + 2;
                }
                message += lLeverText;
                break;
            case lightswitchCommand:
                //L_Lever
                Lightswitch_Script lightswitchScript = rObj.GetComponent<Lightswitch_Script>();
                string lightswitchText = lightswitchScript.newName;
                message = "Turn ";
                if (lightswitchScript.isLightswitchOn)
                {
                    message += "OFF ";
                    rCommand = (rCommand * 100) + 1;
                }
                else
                {
                    message += "ON ";
                    rCommand = (rCommand * 100) + 2;
                }
                message += lightswitchText;
                break;
            case shifterCommand:
                //Dial
                Shifter_Script shifterScript = rObj.GetComponent<Shifter_Script>();
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
                rCommand = (rCommand * 100) + newRCommand;
                break;
            case sliderCommand:
                //Dial
                Slider_Script sliderScript = rObj.GetComponent<Slider_Script>();
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
                rCommand = (rCommand * 100) + newRCommand;
                break;
            case valveCommand:
                //Valve
                Valve_Script valveScript = rObj.GetComponent<Valve_Script>();
                string valveText = valveScript.newName;
                newRCommand = Random.Range(0, 2);
                message = "";
                if (newRCommand == 0)
                {
                    message += "Tighten ";
                    rCommand = (rCommand * 100) + 1;
                }
                else
                {
                    message += "Loosen ";
                    rCommand = (rCommand * 100) + 2;
                }
                message += valveText;
                break;
            case wLeverCommand:
                //W_Lever
                string wLeverText = rObj.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>().text;
                W_Lever_Script wLeverHandleScript = rObj.GetComponent<W_Lever_Script>();
                message = "";
                if (wLeverHandleScript.isWLeverUp)
                {
                    message += "Lower ";
                    rCommand = (rCommand * 100) + 1;
                }
                else
                {
                    message += "Raise ";
                    rCommand = (rCommand * 100) + 2;
                }
                message += wLeverText;
                break;
            default:
                Debug.LogError("Error - name: " + rObj.name);
                break;
        }

        return new string[2]{ rCommand.ToString() , message };
    }
}
