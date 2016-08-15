using UnityEngine;
using System.Collections;

public class Photon_Transform_Rotation_Script : Photon.MonoBehaviour
{

    private bool isRequestingOwnership = false;
    private Rigidbody rigidBody;

    // Use this for initialization
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(!isRequestingOwnership && this.photonView.ownerId != PhotonNetwork.player.ID)
        {
            isRequestingOwnership = true;
            this.photonView.RequestOwnership();
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(rigidBody.velocity);
            stream.SendNext(isRequestingOwnership);
        }
        else
        {
            //Network player, receive data
            pos = (Vector3)stream.ReceiveNext();
            rot = (Quaternion)stream.ReceiveNext();
            vel = (Vector3)stream.ReceiveNext();
            isRequestingOwnership = (bool)stream.ReceiveNext();
        }
    }

    private Vector3 pos = Vector3.zero; //We lerp towards this
    private Quaternion rot = Quaternion.identity; //We lerp towards this
    private Vector3 vel = Vector3.zero; //We lerp towards this

    // Update is called once per frame
    void LateUpdate()
    {
        if (!photonView.isMine)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 10);
            rigidBody.velocity = vel;
        }

        if(isRequestingOwnership && photonView.isMine)
        {
            isRequestingOwnership = false;
        }
    }
}
