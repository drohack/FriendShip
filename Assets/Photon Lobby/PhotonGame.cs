// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkerInGame.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using OVRTouchSample;

public class PhotonGame : Photon.MonoBehaviour
{
    public Transform playerPrefab;
    public Transform player1Spawn;
    public Transform player2Spawn;
    public Transform player3Spawn;
    public Transform player4Spawn;

    GameObject mastermind = null;

    public void Awake()
    {
        // in case we started this demo with the wrong scene being active, simply load the menu scene
        if (!PhotonNetwork.connected)
        {
#if VIVE
            SteamVR_LoadLevel.Begin(PhotonLobby_VR.SceneNameMenu);
#else
	        SceneManager.LoadScene(PhotonLobby_VR.SceneNameMenu);
#endif
            return;
        }

        //Disable Main Camera (we will be using the OvrRigPhoton's camera
        GameObject.Find("Main Camera").SetActive(false);
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Return to Lobby"))
        {
            PhotonNetwork.LeaveRoom();  // we will load the menu level when we successfully left the room
        }
    }

    public void OnMasterClientSwitched(PhotonPlayer player)
    {
        Debug.Log("OnMasterClientSwitched: " + player);

        // to check if this client is the new master...
        if (player.isLocal)
        {
            //You are now the MasterClient
        }
    }

    public void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom (local)");

        // back to main menu
#if VIVE
        SteamVR_LoadLevel.Begin(PhotonLobby_VR.SceneNameMenu);
#else
	    SceneManager.LoadScene(PhotonLobby_VR.SceneNameMenu);
#endif
    }

    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("OnDisconnectedFromPhoton");

        // back to main menu
#if VIVE
        SteamVR_LoadLevel.Begin(PhotonLobby_VR.SceneNameMenu);
#else
	    SceneManager.LoadScene(PhotonLobby_VR.SceneNameMenu);
#endif
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("OnPhotonInstantiate " + info.sender);    // you could use this info to store this or react
    }

    public void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        Debug.Log("OnPhotonPlayerConnected: " + player);
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        Debug.Log("OnPlayerDisconneced: " + otherPlayer);

        if (PhotonNetwork.isMasterClient)
        {
            //End the game
            if (mastermind != null)
            {
                //You were the original MasterClient you can just end the game
                mastermind.GetComponent<Mastermind_Script>().GameOver();
            }
            else
            {
                //You are the new MasterClient, find the Mastermine script and end the game
                mastermind = GameObject.FindGameObjectWithTag("Mastermind");
                mastermind.GetComponent<Mastermind_Script>().GameOver();
            }

            //Open up player position
            if (otherPlayer.customProperties.ContainsKey(PhotonConstants.pPos) && PhotonNetwork.room.customProperties.ContainsKey(PhotonConstants.pPosOccupied))
            {
                int otherPlayerPos = (int)otherPlayer.customProperties[PhotonConstants.pPos];
                bool[] playerPosOccupied = (bool[])PhotonNetwork.room.customProperties[PhotonConstants.pPosOccupied];
                playerPosOccupied[otherPlayerPos] = false;
                ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable() { { PhotonConstants.pPosOccupied, playerPosOccupied } };
                PhotonNetwork.room.SetCustomProperties(ht);
                otherPlayer.customProperties.Clear();
            }
        }
    }

    public void OnFailedToConnectToPhoton()
    {
        Debug.Log("OnFailedToConnectToPhoton");

        // back to main menu
#if VIVE
        SteamVR_LoadLevel.Begin(PhotonLobby_VR.SceneNameMenu);
#else
	    SceneManager.LoadScene(PhotonLobby_VR.SceneNameMenu);
#endif
    }

    void OnLevelWasLoaded(int level)
    {
        if (PhotonNetwork.isMasterClient)
        {
            mastermind = PhotonNetwork.InstantiateSceneObject("Mastermind", Vector3.zero, Quaternion.identity, 0, null);
        }

        //Update new player's isLoadedIntoGame
        Hashtable ht2 = new Hashtable() { { PhotonConstants.isLoadedIntoGame, true } };
        PhotonNetwork.player.SetCustomProperties(ht2);
    }
}
