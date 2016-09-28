using UnityEngine;
using System.Collections;

public class Spawn_Abort_Reset : Photon.MonoBehaviour
{
    [SerializeField]
    int playerNum;
    [SerializeField]
    Transform AbortButton;
    [SerializeField]
    Transform ResetButton; 

    // Use this for initialization
    void Start()
    {
        if (PhotonNetwork.isMasterClient)
        {
            object[] data = new object[1];
            data[0] = playerNum;
            PhotonNetwork.InstantiateSceneObject("Abort Button", AbortButton.position, AbortButton.rotation, 0, data);
            PhotonNetwork.InstantiateSceneObject("Reset Button", ResetButton.position, ResetButton.rotation, 0, data);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
