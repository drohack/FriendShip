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
using Steamworks;
using System.Collections.Generic;

public class PhotonMainMenu : MonoBehaviour
{
    public Text LobbyTitle;
    public Text UsersText;
    public Text GamesText;

    public Transform RoomsGrid;
    private GameObject[] RoomButtonList;

    private Vector2 scrollPos = Vector2.zero;

    private bool connectFailed = false;

    public static readonly string SceneNameMenu = "MainMenu";

    public static readonly string SceneNameGame = "Game";

    public static readonly string SceneNameLobbyRoom = "LobbyRoom";

    private string errorDialog;
    private double timeToClearDialog;

    private bool showLobby = false;
    private bool showRoom = true;

    private bool[] playerPosOccupied = new bool[4] { true, false, false, false };
    private bool isOtherPlayerJoining = false;

    [SerializeField]
    Transform spawnTransform;

    [SerializeField]
    GameObject SteamVR_LoadLevel;

    private GameObject ovrRig;

    private bool isTitleUpdated = false;

    [SerializeField]
    private GameObject roomButton;
    private Vector2 roomButtonDefaultBounds;

    [SerializeField]
    private TextMesh pagesText;

    private int currentPage = 1;
    private int maxPages = 1;

    public void Awake()
    {
#if OCULUS
        //Initialize the Oculus Platform.
        Oculus.Platform.Core.Initialize("1219926394692968");
        Instantiate(Resources.Load("Oculus/[OvrManager]"));
#else
        //Initialize the Steam Manager
        Instantiate(Resources.Load("Vive/[SteamManager]"));
#endif

        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;

        // the following line checks if this client was just created (and not yet online). if so, we connect
        if (PhotonNetwork.connectionStateDetailed == ClientState.PeerCreated)
        {
            // Connect to the photon master-server. We use the settings saved in PhotonServerSettings (a .asset file in this project)
            PhotonNetwork.ConnectUsingSettings("0.9");
        }

        // if you wanted more debug out, turn this on:
        // PhotonNetwork.logLevel = NetworkLogLevel.Full;

        //Spawn the correct VR rig
#if OCULUS
        //Instantiate OvrRig
        ovrRig = (GameObject)Instantiate(Resources.Load("Oculus/OvrRig"), spawnTransform.position, spawnTransform.rotation);
#else
        //Instantiate [SteamVR] and ViveRig
        if (!GameObject.FindGameObjectWithTag("[SteamVR]"))
            Instantiate(Resources.Load("Vive/[SteamVR]"), Vector3.zero, Quaternion.identity);
        if(GameObject.FindGameObjectsWithTag("[SteamVR_LoadLevel]").Length == 0)
        {
            SteamVR_LoadLevel.SetActive(true);
            DontDestroyOnLoad(SteamVR_LoadLevel);
        }
        Instantiate(Resources.Load("Vive/ViveRig"), spawnTransform.position, spawnTransform.rotation);
#endif

        showLobby = true;
    }

    void Start()
    {
#if OCULUS
    Entitlements.IsUserEntitledToApplication().OnComplete(
        (Message msg) =>
        {
            if (msg.IsError)
            {
                // User is NOT entitled.
                Debug.LogError("User is not entitled");
            }
            else
            {
                // User IS entitled
            }
        }
    );
#endif
        // Build RoomButtonList
        RoomButtonList = new GameObject[RoomsGrid.childCount];
        int count = 0;
        foreach (Transform child in RoomsGrid)
        {
            RoomButtonList[count] = child.gameObject;
            count++;
        }

        InvokeRepeating("RoomListUpdate", 1.0f, 0.1f);
        PhotonNetwork.player.customProperties.Clear();

        // generate a name for this player, if none is assigned yet
        if (String.IsNullOrEmpty(PhotonNetwork.playerName))
        {
            //Try to find Oculus Username of logged in user.  If one exists, use it.  Otherwise, use random name.
            try
            {
#if OCULUS
                Users.GetLoggedInUser().OnComplete(GetLoggedInOculusUserCallback);
#else
                if(SteamManager.Initialized) 
                {
                    PhotonNetwork.playerName = SteamFriends.GetPersonaName();
                }
#endif
            }
            catch
            {
                PhotonNetwork.playerName = "Guest" + Random.Range(1, 9999);
            }
        }

        roomButton.SetActive(true);
        roomButtonDefaultBounds = TextMeshArea(roomButton.transform.Find("GameName").GetComponent<TextMesh>());
        roomButton.SetActive(false);
    }

    void RoomListUpdate()
    {
        // Room listing: simply call GetRoomList: no need to fetch/poll whatever!
        RoomInfo[] RoomList = PhotonNetwork.GetRoomList();
        if (RoomList.Length == 0)
        {
            GamesText.text = ("No games\nin session");
            currentPage = 1;
            maxPages = 1;
        }
        else
        {
            GamesText.text = (RoomList.Length + " games\nin session");

            //set maxPages
            maxPages = Mathf.CeilToInt(RoomList.Length / 10f);

            //if you're outside the bounds, go to the max
            if (currentPage > maxPages)
            {
                currentPage = maxPages;
                foreach (GameObject roomButton in RoomButtonList)
                {
                    roomButton.SetActive(false);
                }
            }
        }

        pagesText.text = currentPage + " / " + maxPages;

        //Create a temporary list of Rooms to display only holding the rooms for this page
        RoomInfo[] DisplayRoomList = new RoomInfo[10];
        int count = 0;
        for (int i = ((currentPage - 1)*10); i<(currentPage * 10); i++)
        {
            //Break if it's not a full list
            if (i >= RoomList.Length)
                break;
            DisplayRoomList[count] = RoomList[i];
            count++;
        }

        // For each RoomButton disable it if that Room is either invisible (in a game) or no longer exist
        foreach (GameObject roomButton in RoomButtonList)
        {
            // Only care about active RoomButtons
            if (roomButton.GetActive())
            {
                bool roomExists = false;
                bool isVisible = false;
                string roomButtonName = roomButton.transform.Find("GameName").GetComponent<TextMesh>().text;
                // For each Room in DisplayRoomList check to see if the RoomButton still exists and if it is visible
                foreach (RoomInfo roomInfo in DisplayRoomList)
                {
                    if (roomInfo != null)
                    {
                        if (roomInfo.name.Equals(roomButtonName))
                        {
                            roomExists = true;
                            if (roomInfo.visible)
                                isVisible = true;
                            break;
                        }
                    }
                }
                // If the room does not exist OR is not visible disable the RoomButton
                if (!roomExists || !isVisible)
                {
                    roomButton.SetActive(false);
                }
            }
        }

        // For each visible Room on this page updated/add them to a RoomButton
        foreach (RoomInfo roomInfo in DisplayRoomList)
        {
            if (roomInfo != null)
            {
                // Only display games that are not already running (even if they are full)
                if (roomInfo.visible)
                {
                    bool roomExists = false;

                    // Check all of the RoomButtons to see if the game is already associated with a RoomButton
                    // If it is update the PlayerCount
                    foreach (GameObject roomButton in RoomButtonList)
                    {
                        string roomButtonName = roomButton.transform.Find("GameName").GetComponent<TextMesh>().text;
                        //Debug.Log(childGameName + " is compared to " + roomInfo.name);
                        if (roomInfo.name.Equals(roomButtonName) && roomButton.GetActive() == true)
                        {
                            roomExists = true;
                            roomButton.transform.Find("PlayerCount").GetComponent<TextMesh>().text = (roomInfo.playerCount + "/" + roomInfo.maxPlayers);
                            break;
                        }
                    }

                    // If the room wasn't found look through each button available and find first open Button to use for room
                    if (!roomExists)
                    {
                        foreach (GameObject roomButton in RoomButtonList)
                        {
                            if (!roomButton.GetActive())
                            {
                                roomButton.gameObject.SetActive(true);
                                TextMesh roomName = roomButton.transform.Find("GameName").GetComponent<TextMesh>();
                                roomName.text = (roomInfo.name);
                                //Resize the room name to fit (you wont get exact as font size is an integer)
                                if (TextMeshArea(roomName).y > roomButtonDefaultBounds.y)
                                {
                                    while (TextMeshArea(roomName).y > roomButtonDefaultBounds.y)
                                    {
                                        roomName.fontSize--;
                                    }
                                }
                                else if (TextMeshArea(roomName).y < roomButtonDefaultBounds.y)
                                {
                                    while (TextMeshArea(roomName).y < roomButtonDefaultBounds.y)
                                    {
                                        if (TextMeshArea(roomName).x > roomButtonDefaultBounds.x)
                                            break;
                                        roomName.fontSize++;
                                    }
                                }
                                else if (TextMeshArea(roomName).x > roomButtonDefaultBounds.x)
                                {
                                    while (TextMeshArea(roomName).x > roomButtonDefaultBounds.x)
                                    {
                                        roomName.fontSize--;
                                    }
                                }
                                roomButton.transform.Find("PlayerCount").GetComponent<TextMesh>().text = (roomInfo.playerCount + "/" + roomInfo.maxPlayers);
                                break;
                            }
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
                    LobbyTitle.text = ("Connecting to Server");
                    Debug.Log("Connecting to: " + PhotonNetwork.ServerAddress);
                }
                else
                {
                    Debug.LogError("Not connected. Check console output. Detailed connection state: " + PhotonNetwork.connectionStateDetailed + " Server: " + PhotonNetwork.ServerAddress);
                }

                if (this.connectFailed)
                {
                    LobbyTitle.text = ("CONNECTION FAILED");
                }
            }
            else
            {
                if (!isTitleUpdated)
                {
                    Debug.Log("Connected!");

                    LobbyTitle.text = ("Join or Create a Room");

                    // Save name
                    PlayerPrefs.SetString("playerName", PhotonNetwork.playerName);

                    isTitleUpdated = true;
                }

                UsersText.text = (PhotonNetwork.countOfPlayers + " users are online in " + PhotonNetwork.countOfRooms + " rooms.");
            }
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

    System.Collections.IEnumerator UpdateLobbyTitleText(string tempText)
    {
        LobbyTitle.text = tempText;

        //Wait for 3 seconds then change it back to default
        yield return new WaitForSeconds(3f);

        if (!PhotonNetwork.connected)
        {
            if (PhotonNetwork.connecting)
                LobbyTitle.text = ("Connecting to Server");
            if (this.connectFailed)
                LobbyTitle.text = ("CONNECTION FAILED");
        }
        else
        {
            LobbyTitle.text = ("Join or Create a Room");
        }
    }

    Vector2 TextMeshArea(TextMesh textmesh)
    {
        Quaternion rotation = textmesh.gameObject.transform.rotation;
        textmesh.gameObject.transform.rotation = new Quaternion();
        Vector2 ret = new Vector2(textmesh.GetComponent<Renderer>().bounds.size.x, textmesh.GetComponent<Renderer>().bounds.size.y);
        textmesh.gameObject.transform.rotation = rotation;
        return ret;
    }

    public void UpdatePage(LobbyPageButtonScript.PageDirection pageDirection)
    {
        int originalPage = currentPage;
        if (pageDirection.Equals(LobbyPageButtonScript.PageDirection.Up))
            currentPage++;
        else
            currentPage--;

        if (currentPage < 1)
            currentPage = 1;
        if (currentPage > maxPages)
            currentPage = maxPages;

        // If you actually changed pages clear the room button list & update
        if (currentPage != originalPage)
        {
            foreach (GameObject roomButton in RoomButtonList)
            {
                roomButton.SetActive(false);
            }
            RoomListUpdate();
        }
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
        roomOptions.CustomRoomProperties = new Hashtable() { { PhotonConstants.pPosOccupied, playerPosOccupied } };
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 4;
        if(PhotonNetwork.CreateRoom(PhotonNetwork.player.name, roomOptions, TypedLobby.Default))
        {
#if OCULUS
            //Save your OVRCameraRig to be added to your PlayerObject
            Transform ovrCameraRig = GameObject.FindGameObjectWithTag("Player").transform.Find("OVRCameraRig");
            ovrCameraRig.parent = null;
            DontDestroyOnLoad(ovrCameraRig);
#endif
        }
    }

    public void JoinRoom(string joinGameName)
    {
        Debug.Log("Joining Room: " + joinGameName);

        PhotonNetwork.player.customProperties.Clear();
        if(PhotonNetwork.JoinRoom(joinGameName))
        {
#if OCULUS
            //Save your OVRCameraRig to be added to your PlayerObject
            Transform ovrCameraRig = GameObject.FindGameObjectWithTag("Player").transform.Find("OVRCameraRig");
            ovrCameraRig.parent = null;
            DontDestroyOnLoad(ovrCameraRig);
#endif
        }
    }

    // We have two options here: we either joined(by title, list or random) or created a room.
    public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
    }

    public void OnPhotonCreateRoomFailed()
    {
        StartCoroutine(UpdateLobbyTitleText("Failed to create room"));
        ErrorDialog = "Error: Can't create room (room name maybe already used).";
        Debug.Log("OnPhotonCreateRoomFailed got called. This can happen if the room exists (even if not visible). Try another room name.");
    }

    public void OnPhotonJoinRoomFailed(object[] cause)
    {
        StartCoroutine(UpdateLobbyTitleText("Unable to join room"));
        ErrorDialog = "Error: Can't join room (full or unknown room name). " + cause[1];
        Debug.Log("OnPhotonJoinRoomFailed got called. This can happen if the room is not existing or full or closed.");
    }

    public void OnPhotonRandomJoinFailed()
    {
        StartCoroutine(UpdateLobbyTitleText("No rooms found"));
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
