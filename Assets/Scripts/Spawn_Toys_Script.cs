using UnityEngine;
using System.Collections;

public class Spawn_Toys_Script : Photon.MonoBehaviour
{

    int numPlayers = 0;

    // Use this for initialization
    void Start () {
        if (PhotonNetwork.isMasterClient)
        {
            //Get number of players by the NetwokManager.numPlayers
            numPlayers = PhotonNetwork.playerList.Length;
            Debug.Log("numPlayers: " + numPlayers);

            StartCoroutine(WaitForPlayersToSpawn());

            //For each child (set of toys) spawn all objects under that set
            foreach (Transform toySet in transform)
            {
                foreach (Transform toy in toySet)
                {
                    object[] data = new object[2];
                    data[0] = toy.position;
                    data[1] = toy.localRotation.eulerAngles;
                    //Instantiate the appropriate prefab
                    if (toy.name.Contains("ToyBall"))
                        PhotonNetwork.InstantiateSceneObject("Toys/ToyBallPf", toy.position, toy.rotation, 0, data);
                    else if (toy.name.Contains("ToyCube"))
                        PhotonNetwork.InstantiateSceneObject("Toys/ToyCubePf", toy.position, toy.rotation, 0, data);
                    else if (toy.name.Contains("ToyT-Block"))
                        PhotonNetwork.InstantiateSceneObject("Toys/ToyT-BlockPf", toy.position, toy.rotation, 0, data);
                }
            }
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
    void Update () {
	
	}
}
