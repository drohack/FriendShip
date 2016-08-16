using UnityEngine;
using System.Collections;
using OvrTouch.Hands;

public class Photon_Transform_Rotation_Script : Photon.MonoBehaviour
{
    private bool isGrabbing = false;
    private bool isLoading = true;
    private bool isRequestingOwnership = false;
    private Rigidbody rigidBody;

    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;
    private Vector3 syncStartRotation = Vector3.zero;
    private Vector3 syncEndRotation = Vector3.zero;
    private Vector3 newVelocity = Vector3.zero;
    private Vector3 newAngularVelocity = Vector3.zero;

    void Awake()
    {
        lastSynchronizationTime = Time.time;
    }

    // Use this for initialization
    void Start() {
        object[] data = photonView.instantiationData;
        if (data != null)
        {
            syncStartPosition = (Vector3)data[0];
            syncEndPosition = (Vector3)data[0];
            syncStartRotation = (Vector3)data[1];
            syncEndRotation = (Vector3)data[1];
        }
        rigidBody = GetComponent<Rigidbody>();

        isLoading = false;
    }

    void OnTriggerEnter(Collider other)
    {
        //Request control if object enters your area OR if your hand passes through an object
        if(other.tag.Equals("PlayerArea") && !isGrabbing && !isRequestingOwnership && this.photonView.ownerId != PhotonNetwork.player.ID)
        {
            isRequestingOwnership = true;
            this.photonView.RequestOwnership();
        }
        else if (other.tag.Equals("Hand") && !isRequestingOwnership && this.photonView.ownerId != PhotonNetwork.player.ID)
        {
            isRequestingOwnership = true;
            this.photonView.RequestOwnership();
        }
    }

    //Check to see if object is in your area, not being held by someone, and not owned by you. Request control
    void OnTriggerStay(Collider other)
    {
        if(other.tag.Equals("PlayerArea") && !isGrabbing && !isRequestingOwnership && this.photonView.ownerId != PhotonNetwork.player.ID)
        {
            isRequestingOwnership = true;
            this.photonView.RequestOwnership();
        }
    }

    private void OnGrabBegin(GrabbableGrabMsg grabMsg)
    {
        isGrabbing = true;
    }
    private void OnGrabEnd(GrabbableGrabMsg grabMsg)
    {
        isGrabbing = false;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Vector3 syncPosition = Vector3.zero;
        Vector3 syncVelocity = Vector3.zero;
        Vector3 syncLocalRotation = Vector3.zero;
        Vector3 syncAngularVelocity = Vector3.zero;
        if (stream.isWriting)
        {
            syncPosition = transform.position;
            stream.Serialize(ref syncPosition);

            syncVelocity = rigidBody.velocity;
            stream.Serialize(ref syncVelocity);

            syncLocalRotation = transform.localRotation.eulerAngles;
            stream.Serialize(ref syncLocalRotation);

            syncAngularVelocity = rigidBody.angularVelocity;
            stream.Serialize(ref syncAngularVelocity);

            stream.Serialize(ref isGrabbing);
        }
        else
        {
            stream.Serialize(ref syncPosition);
            stream.Serialize(ref syncVelocity);
            stream.Serialize(ref syncLocalRotation);
            stream.Serialize(ref syncAngularVelocity);
            stream.Serialize(ref isGrabbing);

            syncTime = 0f;
            syncDelay = Time.time - lastSynchronizationTime;
            lastSynchronizationTime = Time.time;

            syncEndPosition = syncPosition + syncVelocity * syncDelay;
            syncStartPosition = transform.position;

            syncEndRotation = syncLocalRotation + syncAngularVelocity * syncDelay;
            syncStartRotation = transform.localRotation.eulerAngles;

            newVelocity = syncVelocity;
            newAngularVelocity = syncAngularVelocity;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!photonView.isMine && !isLoading)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            syncTime += Time.deltaTime;

            transform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
            transform.localRotation = Quaternion.Slerp(Quaternion.Euler(syncStartRotation), Quaternion.Euler(syncEndRotation), syncTime / syncDelay);
            rigidBody.velocity = newVelocity;
            rigidBody.angularVelocity = newAngularVelocity;
        }
        else if (photonView.isMine && !isLoading)
        {
            //keep the syncStartPosition, syncEndPosition, newVelocity, and newAngularVelocity up to date with current position
            syncStartPosition = transform.position;
            syncEndPosition = transform.position;
            newVelocity = rigidBody.velocity;
            newAngularVelocity = rigidBody.angularVelocity;
        }

        if(isRequestingOwnership && photonView.isMine)
        {
            isRequestingOwnership = false;
        }
    }
}
