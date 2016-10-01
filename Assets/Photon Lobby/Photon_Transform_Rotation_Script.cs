using UnityEngine;
using System.Collections;
using OvrTouch.Hands;

[RequireComponent(typeof(Rigidbody), typeof(Grabbable))]
public class Photon_Transform_Rotation_Script : Photon.MonoBehaviour
{
    public bool isGrabbed = false;
    public bool isLoading = true;
    public bool isRequestingOwnership = false;
    public bool isOtherHandColliding = false;
    public int numHandTriggered = 0;
    public bool inPlayerArea = false;
    public bool isHandOff = false;
    private Rigidbody rigidBody;
    private Grabbable grabbableScript;

    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;
    private Vector3 syncStartLocalEulerAngles = Vector3.zero;
    private Vector3 syncEndLocalEulerAngles = Vector3.zero;
    private Vector3 newVelocity = Vector3.zero;
    private Vector3 newAngularVelocity = Vector3.zero;

    void Awake()
    {
        lastSynchronizationTime = Time.time;
    }

    // Use this for initialization
    void Start()
    {
        object[] data = photonView.instantiationData;
        if (data != null)
        {
            syncStartPosition = (Vector3)data[0];
            syncEndPosition = (Vector3)data[0];
            syncStartLocalEulerAngles = (Vector3)data[1];
            syncEndLocalEulerAngles = (Vector3)data[1];
        }
        rigidBody = GetComponent<Rigidbody>();
        grabbableScript = GetComponent<Grabbable>();

        isGrabbed = false;
        isRequestingOwnership = false;
        isOtherHandColliding = false;
        numHandTriggered = 0;

        isLoading = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Hand"))
        {
            numHandTriggered++;
        }
        else if (other.tag.Equals("PlayerArea"))
        {
            inPlayerArea = true;
        }

        //Request control if object enters your area OR if your hand passes through an object
        //as long as it's not bieng held by someone, another players hand isn't already colliding with it (first come first serve), 
        //you're not already requesting ownership, or you don't already own the object
        if (other.tag.Equals("PlayerArea") && !isGrabbed && !isOtherHandColliding && !isRequestingOwnership && this.photonView.ownerId != PhotonNetwork.player.ID)
        {
            isRequestingOwnership = true;
            this.photonView.RequestOwnership();
        }
        else if (other.tag.Equals("Hand") && !isGrabbed && !isOtherHandColliding && !isRequestingOwnership && this.photonView.ownerId != PhotonNetwork.player.ID)
        {
            isRequestingOwnership = true;
            this.photonView.RequestOwnership();
            //Send RPC to other players that you are colliding with this object (in case it is in a different players area)
            photonView.RPC("RPCSetIsOtherHandColliding", PhotonTargets.Others, true, isGrabbed);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Hand"))
        {
            numHandTriggered--;
            if (numHandTriggered < 0)
                numHandTriggered = 0;
        }
        else if (other.tag.Equals("PlayerArea"))
        {
            inPlayerArea = false;
        }

        if (numHandTriggered == 0 && !isGrabbed)
        {
            //Send RPC to other players that you are no longer colliding with this object
            photonView.RPC("RPCSetIsOtherHandColliding", PhotonTargets.Others, false, isGrabbed);
        }
    }

    private void OnGrabBegin(GrabbableGrabMsg grabMsg)
    {
        numHandTriggered = 0;
        isGrabbed = true;

        //If you own this object and you are grabbing it let other players know that they can grab this object again (for passing objects between players)
        //Else hand off the object by requesting the other player to drop the object while you request for ownership
        if (this.photonView.ownerId == PhotonNetwork.player.ID)
        {
            photonView.RPC("RPCSetIsOtherHandColliding", PhotonTargets.Others, false, isGrabbed);
        }
        else
        {
            photonView.RPC("RPCHandOffObject", PhotonTargets.All, PhotonNetwork.player.ID);
        }
    }

    private void OnGrabEnd(GrabbableGrabMsg grabMsg)
    {
        //If you hand off the object to another player the object is still grabbed
        if (isHandOff)
            isHandOff = false;
        else
            isGrabbed = false;
    }

    [PunRPC]
    void RPCSetIsOtherHandColliding(bool val1, bool val2)
    {
        isOtherHandColliding = val1;
        isGrabbed = val2;

        //If someone elses hand is no longer colliding with the object and it's in your play area request control over it
        if (!isOtherHandColliding && inPlayerArea && !isGrabbed && !isRequestingOwnership && this.photonView.ownerId != PhotonNetwork.player.ID)
        {
            isRequestingOwnership = true;
            this.photonView.RequestOwnership();
        }

        //If someone elses hand is colliding disable grabbable, else re-enable it (first come first serve)
        if (isOtherHandColliding)
        {
            grabbableScript.enabled = false;
        }
        else
        {
            grabbableScript.enabled = true;
        }
    }

    [PunRPC]
    void RPCHandOffObject(int requestorsId)
    {
        //If you own this object drop it
        //Else if you are the one taking the object (the requestor) then request owership
        if (this.photonView.ownerId == PhotonNetwork.player.ID)
        {
            grabbableScript.GrabbedHand.OffhandGrabbed(grabbableScript);
            isHandOff = true;
        }
        else if (requestorsId == PhotonNetwork.player.ID)
        {
            isRequestingOwnership = true;
            this.photonView.RequestOwnership();
        }
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

            stream.Serialize(ref isGrabbed);
        }
        else
        {
            stream.Serialize(ref syncPosition);
            stream.Serialize(ref syncVelocity);
            stream.Serialize(ref syncLocalRotation);
            stream.Serialize(ref syncAngularVelocity);
            stream.Serialize(ref isGrabbed);

            syncTime = 0f;
            syncDelay = Time.time - lastSynchronizationTime;
            lastSynchronizationTime = Time.time;

            syncStartPosition = transform.position;
            syncStartLocalEulerAngles = transform.localRotation.eulerAngles;

            //If not grabbed predict end position/local euler angles based on current position/local euler angles, 
            //the velocity the object is moving in, 
            //and the delay time of commands
            if (isGrabbed)
            {
                syncEndPosition = syncPosition;
                syncEndLocalEulerAngles = syncLocalRotation;
            }
            else
            {
                syncEndPosition = syncPosition + syncVelocity * syncDelay;
                syncEndLocalEulerAngles = syncLocalRotation + syncAngularVelocity * syncDelay;
            }

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
            transform.localRotation = Quaternion.Slerp(Quaternion.Euler(syncStartLocalEulerAngles), Quaternion.Euler(syncEndLocalEulerAngles), syncTime / syncDelay);
            //Only update the velocity of not held objects
            if (isGrabbed)
            {
                rigidBody.velocity = Vector3.zero;
                rigidBody.angularVelocity = Vector3.zero;
            }
            else
            {
                rigidBody.velocity = newVelocity;
                rigidBody.angularVelocity = newAngularVelocity;
            }
        }
        else if (photonView.isMine && !isLoading)
        {
            //keep the syncStartPosition, syncEndPosition, newVelocity, and newAngularVelocity up to date with current position
            syncStartPosition = transform.position;
            syncEndPosition = transform.position;
            newVelocity = rigidBody.velocity;
            newAngularVelocity = rigidBody.angularVelocity;
        }

        if (isRequestingOwnership && photonView.isMine)
        {
            isRequestingOwnership = false;
        }
    }
}