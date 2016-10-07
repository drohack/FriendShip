// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhotonLobby.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Oculus.Platform;
using Oculus.Platform.Models;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using System.Collections.Generic;

public class PhotonLobby_VR : MonoBehaviour
{
    public GUISkin Skin;
    public Vector2 WidthAndHeight = new Vector2(600, 400);
    private string roomName = "myRoom";
    public GameObject ListItemPrefab;
    public GameObject LobbyTitle;
    public GameObject UsersText;
    public GameObject GamesText;

    public GameObject RoomsGrid;

    private Vector2 scrollPos = Vector2.zero;

    private bool connectFailed = false;

    public static readonly string SceneNameMenu = "TestLobby";

    public static readonly string SceneNameGame = "Game";

    public static readonly string SceneNameLobbyRoom = "LobbyRoom";

    private string errorDialog;
    private double timeToClearDialog;

    private bool showLobby = false;
    private bool showRoom = true;

    private bool[] playerPosOccupied = new bool[4] { true, false, false, false };
    private bool isOtherPlayerJoining = false;

    private int level = 1;

    void Start()
    {
        InvokeRepeating("RoomListUpdate", 1.0f, 1.0f);
    }

    void RoomListUpdate()
    {
        if (PhotonNetwork.GetRoomList().Length == 0)
        {
            GamesText.GetComponent<Text>().text = ("Currently no games are available.");
        }
        else
        {
            GamesText.GetComponent<Text>().text = (PhotonNetwork.GetRoomList().Length + " rooms available:");

            // Room listing: simply call GetRoomList: no need to fetch/poll whatever!
            int numberOfGames = 0;
            foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList())
            {
                numberOfGames++;
                bool roomExists = false;

                foreach (Transform child in RoomsGrid.transform)
                {
                    Debug.Log(child.GetChild(0).GetComponent<TextMesh>().text + " is compared to " + roomInfo.name);
                    if (roomInfo.name == child.GetChild(0).GetComponent<TextMesh>().text)
                    {
                        roomExists = true;
                        child.GetChild(1).GetComponent<TextMesh>().text = (roomInfo.playerCount + "/" + roomInfo.maxPlayers);
                    }
                    else
                    {
                        Debug.Log("Name does not exist on button #: " + numberOfGames);
                    }
                }

                // If the room wasn't found look through each button available and find first open Button to use for room
                if (!roomExists)
                {
                    int numberOfButton = 0;
                    foreach (Transform child in RoomsGrid.transform)
                    {
                        numberOfButton++;
                        int buttonAvailable = 0;
                        if (child.GetChild(0).GetComponent<TextMesh>().text == "Empty")
                        {
                            buttonAvailable = numberOfButton;
                            child.gameObject.SetActive(true);
                            child.GetChild(0).GetComponent<TextMesh>().text = (roomInfo.name);
                            child.GetChild(1).GetComponent<TextMesh>().text = (roomInfo.playerCount + "/" + roomInfo.maxPlayers);
                            break;
                        }
                    }
                }
            }
        }
    }

    void Update()
    {
        if (showLobby)
        {
            if (!PhotonNetwork.connected)
            {
                if (PhotonNetwork.connecting)
                {
                    LobbyTitle.GetComponent<Text>().text = ("Connecting to: " + PhotonNetwork.ServerAddress);
                }
                else
                {
                    LobbyTitle.GetComponent<Text>().text = ("Not connected. Check console output. Detailed connection state: " + PhotonNetwork.connectionStateDetailed + " Server: " + PhotonNetwork.ServerAddress);
                }

                if (this.connectFailed)
                {
                    LobbyTitle.GetComponent<Text>().text = ("Connection failed. Check setup and use Setup Wizard to fix configuration.");
                }

                return;
            }

            LobbyTitle.GetComponent<Text>().text = ("Join or Create Room");

            // Save name
            PlayerPrefs.SetString("playerName", PhotonNetwork.playerName);

            if (!string.IsNullOrEmpty(ErrorDialog))
            {
                GUILayout.Label(ErrorDialog);

                if (this.timeToClearDialog < Time.time)
                {
                    this.timeToClearDialog = 0;
                    ErrorDialog = "";
                }
            }

            UsersText.GetComponent<Text>().text = (PhotonNetwork.countOfPlayers + " users are online in " + PhotonNetwork.countOfRooms + " rooms.");

            
        }
        else if (showRoom)
        {
            Rect content = new Rect((Screen.width - this.WidthAndHeight.x) / 2, (Screen.height - this.WidthAndHeight.y) / 2, this.WidthAndHeight.x, this.WidthAndHeight.y);
            GUI.Box(content, "Room: " + this.roomName);
            GUILayout.BeginArea(content);

            GUILayout.Space(40);

            // Display all players
            foreach (PhotonPlayer player in PhotonNetwork.playerList)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(player.name);
                if (GUILayout.Button("Start", GUILayout.Width(150)))
                {
                    //Set player as started.
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(15);

            // Buttons
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Return to Lobby"))
            {
                //Remove yourself from the list of ocu

                PhotonNetwork.LeaveRoom();  // we will load the menu level when we successfully left the room
                showRoom = false;
                showLobby = true;
            }

            if (PhotonNetwork.isMasterClient && GUILayout.Button("Start Game"))
            {
                Debug.Log("pPosOccupied: " + playerPosOccupied[0] + ", " + playerPosOccupied[1] + ", " + playerPosOccupied[2] + ", " + playerPosOccupied[3]);
                PhotonNetwork.LoadLevel(SceneNameGame); //Start Game
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
    }
    public string ErrorDialog
    {
        get { return this.errorDialog; }
        private set
        {
            this.errorDialog = value;
            if (!string.IsNullOrEmpty(value))
            {
                this.timeToClearDialog = Time.time + 4.0f;
            }
        }
    }

    private void GetLoggedInOculusUserCallback(Message msg)
    {
        if (!msg.IsError)
        {
            User u = msg.GetUser();
            PhotonNetwork.playerName = u.OculusID;
        }
    }

    public void Awake()
    {
        //Initialize the Oculus Platform.
        Oculus.Platform.Core.Initialize("1219926394692968");

        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;

        // the following line checks if this client was just created (and not yet online). if so, we connect
        if (PhotonNetwork.connectionStateDetailed == ClientState.PeerCreated)
        {
            // Connect to the photon master-server. We use the settings saved in PhotonServerSettings (a .asset file in this project)
            PhotonNetwork.ConnectUsingSettings("0.9");
        }

        // generate a name for this player, if none is assigned yet
        if (String.IsNullOrEmpty(PhotonNetwork.playerName))
        {
            //Try to find Oculus Username of logged in user.  If one exists, use it.  Otherwise, use random name.
            try
            {
                Users.GetLoggedInUser().OnComplete(GetLoggedInOculusUserCallback);
            }
            catch
            {
                PhotonNetwork.playerName = "Guest" + Random.Range(1, 9999);
            }
        }

        // if you wanted more debug out, turn this on:
        // PhotonNetwork.logLevel = NetworkLogLevel.Full;

        showLobby = true;
    }

    public void CreateGame()
    {
        Debug.Log("trying to create a game");
       
        //Set yourself as the first position and update your pPos
        playerPosOccupied = new bool[4] { true, false, false, false };
        Hashtable ht2 = new Hashtable() { { PhotonConstants.pPos, 0 } };
        PhotonNetwork.player.SetCustomProperties(ht2);
        isOtherPlayerJoining = false;

        //Create room 
        
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomProperties = new Hashtable() { { PhotonConstants.pPosOccupied, playerPosOccupied }, { PhotonConstants.level, level } };
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(this.roomName, roomOptions, TypedLobby.Default);
        

    }

    public void JoinRoom(string joinGameName)
    {
        Debug.Log("Joining Room: " + joinGameName);
        PhotonNetwork.JoinRoom(joinGameName);
    }

    // We have two options here: we either joined(by title, list or random) or created a room.
    public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
    }

    public void OnPhotonCreateRoomFailed()
    {
        ErrorDialog = "Error: Can't create room (room name maybe already used).";
        Debug.Log("OnPhotonCreateRoomFailed got called. This can happen if the room exists (even if not visible). Try another room name.");
    }

    public void OnPhotonJoinRoomFailed(object[] cause)
    {
        ErrorDialog = "Error: Can't join room (full or unknown room name). " + cause[1];
        Debug.Log("OnPhotonJoinRoomFailed got called. This can happen if the room is not existing or full or closed.");
    }

    public void OnPhotonRandomJoinFailed()
    {
        ErrorDialog = "Error: Can't join random room (none found).";
        Debug.Log("OnPhotonRandomJoinFailed got called. Happens if no room is available (or all full or invisible or closed). JoinrRandom filter-options can limit available rooms.");
    }

    public void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");

        showLobby = false;
        showRoom = true;
        PhotonNetwork.LoadLevel(SceneNameLobbyRoom);
    }

    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("Disconnected from Photon.");
    }

    public void OnFailedToConnectToPhoton(object parameters)
    {
        this.connectFailed = true;
        Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters + " ServerAddress: " + PhotonNetwork.ServerAddress);
    }

    //Only called when another player enters the room
    void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log(newPlayer.name + " has joined the room.");

        //Set new players pPos and update the pPosOccupied (if masterclient)
        if (PhotonNetwork.isMasterClient)
        {
            //wait till you've finished adding the other player to join the room
            StartCoroutine(WaitForOtherPlayersToJoin());

            isOtherPlayerJoining = true;
            int newPlayerPos = 0;
            //Find the first open spot in pPosOccupied and set the new player's position
            for (int i = 0; i < playerPosOccupied.Length; i++)
            {
                if (playerPosOccupied[i] == false)
                {
                    playerPosOccupied[i] = true;
                    newPlayerPos = i;
                    break;
                }
            }

            //Update the room's pPosOccupied
            Hashtable ht1 = new Hashtable() { { PhotonConstants.pPosOccupied, playerPosOccupied } };
            PhotonNetwork.room.SetCustomProperties(ht1);
            //Update new player's pPos
            Hashtable ht2 = new Hashtable() { { PhotonConstants.pPos, newPlayerPos } };
            newPlayer.SetCustomProperties(ht2);
            isOtherPlayerJoining = false;
        }
    }

    private System.Collections.IEnumerator WaitForOtherPlayersToJoin()
    {
        while (isOtherPlayerJoining)
        {
            yield return null;
        }
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        Debug.Log(otherPlayer.name + " disconnected from the room.");

        //Open up player position
        if (PhotonNetwork.isMasterClient && otherPlayer.customProperties.ContainsKey(PhotonConstants.pPos) && PhotonNetwork.room.customProperties.ContainsKey(PhotonConstants.pPosOccupied))
        {
            int otherPlayerPos = (int)otherPlayer.customProperties[PhotonConstants.pPos];
            playerPosOccupied = (bool[])PhotonNetwork.room.customProperties[PhotonConstants.pPos];
            playerPosOccupied[otherPlayerPos] = false;
            Hashtable ht = new Hashtable() { { PhotonConstants.pPosOccupied, playerPosOccupied } };
            PhotonNetwork.room.SetCustomProperties(ht);
        }
    }
}
