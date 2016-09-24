using UnityEngine;
using System.Collections;

public class Spawn_Abort_Reset : Photon.MonoBehaviour
{
    int numPlayers = 0;
    public int playerNum = 0;
    [SerializeField]
    Transform AbortButton;
    [SerializeField]
    Transform ResetButton; 

    // Use this for initialization
    void Start()
    {
        if (PhotonNetwork.isMasterClient)
        {
            //Get number of players by the NetwokManager.numPlayers
            numPlayers = PhotonNetwork.playerList.Length;

            StartCoroutine(WaitForPlayersToSpawn());
            object[] data = new object[1];
            data[0] = playerNum;
            PhotonNetwork.InstantiateSceneObject("Abort Button", AbortButton.position, AbortButton.rotation, 0, data);
            PhotonNetwork.InstantiateSceneObject("Reset Button", ResetButton.position, ResetButton.rotation, 0, data);
        }
    }

    IEnumerator WaitForPlayersToSpawn()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        //Debug.Log("numPlayers: " + numPlayers + "playerObjects.Length: " + playerObjects.Length);
        while (playerObjects.Length < numPlayers)
        {
            playerObjects = GameObject.FindGameObjectsWithTag("Player");
            //Debug.Log("numPlayers: " + numPlayers + " playerObjects.Length: " + playerObjects.Length);
            yield return new WaitForSeconds(0.1f);
        }

        //foreach (GameObject player in playerObjects)
        //{
        //    Debug.Log("name: " + player.name);
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }
}
