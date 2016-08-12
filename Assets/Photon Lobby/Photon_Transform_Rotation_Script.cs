using UnityEngine;
using System.Collections;

public class Photon_Transform_Rotation_Script : Photon.MonoBehaviour
{

    // Use this for initialization
    void Start() {
        
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            //Network player, receive data
            pos = (Vector3)stream.ReceiveNext();
            rot = (Quaternion)stream.ReceiveNext();
        }
    }

    private Vector3 pos = Vector3.zero; //We lerp towards this
    private Quaternion rot = Quaternion.identity; //We lerp towards this

    // Update is called once per frame
    void Update()
    {
        if (!photonView.isMine)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 15);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 30);
        }
    }
}
