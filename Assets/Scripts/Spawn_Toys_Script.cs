using UnityEngine;
using System.Collections;

public class Spawn_Toys_Script : Photon.MonoBehaviour
{

	// Use this for initialization
	void Start () {
        if (PhotonNetwork.isMasterClient)
        {
            //For each child (set of toys) spawn all objects under that set
            foreach (Transform toySet in transform)
            {
                foreach (Transform toy in toySet)
                {
                    //Instantiate the appropriate prefab
                    if (toy.name.Contains("ToyBall"))
                        PhotonNetwork.InstantiateSceneObject("Toys/ToyBallPf", toy.position, toy.rotation, 0, null);
                    else if (toy.name.Contains("ToyCube"))
                        PhotonNetwork.InstantiateSceneObject("Toys/ToyCubePf", toy.position, toy.rotation, 0, null);
                    else if (toy.name.Contains("ToyT-Block"))
                        PhotonNetwork.InstantiateSceneObject("Toys/ToyT-BlockPf", toy.position, toy.rotation, 0, null);
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
