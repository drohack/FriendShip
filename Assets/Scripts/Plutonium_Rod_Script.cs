using UnityEngine;
using System.Collections;
using OvrTouch.Hands;

public class Plutonium_Rod_Script : Photon.MonoBehaviour
{

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    Grabbable grabbable;
    private int respawnTimeout = 5; //number of seconds that will pass before the object will respawn
    private float currentRespawnTime = 5f;
    private bool isCollidingWithSpawn = false;

	// Use this for initialization
	void Start () {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        grabbable = GetComponent<Grabbable>();
	}

    void OnTriggerEnter (Collider other)
    {
        if (other.tag.Equals("PlutoniumSpawn"))
            isCollidingWithSpawn = true;
    }

    void OnTriggerExit (Collider other)
    {
        if (other.tag.Equals("PlutoniumSpawn"))
            isCollidingWithSpawn = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (photonView.isMine)
        {
            // If the following is not true start the respawn countdown
            // Is in the spawn location
            // Is being grabbed
            if (!isCollidingWithSpawn && !grabbable.isGrabbing)
            {
                currentRespawnTime -= Time.deltaTime;
                if (currentRespawnTime <= 0f)
                {
                    //Respawn object before destroying this one
                    PhotonNetwork.Instantiate("Prefabs/Plutonium_Rod", spawnPosition, spawnRotation, 0);

                    //Destroy this object
                    PhotonNetwork.Destroy(gameObject);
                }
            }
            else
            {
                // Reset the respawn time
                currentRespawnTime = respawnTimeout;
            }
        }
	}

    [PunRPC]
    void RPCDestroy()
    {
        if (photonView.isMine)
        {
            //Respawn object before destroying this one
            PhotonNetwork.Instantiate("Prefabs/Plutonium_Rod", spawnPosition, spawnRotation, 0);

            //Destroy this object
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
