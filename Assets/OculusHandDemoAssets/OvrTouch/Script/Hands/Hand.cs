/********************************************************************************//**
\file      Hand.cs
\brief     Basic hand impementation.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OvrTouch.Controllers;

namespace OvrTouch.Hands {

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(VelocityTracker))]
    public class Hand : MonoBehaviour {

        //==============================================================================
        // Nested Types
        //==============================================================================

        private static class Const {

            public const string AnimLayerNamePoint = "Point Layer";
            public const string AnimLayerNameThumb = "Thumb Layer";
            public const string AnimParamNameFlex = "Flex";
            public const string AnimParamNamePose = "Pose";

            public const float HapticOverlapAmplitude = 0.25f;
            public const float HapticOverlapFrequency = 320.0f;
            public const float HapticOverlapDuration = 0.05f;

            public const float InputRateChange = 20.0f;

            public static readonly RegistrationTransform RegistrationLeft = new RegistrationTransform(
                new Vector3(-0.02f, -0.04f, -0.03f),
                Quaternion.Euler(21.407562f, -0.319427f, 77.774536f)
            );

            public static readonly RegistrationTransform RegistrationRight = new RegistrationTransform(
                new Vector3(0.02f, -0.04f, -0.03f),
                Quaternion.Euler(21.407562f, -0.319427f, -77.774536f)
            );

            public const float ThreshCollisionFlex = 0.96f;
            public const float ThreshGrabBegin = 0.55f;
            public const float ThreshGrabEnd = 0.35f;
            public const float ThreshThrowSpeed = 1.0f;

        }

        private struct RegistrationTransform {

            //==============================================================================
            // Fields
            //==============================================================================

            public Vector3 Translation;
            public Quaternion Rotation;

            //==============================================================================
            // Public
            //==============================================================================

            //==============================================================================
            public RegistrationTransform (Vector3 translation, Quaternion rotation) {
                Translation = translation;
                Rotation = rotation;
            }

        };

        //==============================================================================
        // Fields
        //==============================================================================

        [SerializeField] public HandednessId m_handedness = HandednessId.Left;
        [SerializeField] private Transform m_gripTransform = null;
        [SerializeField] private Animator m_animator = null;
        [SerializeField] private Transform m_meshRoot = null;
        [SerializeField] private HandParticles m_handParticlesPf = null;
        [SerializeField] private Collider[] m_grabVolumes = null;

        private TrackedController m_trackedController = null;
        private VelocityTracker m_velocityTracker = null;
        private Rigidbody m_rigidbody = null;
        //private Collider[] m_colliders = null;
        //private bool m_collisionEnabled = true;
        private bool m_grabVolumeEnabled = true;
        private bool m_wasGrabVolumeEnabled = true;

        private int m_animLayerIndexThumb = -1;
        private int m_animLayerIndexPoint = -1;
        private int m_animParamIndexFlex = -1;
        private int m_animParamIndexPose = -1;

        public float m_flex = 0.0f;
        public float m_trigger = 0.0f;
        public float m_point = 0.0f;
        public float m_thumbsUp = 0.0f;

        public HandPoseId handPoseId;
        public bool canPoint;
        public bool canThumbsUp;

        private HandPose m_grabbedHandPose = null;
        private Grabbable m_grabbedGrabbable = null;
        private Dictionary<Grabbable, int> m_grabCandidates = new Dictionary<Grabbable, int>();

        private FixedJoint fixedJoint = null;

        //==============================================================================
        // Properties
        //==============================================================================

        public HandednessId Handedness {
            get { return m_handedness; }
        }

        public bool IsGrabbingGrabbable {
            get { return m_grabbedGrabbable != null; }
        }

        public Rigidbody Rigidbody {
            get { return m_rigidbody; }
        }

        public Vector3 LinearVelocity {
            get { return m_velocityTracker.TrackedLinearVelocity; }
        }

        public Vector3 AngularVelocity {
            get { return m_velocityTracker.TrackedAngularVelocity; }
        }

        public TrackedController TrackedController {
            get { return m_trackedController; }
        }

        //==============================================================================
        // Public
        //==============================================================================

        //==============================================================================
        public void SetVisible (bool visible) {
            m_meshRoot.gameObject.SetActive(visible);
        }

        //==============================================================================
        // MonoBehaviour
        //==============================================================================

        //==============================================================================
        private void Start () {
            // Get all collision and disable it
            //m_colliders = this.GetComponentsInChildren<Collider>().Where(childCollider => !childCollider.isTrigger).ToArray();
            //CollisionEnable(false);

            // Get components
            m_rigidbody = this.GetComponent<Rigidbody>();
            m_velocityTracker = this.GetComponent<VelocityTracker>();

            // Get animator indices by name
            m_animLayerIndexPoint = m_animator.GetLayerIndex(Const.AnimLayerNamePoint);
            m_animLayerIndexThumb = m_animator.GetLayerIndex(Const.AnimLayerNameThumb);
            m_animParamIndexFlex = Animator.StringToHash(Const.AnimParamNameFlex);
            m_animParamIndexPose = Animator.StringToHash(Const.AnimParamNamePose);

            // Spawn fx
            HandParticles handParticles = GameObject.Instantiate<HandParticles>(m_handParticlesPf);
            handParticles.transform.parent = this.transform;
            handParticles.transform.position = this.transform.position;
            handParticles.transform.rotation = this.transform.rotation;
            handParticles.SetHand(this);

            // Find the tracked controller
            m_trackedController = TrackedController.FindOrCreate(m_handedness);
        }

        //==============================================================================
	    private void FixedUpdate () {
            float prevFlex = m_flex;
            float prevTrigger = m_trigger;

            // Update values from inputs
            m_flex = InputFlex();
            m_trigger = InputTrigger();
            m_point = InputValueRateChange(InputPoint(), m_point);
            m_thumbsUp = InputValueRateChange(InputThumbsUp(), m_thumbsUp);
            
            // Advance the hand
            GrabVolumeAdvance();
            GrabAdvance(prevFlex, prevTrigger);
            //CollisionAdvance();
            AnimationAdvance();
	    }

        //==============================================================================
        private void LateUpdate () {
            Vector3 finalPosition = this.transform.position;
            Quaternion finalRotation = this.transform.rotation;

            // Compute final transform based on tracked transform and hand registration
            RegistrationTransform handRegistration = HandRegistration();
            finalPosition = m_trackedController.transform.position + m_trackedController.transform.rotation * handRegistration.Translation;
            finalRotation = m_trackedController.transform.rotation * handRegistration.Rotation;

            // Move the grabbable
            GrabbableAdvance(finalPosition, finalRotation);

            // Move the hand
            this.transform.position = finalPosition;
            this.transform.rotation = finalRotation;
        }

        //==============================================================================
        private void OnTriggerEnter (Collider otherCollider) {
            // Get the grab trigger
            GrabTrigger grabTrigger = otherCollider.GetComponent<GrabTrigger>();
            if (grabTrigger == null) {
                return;
            }

            // Get the grabbable
            Grabbable grabbable = grabTrigger.Grabbable;

            // Add the grabbable
            int refCount = 0;
            m_grabCandidates.TryGetValue(grabbable, out refCount);
            m_grabCandidates[grabbable] = refCount + 1;

            if (refCount == 0) {
                // Overlap begin
                grabbable.OverlapBegin(this);

                if (m_wasGrabVolumeEnabled == m_grabVolumeEnabled && m_trackedController != null) {
                    // Only play overlap haptics when there was no initial overlap (like after a grab release)
                    m_trackedController.PlayHapticEvent(
                        Const.HapticOverlapFrequency,
                        Const.HapticOverlapAmplitude,
                        Const.HapticOverlapDuration
                    );
                }
            }
        }

        //==============================================================================
        private void OnTriggerExit (Collider otherCollider) {
            // Get the grab trigger
            GrabTrigger grabbableVolume = otherCollider.GetComponent<GrabTrigger>();
            if (grabbableVolume == null) {
                return;
            }

            // Get the grabbable
            Grabbable grabbable = grabbableVolume.Grabbable;

            // Remove the grabbable
            int refCount = 0;
            bool found = m_grabCandidates.TryGetValue(grabbable, out refCount);
            if (!found) {
                return;
            }

            if (refCount > 1) {
                m_grabCandidates[grabbable] = refCount - 1;
            }
            else {
                // Overlap end
                grabbable.OverlapEnd(this);
                m_grabCandidates.Remove(grabbable);
            }
        }

        //==============================================================================
        // Private
        //==============================================================================

        //==============================================================================
        private float InputFlex () {
            return m_trackedController.GripTrigger;
        }

        private float InputTrigger()
        {
            return m_trackedController.Trigger;
        }

        //==============================================================================
        private bool InputPoint () {
            return m_trackedController.IsPoint;
        }

        //==============================================================================
        private bool InputThumbsUp () {
            return m_trackedController.IsThumbsUp;
        }

        //==============================================================================
        private float InputValueRateChange (bool isDown, float value) {
            float rateDelta = Time.deltaTime * Const.InputRateChange;
            float sign = isDown ? 1.0f : -1.0f;
            return Mathf.Clamp01(value + rateDelta * sign);
        }

        //==============================================================================
        private void AnimationAdvance () {
            // Pose
            this.handPoseId = (m_grabbedHandPose != null) ? m_grabbedHandPose.PoseId : HandPoseId.Default;
            m_animator.SetInteger(m_animParamIndexPose, (int)handPoseId);

            // Flex
            m_animator.SetFloat(m_animParamIndexFlex, m_flex);

            // Point
            this.canPoint = !IsGrabbingGrabbable || ((m_grabbedHandPose != null) && (m_grabbedHandPose.AllowPointing));
            float point = canPoint ? m_point : 0.0f;
            m_animator.SetLayerWeight(m_animLayerIndexPoint, point);

            // Thumbs up
            this.canThumbsUp = !IsGrabbingGrabbable || ((m_grabbedHandPose != null) && (m_grabbedHandPose.AllowThumbsUp));
            float thumbsUp = canThumbsUp ? m_thumbsUp : 0.0f;
            m_animator.SetLayerWeight(m_animLayerIndexThumb, thumbsUp);
    }

        //==============================================================================
        //private void CollisionAdvance () {
        //    bool collisionEnabled = (
        //        IsGrabbingGrabbable ||
        //        (m_flex >= Const.ThreshCollisionFlex)
        //    );
        //    CollisionEnable(collisionEnabled);
        //}

        //==============================================================================
        //private void CollisionEnable (bool enabled) {
        //    if (m_collisionEnabled == enabled) {
        //        return;
        //    }

        //    // Set collision state
        //    m_collisionEnabled = enabled;
        //    foreach (Collider collider in m_colliders) {
        //        collider.enabled = m_collisionEnabled;
        //    }
        //}

        //==============================================================================
        private void GrabAdvance (float prevFlex, float prevTrigger) {
            if (((m_flex >= Const.ThreshGrabBegin) && (prevFlex < Const.ThreshGrabBegin)) || ((m_trigger >= Const.ThreshGrabBegin) && (prevTrigger < Const.ThreshGrabBegin))) {
                GrabBegin();
            }
            else if (((m_flex <= Const.ThreshGrabEnd) && (prevFlex > Const.ThreshGrabEnd) && (m_trigger <= Const.ThreshGrabEnd)) || 
                ((m_trigger <= Const.ThreshGrabEnd) && (prevTrigger > Const.ThreshGrabEnd) && (m_flex <= Const.ThreshGrabEnd))) {
                GrabEnd();
            }
        }

        //==============================================================================
        private void GrabBegin () {
            float closestMagSq = float.MaxValue;
            Grabbable closestGrabbable = null;
            GrabPoint closestGrabPoint = null;

            // Iterate grab candidates and find the closest grabbable candidate
            foreach (Grabbable grabbable in m_grabCandidates.Keys) {
                // Determine if the grabbable can be grabbed
                bool canGrab = grabbable != null && !(grabbable.IsGrabbed && !grabbable.AllowOffhandGrab);
                if (!canGrab) {
                    continue;
                }

                foreach (GrabPoint grabPoint in grabbable.GrabPoints) {
                    // Store the closest grabbable
                    Vector3 closestPointOnBounds = grabPoint.GrabCollider.ClosestPointOnBounds(m_gripTransform.position);
                    float grabbableMagSq = (m_gripTransform.position - closestPointOnBounds).sqrMagnitude;
                    if (grabbableMagSq < closestMagSq) {
                        closestMagSq = grabbableMagSq;
                        closestGrabbable = grabbable;
                        closestGrabPoint = grabPoint;
                    }
                }
            }

            if (closestGrabbable != null && !closestGrabbable.m_grabMode.Equals(Grabbable.GrabMode.None))
            {
                // Disable grab volumes to prevent overlaps
                GrabVolumeEnable(false);

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

            if (closestGrabbable != null && !closestGrabbable.m_grabMode.Equals(Grabbable.GrabMode.None)) { 
                if (closestGrabbable.IsGrabbed) {
                    // Release the grabbable from the another hand
                    closestGrabbable.GrabbedHand.OffhandGrabbed(closestGrabbable);
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
        public void GrabEnd () {
            if (IsGrabbingGrabbable) {
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
                bool wasThrown = m_velocityTracker.TrackedLinearVelocity.magnitude >= Const.ThreshThrowSpeed;
                
                // Compute release velocities
                Vector3 linearVelocity = Vector3.zero;
                Vector3 angularVelocity = Vector3.zero;
                if (wasThrown) {
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

            // Re-enable grab volumes to allow overlap events
            GrabVolumeEnable(true);
        }

        //==============================================================================
        private void GrabbableAdvance (Vector3 finalPosition, Quaternion finalRotation) {
            if (!IsGrabbingGrabbable) {
                // No grabbed grabbables
                return;
            }

            // Get grabbed object's transform
            Transform grabbedTransform = m_grabbedGrabbable.GrabTransform;

            if (m_grabbedGrabbable.m_grabMode.Equals(Grabbable.GrabMode.Grab))
            {
                // Determine grab snapping
                bool snapPosition = (
                    (m_grabbedHandPose != null) &&
                    ((m_grabbedHandPose.AttachType == HandPoseAttachType.Snap) || (m_grabbedHandPose.AttachType == HandPoseAttachType.SnapPosition))
                );
                bool snapRotation = (
                    (m_grabbedHandPose != null) &&
                    (m_grabbedHandPose.AttachType == HandPoseAttachType.Snap)
                );

                Quaternion deltaRotation = finalRotation * Quaternion.Inverse(this.transform.rotation);

                Vector3 grabbablePosition = snapPosition
                    ? m_gripTransform.position
                    : finalPosition + deltaRotation * (grabbedTransform.position - this.transform.position);

                Quaternion grabbableRotation = snapRotation
                    ? m_gripTransform.rotation
                    : deltaRotation * grabbedTransform.rotation;

                grabbedTransform.position = grabbablePosition;
                grabbedTransform.rotation = grabbableRotation;
            }
        }

        //==============================================================================
        private void GrabbableGrab (Grabbable grabbable, GrabPoint grabPoint) {
            m_grabbedGrabbable = grabbable;
            m_grabbedGrabbable.GrabBegin(this, grabPoint);
            m_grabbedHandPose = m_grabbedGrabbable.HandPose;
        }

        //==============================================================================
        private void GrabbableRelease (Vector3 linearVelocity, Vector3 angularVelocity) {
            m_grabbedGrabbable.GrabEnd(linearVelocity, angularVelocity);
            m_grabbedHandPose = null;
            m_grabbedGrabbable = null;
        }

        //==============================================================================
        private void GrabVolumeAdvance () {
            m_wasGrabVolumeEnabled = m_grabVolumeEnabled;
        }

        //==============================================================================
        private void GrabVolumeEnable (bool enabled) {
            if (m_grabVolumeEnabled == enabled) {
                return;
            }

            // Set collision state
            m_grabVolumeEnabled = enabled;
            foreach (Collider grabVolume in m_grabVolumes) {
                grabVolume.enabled = m_grabVolumeEnabled;
            }

            if (!m_grabVolumeEnabled) {
                // Clear overlaps
                foreach (Grabbable grabbable in m_grabCandidates.Keys) {
                    if (grabbable != null)
                    {
                        grabbable.OverlapEnd(this);
                    }
                }
                m_grabCandidates.Clear();
            }
        }

        //==============================================================================
        public void OffhandGrabbed (Grabbable grabbable) {
            if (m_grabbedGrabbable == grabbable) {
                GrabbableRelease(Vector3.zero, Vector3.zero);
            }
        }

        //==============================================================================
        private RegistrationTransform HandRegistration () {
            RegistrationTransform handRegistration = (m_handedness == HandednessId.Left)
                ? Const.RegistrationLeft
                : Const.RegistrationRight;
            return handRegistration;
        }

    }

}
