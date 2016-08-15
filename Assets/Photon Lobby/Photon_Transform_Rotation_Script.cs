using UnityEngine;
using System.Collections;

public class Photon_Transform_Rotation_Script : Photon.MonoBehaviour
{

    private bool isRequestingOwnership = false;

    // Use this for initialization
    void Start() {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(!isRequestingOwnership && this.photonView.ownerId != PhotonNetwork.player.ID)
        {
            isRequestingOwnership = true;
            this.photonView.RequestOwnership();
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(isRequestingOwnership && photonView.isMine)
        {
            isRequestingOwnership = false;
        }
    }
}
