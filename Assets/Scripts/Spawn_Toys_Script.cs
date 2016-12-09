using UnityEngine;
using System.Collections;
using System.Linq;

public class Spawn_Toys_Script : Photon.MonoBehaviour
{

    int numPlayers = 0;

    // Use this for initialization
    void Start () {
        if (PhotonNetwork.isMasterClient)
        {
            StartCoroutine(WaitForPlayersToSpawn());
        }
	}

    IEnumerator WaitForPlayersToSpawn()
    {
        yield return new WaitForEndOfFrame(); 

        //Wait for all players to load into the scene
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains(PhotonMainMenu.SceneNameGame))
        {
            bool areAllPlayersReady = false;
            do
            {
                bool[] playerPosOccupied = (bool[])PhotonNetwork.room.customProperties[PhotonConstants.pPosOccupied];
                bool[] playerLoadedList = new bool[playerPosOccupied.Length];
                foreach (PhotonPlayer p in PhotonNetwork.playerList)
                {
                    if (p.customProperties.ContainsKey(PhotonConstants.isLoadedIntoGame) && p.customProperties.ContainsKey(PhotonConstants.pPos))
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
        }

        Spawn();
    }

    private void Spawn()
    {
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

    // Update is called once per frame
    void Update () {
	
	}
}
