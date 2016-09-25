using UnityEngine;
using System.Collections;

public class Spawn_Plutonium_Rods : Photon.MonoBehaviour
{

    int numPlayers = 0;

    // Use this for initialization
    void Start () {
        if (PhotonNetwork.isMasterClient)
        {
            //Get number of players by the NetwokManager.numPlayers
            numPlayers = PhotonNetwork.playerList.Length;

            StartCoroutine(WaitForPlayersToSpawn());

            //For each child (set of toys) spawn all objects under that set
            foreach (Transform plutoniumRodSet in transform)
            {
                foreach (Transform plutoniumRod in plutoniumRodSet)
                {
                    object[] data = new object[2];
                    data[0] = plutoniumRod.position;
                    data[1] = plutoniumRod.localRotation.eulerAngles;
                    //Instantiate the plutoniumRod
                    PhotonNetwork.InstantiateSceneObject("Modules/Plutonium_Rod", plutoniumRod.position, plutoniumRod.rotation, 0, data);
                }
            }
        }
    }

    IEnumerator WaitForPlayersToSpawn()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        while (playerObjects.Length < numPlayers)
        {
            playerObjects = GameObject.FindGameObjectsWithTag("Player");
            yield return new WaitForSeconds(0.1f);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
