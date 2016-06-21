// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkerInGame.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.SceneManagement;
using OvrTouch.Hands;
using OvrTouch.Controllers;

public class PhotonGame : Photon.MonoBehaviour
{
    public Transform playerPrefab;
    public Transform player1Spawn;
    public Transform player2Spawn;
    public Transform player3Spawn;
    public Transform player4Spawn;

    public void Awake()
    {
        // in case we started this demo with the wrong scene being active, simply load the menu scene
        if (!PhotonNetwork.connected)
        {
            SceneManager.LoadScene(PhotonLobby.SceneNameMenu);
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

        string message;
        InRoomChat chatComponent = GetComponent<InRoomChat>();  // if we find a InRoomChat component, we print out a short message

        if (chatComponent != null)
        {
            // to check if this client is the new master...
            if (player.isLocal)
            {
                message = "You are Master Client now.";
            }
            else
            {
                message = player.name + " is Master Client now.";
            }


            chatComponent.AddLine(message); // the Chat method is a RPC. as we don't want to send an RPC and neither create a PhotonMessageInfo, lets call AddLine()
        }
    }

    public void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom (local)");

        // back to main menu
        SceneManager.LoadScene(PhotonLobby.SceneNameMenu);
    }

    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("OnDisconnectedFromPhoton");

        // back to main menu
        SceneManager.LoadScene(PhotonLobby.SceneNameMenu);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("OnPhotonInstantiate " + info.sender);    // you could use this info to store this or react
    }

    public void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        Debug.Log("OnPhotonPlayerConnected: " + player);
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        Debug.Log("OnPlayerDisconneced: " + player);
    }

    public void OnFailedToConnectToPhoton()
    {
        Debug.Log("OnFailedToConnectToPhoton");

        // back to main menu
        SceneManager.LoadScene(PhotonLobby.SceneNameMenu);
    }

    void OnLevelWasLoaded(int level)
    {
        //Find which position you're player is in
        int playerPosition = 1;
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            if (PhotonNetwork.playerName.Equals(player.name))
                break;
            else
                playerPosition++;
        }

        //Get transform of your position
        Transform currentPlayerTransform = player1Spawn;
        if (PhotonNetwork.isMasterClient)
            currentPlayerTransform = player1Spawn;
        else if (playerPosition == 1)
            currentPlayerTransform = player1Spawn;
        else if (playerPosition == 2)
            currentPlayerTransform = player2Spawn;
        else if (playerPosition == 3)
            currentPlayerTransform = player3Spawn;
        else if (playerPosition == 4)
            currentPlayerTransform = player4Spawn;

        // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
        Transform ovrRigPhoton = PhotonNetwork.Instantiate(this.playerPrefab.name, currentPlayerTransform.position, currentPlayerTransform.rotation, 0).transform;

        // Don't destroy the object when another client loads in
        DontDestroyOnLoad(ovrRigPhoton.gameObject);

        // Enable your own camera and scripts
        ovrRigPhoton.name = ovrRigPhoton.name + "-" + PhotonNetwork.playerName;
        ovrRigPhoton.Find("OVRCameraRig").gameObject.AddComponent<OVRManager>();
        ovrRigPhoton.Find("OVRCameraRig").GetComponent<OVRCameraRig>().enabled = true;
        ovrRigPhoton.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor").GetComponent<Camera>().enabled = true;
        ovrRigPhoton.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor").GetComponent<AudioListener>().enabled = true;
        ovrRigPhoton.Find("LeftHandPf").GetComponent<Hand>().enabled = true;
        ovrRigPhoton.Find("LeftHandPf").GetComponent<VelocityTracker>().enabled = true;
        ovrRigPhoton.Find("RightHandPf").GetComponent<Hand>().enabled = true;
        ovrRigPhoton.Find("RightHandPf").GetComponent<VelocityTracker>().enabled = true;
    }
}
