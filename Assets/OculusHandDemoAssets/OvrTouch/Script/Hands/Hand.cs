/********************************************************************************//**
\file      Hand.cs
\brief     Basic hand impementation.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OVRTouchSample;

namespace OVRTouchSample
{

    [RequireComponent(typeof(ParticleSystem))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(VelocityTracker))]
    public class Hand : MonoBehaviour
    {
        public const string ANIM_LAYER_NAME_POINT = "Point Layer";
        public const string ANIM_LAYER_NAME_THUMB = "Thumb Layer";
        public const string ANIM_PARAM_NAME_FLEX = "Flex";
        public const string ANIM_PARAM_NAME_POSE = "Pose";

        public const int HMD_REFRESH_RATE = 90;

        public const float INPUT_RATE_CHANGE = 20.0f;

        public const float THRESH_COLLISION_FLEX = 0.9f;
        public const float THRESH_GRAB_BEGIN = 0.55f;
        public const float THRESH_GRAB_END = 0.35f;
        public const float THRESH_THROW_SPEED = 1.0f;

        public const float COLLIDER_SCALE_MIN = 0.01f;
        public const float COLLIDER_SCALE_MAX = 1.0f;
        public const float COLLIDER_SCALE_PER_SECOND = 1.0f;

        // NOTE: I attempted a FixedJoint option, attaching the held object by adding a FixedJoint object to the
        // hand and connecting the held object to it. However, since the Hand is allowed inside the geometry and the
        // non-kinematic held object was not, this simply led to serious physics freak-out when the hand moved inside 
        // the table, unless we used a trivially breakable break force. A more elaborate solution is required for
        // a satisfactory non-geo-penetrating solution.
        // When this is false, instead of using a FixedJoint, simply parents the held object to the hand.
        [SerializeField]
        private bool m_useFixedJointForGrabbedObject = false;

        // MTF EXPERIMENT:
        // ed if there's any difference between parenting the held object on the hand (the hand uses MovePosition)
        // or calling MovePosition on the held object. Parenting wrecks physics behavior, e.g. no friction etc.
        [SerializeField]
        private bool m_parentHeldObject = false;

        [SerializeField]
        public GameObject m_touchAnchor = null;
        
        [SerializeField]
        private OVRInput.Controller m_handedness;
        [SerializeField]
        private Transform m_gripTransform = null;
        [SerializeField]
        private Animator m_animator = null;
        [SerializeField]
        private float m_particleEmissionRateVelocityScale = 50.0f;
        [SerializeField]
        private Collider[] m_grabVolumes = null;

        private ParticleSystem m_particles;
        private TrackedController m_trackedController = null;
        private VelocityTracker m_velocityTracker = null;
        private Collider[] m_colliders = null;
        private bool m_collisionEnabled = true;
        private bool m_grabVolumeEnabled = true;
        private Vector3 m_lastPos;
        private Quaternion m_lastRot;
        private Quaternion m_anchorOffsetRotation;
        private Vector3 m_anchorOffsetPosition;

        private int m_animLayerIndexThumb = -1;
        private int m_animLayerIndexPoint = -1;
        private int m_animParamIndexFlex = -1;
        private int m_animParamIndexPose = -1;

        private Grabbable m_grabbedObj = null;
        private Dictionary<Grabbable, int> m_grabCandidates = new Dictionary<Grabbable, int>();
        private bool m_handVisible = true;

        // EDITED FIELDS
        [SerializeField]
        public HandednessId m_handednessId;
        [SerializeField]
        private Hand_Pose m_defaultGrabPose;
        public HandPoseId handPoseId;
        public Hand_Pose m_grabbedHandPose = null;
        public float m_flex = 0.0f;
        public float m_trigger = 0.0f;
        public float m_point = 0.0f;
        public float m_thumbsUp = 0.0f;
        public bool canPoint;
        public bool canThumbsUp;
        private Grabbed_Hand_Script grabbedHandScript;

        // HAPTICS
        public const float HAPTIC_OVERLAP_AMPLITUDE = 0.25f;
        public const float HAPTIC_OVERLAP_FREQUENCY = 320.0f;
        public const float HAPTIC_OVERLAP_DURATION = 0.05f;
        private OVRHaptics.OVRHapticsChannel m_hapticsChannel = null;
        public AudioClip m_clipA;
        private OVRHapticsClip m_hapticsClipA;

        public bool HandVisible
        {
            set
            {
                m_handVisible = value;
                // MTF HACK: setting the animator itself to inactive causes "stuck anim" bugs on reactivation.
                // Need to investigate further, but for now this works fine.
                int numChildren = m_animator.gameObject.transform.childCount;
                for(int i=0; i<numChildren; ++i)
                {
                    m_animator.gameObject.transform.GetChild(i).gameObject.SetActive(value);
                }
                //m_animator.gameObject.SetActive(value);
            }
            get { return m_handVisible; }
        }

        private Hand_Pose GrabPose
        {
            get { return m_grabbedHandPose ?? m_defaultGrabPose; }
        }

        public OVRInput.Controller Handedness { get { return m_handedness; } }

        public void ForceRelease(Grabbable grabbable)
        {
            bool canRelease = (
                (m_grabbedObj != null) &&
                (m_grabbedObj == grabbable)
            );
            if (canRelease)
            {
                if (grabbedHandScript != null)
                    grabbedHandScript.GrabEnd();
            }
        }
        private void Awake()
        {
            m_anchorOffsetPosition = transform.localPosition;
            m_anchorOffsetRotation = transform.localRotation;
        }

        private void Start()
        {
            m_particles = GetComponent<ParticleSystem>();

            // Collision starts disabled. We'll enable it for certain cases such as making a fist.
            m_colliders = this.GetComponentsInChildren<Collider>().Where(childCollider => !childCollider.isTrigger).ToArray();
            CollisionEnable(false);

            m_velocityTracker = this.GetComponent<VelocityTracker>();

            // Get animator layer indices by name, for later use switching between hand visuals
            m_animLayerIndexPoint = m_animator.GetLayerIndex(ANIM_LAYER_NAME_POINT);
            m_animLayerIndexThumb = m_animator.GetLayerIndex(ANIM_LAYER_NAME_THUMB);
            m_animParamIndexFlex = Animator.StringToHash(ANIM_PARAM_NAME_FLEX);
            m_animParamIndexPose = Animator.StringToHash(ANIM_PARAM_NAME_POSE);

            m_trackedController = TrackedController.GetController(m_handedness);
            m_lastPos = transform.position;
            m_lastRot = transform.rotation;

            // Fixed timestep error checking
            float fps = Mathf.CeilToInt(1 / Time.fixedDeltaTime);
            if (fps < HMD_REFRESH_RATE)
            {
                Debug.LogError("Hand: Hands should use a fixed timestep that is at least the refresh rate of the hmd (90hz) for responsive physics behavior.");
            }

            // Haptics
            m_hapticsChannel = m_handedness == OVRInput.Controller.LTouch ? OVRHaptics.LeftChannel : OVRHaptics.RightChannel;
            if (m_clipA != null) m_hapticsClipA = new OVRHapticsClip(m_clipA);

            grabbedHandScript = this.GetComponent<Grabbed_Hand_Script>();
        }

        private float interval = 0.1f;
        private float nextUpdate = 0f;
        private void Update()
        {
            if (Time.time >= nextUpdate)
            {
                ParticleSystem.EmissionModule emission = m_particles.emission;
                var rateCurve = emission.rate;
                rateCurve.constantMax = m_velocityTracker.TrackedLinearVelocity.magnitude * m_particleEmissionRateVelocityScale;
                emission.rate = rateCurve;
            }

            float prevFlex = m_flex;
            float prevTrigger = m_trigger;

            // Update values from inputs
            m_flex = m_trackedController.GripTrigger;
            m_trigger = m_trackedController.Trigger;
            m_point = InputValueRateChange(m_trackedController.IsPoint, m_point);
            m_thumbsUp = InputValueRateChange(m_trackedController.IsThumbsUp, m_thumbsUp);

            CheckForGrabOrRelease(prevFlex, prevTrigger);

            bool collisionEnabled = (
                m_grabbedObj != null ||
                (m_flex >= THRESH_COLLISION_FLEX)
            );
            CollisionEnable(collisionEnabled);

            if (m_handVisible) UpdateAnimStates();
        }

        // Hands follow the touch anchors by calling MovePosition each frame to reach the anchor.
        // This is done instead of parenting to achieve workable physics.
        //
        // NOTE: touch controller positions are updated once per frame, before Unity runs any scripts. So the
        // position you'll get for hand anchors in this function will be the same as the ones you get in Update
        // and LateUpdate, for everything processed in the same frame.
        // 
        // BUG: currently (5/24/2016, Unity 5.3.3p3, Unity 5.4.0B18.), there's an unavoidable cosmetic issue with
        // the hand. FixedUpdate must be used, or else physics behavior is wildly erratic.
        // However, FixedUpdate cannot be guaranteed to run every frame, even when at 90Hz.
        // On frames where FixedUpdate fails to run, the hand will fail to update its position, causing apparent
        // judder. A fix is in progress, but not fixable on the user side at this time.
        private void FixedUpdate()
        {
            if(m_touchAnchor != null)
            {
                GetComponent<Rigidbody>().MovePosition(m_anchorOffsetPosition + m_touchAnchor.transform.position);
                GetComponent<Rigidbody>().MoveRotation(m_touchAnchor.transform.rotation * m_anchorOffsetRotation);
            }
            //if (!m_useFixedJointForGrabbedObject && !m_parentHeldObject)
            //{
            //    MoveGrabbedObject();
            //}
            m_lastPos = transform.position;
            m_lastRot = transform.rotation;
        }

        private void LateUpdate()
        {
            // Hand's collision grows over a short amount of time on enable, rather than snapping to on, to help somewhat with interpenetration issues.
            if (m_collisionEnabled && m_collisionScaleCurrent + Mathf.Epsilon < COLLIDER_SCALE_MAX)
            {
                m_collisionScaleCurrent = Mathf.Min(COLLIDER_SCALE_MAX, m_collisionScaleCurrent + Time.deltaTime * COLLIDER_SCALE_PER_SECOND);
                for (int i = 0; i < m_colliders.Length; ++i)
                {
                    Collider collider = m_colliders[i];
                    collider.transform.localScale = new Vector3(m_collisionScaleCurrent, m_collisionScaleCurrent, m_collisionScaleCurrent);
                }
            }
        }

        private void OnDestroy()
        {
            if (m_grabbedObj != null)
            {
                if (grabbedHandScript != null)
                    grabbedHandScript.GrabEnd();
            }
        }

        private void OnTriggerEnter(Collider otherCollider)
        {
            // Get the grab trigger
            Grabbable grabbable = otherCollider.GetComponent<Grabbable>() ?? otherCollider.GetComponentInParent<Grabbable>();
            if (grabbable == null) return;

            // Add the grabbable
            int refCount = 0;
            m_grabCandidates.TryGetValue(grabbable, out refCount);
            m_grabCandidates[grabbable] = refCount + 1;

            if (refCount == 0)
            {
                // Overlap begin
                grabbable.OverlapBegin(this);

                if (m_grabVolumeEnabled && m_trackedController != null)
                {
                    // Only play overlap haptics when there was no initial overlap (like after a grab release)
                    //m_trackedController.PlayHapticEvent(
                    //    HAPTIC_OVERLAP_FREQUENCY,
                    //    HAPTIC_OVERLAP_AMPLITUDE,
                    //    HAPTIC_OVERLAP_DURATION
                    //);
                    m_hapticsChannel.Queue(m_hapticsClipA);
                }
            }
        }

        private void OnTriggerExit(Collider otherCollider)
        {
            Grabbable grabbable = otherCollider.GetComponent<Grabbable>() ?? otherCollider.GetComponentInParent<Grabbable>();
            if (grabbable == null) return;

            // Remove the grabbable
            int refCount = 0;
            bool found = m_grabCandidates.TryGetValue(grabbable, out refCount);
            if (!found)
            {
                return;
            }

            if (refCount > 1)
            {
                m_grabCandidates[grabbable] = refCount - 1;
            }
            else
            {
                m_grabCandidates.Remove(grabbable);
            }
        }

        private float InputValueRateChange(bool isDown, float value)
        {
            float rateDelta = Time.deltaTime * INPUT_RATE_CHANGE;
            float sign = isDown ? 1.0f : -1.0f;
            return Mathf.Clamp01(value + rateDelta * sign);
        }

        private void UpdateAnimStates()
        {
            // Pose
            handPoseId = GrabPose.PoseId;
            m_animator.SetInteger(m_animParamIndexPose, (int)handPoseId);

            // Flex
            // blend between open hand and fully closed fist
            m_animator.SetFloat(m_animParamIndexFlex, m_flex);

            // Point
            canPoint = m_grabbedObj == null || GrabPose.AllowPointing;
            float point = canPoint ? m_point : 0.0f;
            m_animator.SetLayerWeight(m_animLayerIndexPoint, point);

            // Thumbs up
            canThumbsUp = m_grabbedObj != null || GrabPose.AllowThumbsUp;
            float thumbsUp = canThumbsUp ? m_thumbsUp : 0.0f;
            m_animator.SetLayerWeight(m_animLayerIndexThumb, thumbsUp);
        }

        private float m_collisionScaleCurrent = 0.0f;

        private void CollisionEnable(bool enabled)
        {
            if (m_collisionEnabled == enabled)
            {
                return;
            }
            m_collisionEnabled = enabled;

            if (enabled)
            {
                m_collisionScaleCurrent = COLLIDER_SCALE_MIN;
                for (int i = 0; i < m_colliders.Length; ++i)
                {
                    Collider collider = m_colliders[i];
                    collider.transform.localScale = new Vector3(COLLIDER_SCALE_MIN, COLLIDER_SCALE_MIN, COLLIDER_SCALE_MIN);
                    collider.enabled = true;
                }
            }
            else
            {
                m_collisionScaleCurrent = COLLIDER_SCALE_MAX;
                for (int i = 0; i < m_colliders.Length; ++i)
                {
                    Collider collider = m_colliders[i];
                    collider.enabled = false;
                    collider.transform.localScale = new Vector3(COLLIDER_SCALE_MIN, COLLIDER_SCALE_MIN, COLLIDER_SCALE_MIN);
                }
            }
        }

        private void CheckForGrabOrRelease(float prevFlex, float prevTrigger)
        {
            if (((m_flex >= THRESH_GRAB_BEGIN) && (prevFlex < THRESH_GRAB_BEGIN)) || ((m_trigger >= THRESH_GRAB_BEGIN) && (prevTrigger < THRESH_GRAB_BEGIN)))
            {
                if (grabbedHandScript != null)
                    grabbedHandScript.GrabBegin(m_grabCandidates);
            }
            else if (((m_flex <= THRESH_GRAB_END) && (prevFlex > THRESH_GRAB_END) && (m_trigger <= THRESH_GRAB_END)) ||
                ((m_trigger <= THRESH_GRAB_END) && (prevTrigger > THRESH_GRAB_END) && (m_flex <= THRESH_GRAB_END)))
            {
                if (grabbedHandScript != null)
                    grabbedHandScript.GrabEnd();
            }
        }

        //private void GrabBegin()
        //{
        //    float closestMagSq = float.MaxValue;
        //    Grabbable closestGrabbable = null;
        //    Collider closestGrabbableCollider = null;

        //    // Iterate grab candidates and find the closest grabbable candidate
        //    foreach (Grabbable grabbable in m_grabCandidates.Keys)
        //    {
        //        bool canGrab = !(grabbable.IsGrabbed && !grabbable.AllowOffhandGrab);
        //        if (!canGrab)
        //        {
        //            continue;
        //        }

        //        for (int j = 0; j < grabbable.GrabPoints.Length; ++j)
        //        {
        //            Collider grabbableCollider = grabbable.GrabPoints[j];
        //            // Store the closest grabbable
        //            Vector3 closestPointOnBounds = grabbableCollider.ClosestPointOnBounds(m_gripTransform.position);
        //            float grabbableMagSq = (m_gripTransform.position - closestPointOnBounds).sqrMagnitude;
        //            if (grabbableMagSq < closestMagSq)
        //            {
        //                closestMagSq = grabbableMagSq;
        //                closestGrabbable = grabbable;
        //                closestGrabbableCollider = grabbableCollider;
        //            }
        //        }
        //    }

        //    // Disable grab volumes to prevent overlaps
        //    GrabVolumeEnable(false);

        //    if (closestGrabbable != null)
        //    {
        //        if (closestGrabbable.IsGrabbed)
        //        {
        //            closestGrabbable.GrabbedHand.OffhandGrabbed(closestGrabbable);
        //        }

        //        m_grabbedObj = closestGrabbable;
        //        m_grabbedObj.GrabBegin(this, closestGrabbableCollider);
        //        m_grabbedHandPose = m_grabbedObj.HandPose;

        //        if(m_useFixedJointForGrabbedObject)
        //        {
        //            FixedJoint fj = gameObject.GetComponent<FixedJoint>() ?? gameObject.AddComponent<FixedJoint>();
        //            fj.connectedBody = m_grabbedObj.GetComponent<Rigidbody>();
        //        }
        //        else
        //        {
        //            // Teleport on grab, to avoid high-speed travel to dest which hits a lot of other objects at high
        //            // speed and sends them flying. The grabbed object may still teleport inside of other objects, but fixing that
        //            // is beyond the scope of this demo.
        //            m_lastPos = transform.position;
        //            m_lastRot = transform.rotation;
        //            MoveGrabbedObject(true);
        //            if(m_parentHeldObject)
        //            {
        //                m_grabbedObj.transform.parent = transform;
        //            }
        //        }
        //    }
        //}

        //private void MoveGrabbedObject(bool forceTeleport = false)
        //{
        //    if (m_grabbedObj == null)
        //    {
        //        return;
        //    }

        //    Vector3 handInitialPosition = m_lastPos;
        //    Quaternion handInitialRotation = m_lastRot;
        //    Vector3 handFinalPosition = transform.position;
        //    Quaternion handFinalRotation = transform.rotation;
        //    Quaternion handDeltaRotation = handFinalRotation * Quaternion.Inverse(handInitialRotation);

        //    bool snapPosition = GrabPose.AttachType == HandPoseAttachType.Snap || GrabPose.AttachType == HandPoseAttachType.SnapPosition;
        //    bool snapRotation = GrabPose.AttachType == HandPoseAttachType.Snap;

        //    Rigidbody grabbedRigidbody = m_grabbedObj.GrabbedRigidbody;
        //    Transform grabbedTransform = grabbedRigidbody.transform;
        //    // snap uses:   gripTransform.position, transform.position, m_lastpos
        //    // nosnap uses: m_lastPos, transform.position, grabbedTransform.position
        //    // is grabbedTransform.position giving us late data? this is in fixedUpdate
        //    Vector3 grabbablePosition = snapPosition ?
        //        m_gripTransform.position + handDeltaRotation * (handFinalPosition - handInitialPosition) : 
        //        handFinalPosition + handDeltaRotation * (grabbedTransform.position - handInitialPosition);
        //    Quaternion grabbableRotation = snapRotation ? 
        //        handDeltaRotation * m_gripTransform.rotation :
        //        handDeltaRotation * grabbedTransform.rotation;

        //    if (forceTeleport)
        //    {
        //        grabbedRigidbody.transform.position = grabbablePosition;
        //        grabbedRigidbody.transform.rotation = grabbableRotation;
        //    }
        //    else
        //    {
        //        grabbedRigidbody.MovePosition(grabbablePosition);
        //        grabbedRigidbody.MoveRotation(grabbableRotation);
        //    }
        //}

        //private void GrabEnd()
        //{
        //    if (m_grabbedObj != null)
        //    {
        //        // Determine if the grabbable was thrown, compute appropriate velocities.
        //        bool wasThrown = m_velocityTracker.TrackedLinearVelocity.magnitude >= THRESH_THROW_SPEED;
        //        Vector3 linearVelocity = Vector3.zero;
        //        Vector3 angularVelocity = Vector3.zero;
        //        if (wasThrown)
        //        {
        //            // Throw velocity
        //            linearVelocity = m_velocityTracker.TrackedLinearVelocity;
        //            angularVelocity = m_velocityTracker.TrackedAngularVelocity;
        //        }
        //        else
        //        {
        //            // Drop velocity
        //            linearVelocity = m_velocityTracker.FrameLinearVelocity;
        //            angularVelocity = m_velocityTracker.FrameAngularVelocity;
        //        }

        //        GrabbableRelease(linearVelocity, angularVelocity);
        //    }

        //    // Re-enable grab volumes to allow overlap events
        //    GrabVolumeEnable(true);
        //}

        private void GrabbableRelease(Vector3 linearVelocity, Vector3 angularVelocity)
        {
            Destroy(gameObject.GetComponent<FixedJoint>());
            //m_grabbedObj.transform.parent = null;
            m_grabbedObj.GrabEnd(linearVelocity, angularVelocity, false);
            m_grabbedHandPose = null;
            if(m_parentHeldObject) m_grabbedObj.transform.parent = null;
            m_grabbedObj = null;
        }

        public void GrabVolumeEnable(bool enabled)
        {
            if (m_grabVolumeEnabled == enabled)
            {
                return;
            }

            m_grabVolumeEnabled = enabled;
            for (int i = 0; i < m_grabVolumes.Length; ++i)
            {
                Collider grabVolume = m_grabVolumes[i];
                grabVolume.enabled = m_grabVolumeEnabled;
            }

            if (!m_grabVolumeEnabled)
            {
                m_grabCandidates.Clear();
            }
        }

        private void OffhandGrabbed(Grabbable grabbable)
        {
            if (m_grabbedObj == grabbable)
            {
                GrabbableRelease(Vector3.zero, Vector3.zero);
            }
        }
    }
}
