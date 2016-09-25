using UnityEngine;
using System.Collections.Generic;

public enum HandednessId
{
    Left,
    Right,
}

public class Grabbed_Hand_Script : MonoBehaviour {

    [SerializeField]
    public HandednessId m_handedness = HandednessId.Left;
    [SerializeField]
    private Transform m_gripTransform = null;

    private Rigidbody m_rigidbody = null;
    private Velocity_Tracker m_velocityTracker = null;
    private Grabbable m_grabbedGrabbable = null;
    private Hand_Pose m_grabbedHandPose = null;
    private FixedJoint fixedJoint = null;

    public const float THRESH_THROW_SPEED = 1.0f;

    //==============================================================================
    // Properties
    //==============================================================================

    public bool IsGrabbingGrabbable
    {
        get { return m_grabbedGrabbable != null; }
    }

    // Use this for initialization
    void Start () {
        // Get components
        m_rigidbody = this.GetComponent<Rigidbody>();
        m_velocityTracker = this.GetComponent<Velocity_Tracker>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    //==============================================================================
    public void GrabBegin(Dictionary<Grabbable, int> m_grabCandidates)
    {
        float closestMagSq = float.MaxValue;
        Grabbable closestGrabbable = null;
        Grab_Point closestGrabPoint = null;

        // Iterate grab candidates and find the closest grabbable candidate
        foreach (Grabbable grabbable in m_grabCandidates.Keys)
        {
            // Determine if the grabbable can be grabbed
            bool canGrab = grabbable != null && !(grabbable.isGrabbed && !grabbable.AllowOffhandGrab);
            if (!canGrab)
            {
                continue;
            }

            foreach (Grab_Point grabPoint in grabbable.GrabPoints)
            {
                // Store the closest grabbable
                Vector3 closestPointOnBounds = grabPoint.GrabCollider.ClosestPointOnBounds(m_gripTransform.position);
                float grabbableMagSq = (m_gripTransform.position - closestPointOnBounds).sqrMagnitude;
                if (grabbableMagSq < closestMagSq)
                {
                    closestMagSq = grabbableMagSq;
                    closestGrabbable = grabbable;
                    closestGrabPoint = grabPoint;
                }
            }
        }

        if (closestGrabbable != null && !closestGrabbable.m_grabMode.Equals(Grabbable.GrabMode.None))
        {
            if (this.GetComponent<OVRTouchSample.Hand>() != null)
            {
                // Disable grab volumes to prevent overlaps
                this.GetComponent<OVRTouchSample.Hand>().GrabVolumeEnable(false);
            }

            // Only run if object GrabMode is "Drag" or "Rotate"
            if (closestGrabbable.m_grabMode.Equals(Grabbable.GrabMode.Drag) || closestGrabbable.m_grabMode.Equals(Grabbable.GrabMode.Rotate))
            {
                // Set isKinematic to false so the hand doesn't bump into things
                m_rigidbody.isKinematic = false;
                // disable the hand geometry
                transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().enabled = false;
                if (transform.parent != null && transform.parent.GetComponent<PhotonNetworkOvrRig>() != null)
                {
                    transform.parent.GetComponent<PhotonNetworkOvrRig>().ToggleMeshRenderer(m_handedness, transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().enabled);
                }
            }
        }

        if (closestGrabbable != null && !closestGrabbable.m_grabMode.Equals(Grabbable.GrabMode.None))
        {
            if (closestGrabbable.isGrabbed)
            {
                // Release the grabbable from the another hand
                closestGrabbable.GrabbedHand.GetComponent<Grabbed_Hand_Script>().OffhandGrabbed(closestGrabbable);
            }

            // Grab the grabbable
            GrabbableGrab(closestGrabbable, closestGrabPoint);

            // If grabbable is Rotate attach the fixed joint to the object
            if (m_grabbedGrabbable.m_grabMode.Equals(Grabbable.GrabMode.Rotate))
            {
                fixedJoint = m_grabbedGrabbable.gameObject.AddComponent<FixedJoint>();
                fixedJoint.connectedBody = m_rigidbody;
            }
        }
    }

    //==============================================================================
    public void GrabEnd()
    {
        if (IsGrabbingGrabbable)
        {
            // If grabbable is Rotate remove fixed joint
            if (m_grabbedGrabbable.m_grabMode.Equals(Grabbable.GrabMode.Rotate))
            {
                Object.Destroy(fixedJoint);
                fixedJoint = null;
            }

            if (transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().enabled == false)
            {
                // Enable hand geometry to pop back in
                transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().enabled = true;
                if (transform.parent != null && transform.parent.GetComponent<PhotonNetworkOvrRig>() != null)
                {
                    transform.parent.GetComponent<PhotonNetworkOvrRig>().ToggleMeshRenderer(m_handedness, transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().enabled);
                }
                //set isKinematic to false so the hand doesn't bump into things
                m_rigidbody.isKinematic = true;
            }

            // Determine if the grabbable was thrown
            bool wasThrown = m_velocityTracker.TrackedLinearVelocity.magnitude >= THRESH_THROW_SPEED;

            // Compute release velocities
            Vector3 linearVelocity = Vector3.zero;
            Vector3 angularVelocity = Vector3.zero;
            if (wasThrown)
            {
                // Throw velocity
                linearVelocity = m_velocityTracker.TrackedLinearVelocity;
                angularVelocity = m_velocityTracker.TrackedAngularVelocity;
            }
            else {
                // Drop velocity
                linearVelocity = m_velocityTracker.FrameLinearVelocity;
                angularVelocity = m_velocityTracker.FrameAngularVelocity;
            }

            // Release the grabbable
            GrabbableRelease(linearVelocity, angularVelocity);
        }

        if (this.GetComponent<OVRTouchSample.Hand>() != null)
        {
            // Re-enable grab volumes to allow overlap events
            this.GetComponent<OVRTouchSample.Hand>().GrabVolumeEnable(true);
        }
    }

    //==============================================================================
    private void GrabbableGrab(Grabbable grabbable, Grab_Point grabPoint)
    {
        m_grabbedGrabbable = grabbable;
        m_grabbedGrabbable.GrabBegin(this, grabPoint);
        m_grabbedHandPose = m_grabbedGrabbable.HandPose;
        if (this.GetComponent<OVRTouchSample.Hand>() != null)
            this.GetComponent<OVRTouchSample.Hand>().m_grabbedHandPose = m_grabbedHandPose;
    }

    //==============================================================================
    private void GrabbableRelease(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        m_grabbedGrabbable.GrabEnd(linearVelocity, angularVelocity);
        m_grabbedHandPose = null;
        m_grabbedGrabbable = null;
        if (this.GetComponent<OVRTouchSample.Hand>() != null)
            this.GetComponent<OVRTouchSample.Hand>().m_grabbedHandPose = m_grabbedHandPose;
    }

    //==============================================================================
    public void OffhandGrabbed(Grabbable grabbable)
    {
        if (m_grabbedGrabbable == grabbable)
        {
            GrabbableRelease(Vector3.zero, Vector3.zero);
        }
    }
}
