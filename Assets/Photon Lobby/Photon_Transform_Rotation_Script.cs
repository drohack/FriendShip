using UnityEngine;
using System.Collections;
using OvrTouch.Hands;

public class Photon_Transform_Rotation_Script : Photon.MonoBehaviour
{
    private int numTriggered = 0;
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
        if(other.tag.Equals("Hand"))
        {
            numTriggered++;
        }

        if(other.tag.Equals("Hand") && !isRequestingOwnership && this.photonView.ownerId != PhotonNetwork.player.ID)
        {
            isRequestingOwnership = true;
            this.photonView.RequestOwnership();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag.Equals("Hand"))
        {
            numTriggered--;
        }

        if(numTriggered == 0 && !isGrabbing && !PhotonNetwork.isMasterClient)
        {
            //pass ownership back to master client
            photonView.RPC("RPCMasterClientTakeOwnership", PhotonTargets.MasterClient, null);
        }
    }

    private void OnGrabBegin(GrabbableGrabMsg grabMsg)
    {
        numTriggered = 0;
        isGrabbing = true;
    }
    private void OnGrabEnd(GrabbableGrabMsg grabMsg)
    {
        isGrabbing = false;
    }

    [PunRPC]
    void RPCMasterClientTakeOwnership()
    {
        if(PhotonNetwork.isMasterClient)
            this.photonView.RequestOwnership();
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
        }
        else
        {
            stream.Serialize(ref syncPosition);
            stream.Serialize(ref syncVelocity);
            stream.Serialize(ref syncLocalRotation);
            stream.Serialize(ref syncAngularVelocity);

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

        if(isRequestingOwnership && photonView.isMine)
        {
            isRequestingOwnership = false;
        }
    }
}
