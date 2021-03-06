﻿using UnityEngine;
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
    [SerializeField]
    private SkinnedMeshRenderer m_skinnedMeshRenderer;

    public Vector3 m_lastPos;
    public Quaternion m_lastRot;
    private Rigidbody m_rigidbody = null;
    private Velocity_Tracker m_velocityTracker = null;
    private Grabbable m_grabbedGrabbable = null;
    private Hand_Pose m_grabbedHandPose = null;
    private FixedJoint fixedJoint = null;
#if OCULUS
    private OVRTouchSample.Hand m_hand = null;
    private PhotonNetworkOvrRig parentRigScript = null;
#elif VIVE
    private Vive_Hand m_hand = null;
    private PhotonNetworkViveRig parentRigScript = null;
#endif

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
        // initialize last rot/pos
        m_lastPos = transform.position;
        m_lastRot = transform.rotation;
        // Get components
        m_rigidbody = this.GetComponent<Rigidbody>();
        m_velocityTracker = this.GetComponent<Velocity_Tracker>();
#if OCULUS
        m_hand = this.GetComponent<OVRTouchSample.Hand>();
        if (transform.parent != null && transform.parent.GetComponent<PhotonNetworkOvrRig>() != null)
            parentRigScript = transform.parent.GetComponent<PhotonNetworkOvrRig>();
#elif VIVE
        m_hand = this.GetComponent<Vive_Hand>();
        if (transform.parent != null && transform.parent.GetComponent<PhotonNetworkViveRig>() != null)
            parentRigScript = transform.parent.GetComponent<PhotonNetworkViveRig>();
#endif
    }

    // Update is called once per frame
    void FixedUpdate () {
        
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
            if (m_hand != null)
            {
                // Disable grab volumes to prevent overlaps
                m_hand.GrabVolumeEnable(false);
            }

            // Only run if object GrabMode is "Drag" or "Rotate"
            if (closestGrabbable.m_grabMode.Equals(Grabbable.GrabMode.Drag) || closestGrabbable.m_grabMode.Equals(Grabbable.GrabMode.Rotate))
            {
                // Set isKinematic to false so the hand doesn't bump into things
                m_rigidbody.isKinematic = false;
                // disable the hand geometry
                m_skinnedMeshRenderer.enabled = false;
                if (parentRigScript != null)
                {
                    parentRigScript.ToggleMeshRenderer(m_handedness, m_skinnedMeshRenderer.enabled);
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

            if (m_skinnedMeshRenderer.enabled == false)
            {
                // Enable hand geometry to pop back in
                m_skinnedMeshRenderer.enabled = true;
                if (parentRigScript != null)
                {
                    parentRigScript.ToggleMeshRenderer(m_handedness, m_skinnedMeshRenderer.enabled);
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
            GrabbableRelease(linearVelocity, angularVelocity, false);
        }
        
        if (m_hand != null)
        {
            // Re-enable grab volumes to allow overlap events
            m_hand.GrabVolumeEnable(true);
        }
    }

    //==============================================================================
    private void GrabbableGrab(Grabbable grabbable, Grab_Point grabPoint)
    {
        m_grabbedGrabbable = grabbable;
        m_grabbedGrabbable.GrabBegin(this, grabPoint);
        m_grabbedHandPose = m_grabbedGrabbable.HandPose;
        if (m_hand != null)
            m_hand.m_grabbedHandPose = m_grabbedHandPose;
    }

    //==============================================================================
    private void GrabbableRelease(Vector3 linearVelocity, Vector3 angularVelocity, bool isOffhandGrab)
    {
        m_grabbedGrabbable.GrabEnd(linearVelocity, angularVelocity, isOffhandGrab);
        m_grabbedHandPose = null;
        m_grabbedGrabbable = null;
        if (m_hand != null)
            m_hand.m_grabbedHandPose = m_grabbedHandPose;
    }

    //==============================================================================
    public void OffhandGrabbed(Grabbable grabbable)
    {
        if (m_grabbedGrabbable == grabbable)
        {
            GrabbableRelease(Vector3.zero, Vector3.zero, true);
        }
    }
}
