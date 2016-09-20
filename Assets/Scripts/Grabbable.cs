using System;
using UnityEngine;
using System.Collections;

namespace OvrTouch.Hands
{
    public struct GrabbableGrabMsg
    {

        //==============================================================================
        // Fields
        //==============================================================================

        public const string MsgNameGrabBegin = "OnGrabBegin";
        public const string MsgNameGrabEnd = "OnGrabEnd";

        public Grabbable Sender;

    }

    public struct GrabbableOverlapMsg
    {

        //==============================================================================
        // Fields
        //==============================================================================

        public const string MsgNameOverlapBegin = "OnOverlapBegin";
        public const string MsgNameOverlapEnd = "OnOverlapEnd";

        public Grabbable Sender;
        public Hand Hand;

    }

    public class Grabbable : MonoBehaviour
    {

        //==============================================================================
        // Nested Types
        //==============================================================================

        public enum GrabMode
        {
            Grab,
            Drag,
            Rotate,
            None
        }

        public bool isGrabbing = false;

        //Drag Spring variables
        const float d_Spring = 200.0f;
        const float d_Damper = 1.0f;
        const float d_Drag = 10.0f;
        const float d_AngularDrag = 5.0f;
        const float d_BreakForce = 25f;
        //Spring variables
        const float k_Spring = 0.1f;
        const float k_Damper = 0f;
        const float k_Drag = 0f;
        const float k_AngularDrag = 0f;
        const float k_Distance = 0.001f;
        const float k_BreakForce = 0.01f;
        const float k_Tolerance = 0.01f;
        const bool k_AttachToCenterOfMass = false;
        private SpringJoint m_SpringJoint;
        private GameObject rigidbodyDragger;

        //Grabbable variables
        [SerializeField]
        public GrabMode m_grabMode = GrabMode.Grab;
        [SerializeField]
        private bool m_allowOffhandGrab = true;
        [SerializeField]
        private GrabPoint[] m_grabPoints = null;
        private bool m_grabbedKinematic = false;
        private GrabPoint m_grabbedGrabPoint = null;
        private Hand m_grabbedHand = null;

        //==============================================================================
        // Properties
        //==============================================================================

        public bool AllowOffhandGrab
        {
            get { return m_allowOffhandGrab; }
        }

        public HandPose HandPose
        {
            get { return m_grabbedGrabPoint.HandPose; }
        }

        public bool IsGrabbed
        {
            get { return m_grabbedHand != null; }
        }

        public Hand GrabbedHand
        {
            get { return m_grabbedHand; }
        }

        public Transform GrabTransform
        {
            get { return m_grabbedGrabPoint.GrabTransform; }
        }

        public GrabPoint[] GrabPoints
        {
            get { return m_grabPoints; }
        }

        private void Start()
        {
            isGrabbing = false;
        }

        public void GrabBegin(Hand hand, GrabPoint grabPoint)
        {
            isGrabbing = true;

            // Store the grabbed data
            m_grabbedHand = hand;
            m_grabbedGrabPoint = grabPoint;

            // Add Spring
            if (!rigidbodyDragger)
            {
                rigidbodyDragger = new GameObject("Rigidbody dragger");
                rigidbodyDragger.transform.parent = transform;
                Rigidbody body = rigidbodyDragger.AddComponent<Rigidbody>();
                body.useGravity = false;
                body.isKinematic = true;
            }
            if (!m_SpringJoint)
            {
                m_SpringJoint = rigidbodyDragger.AddComponent<SpringJoint>();
            }

            m_SpringJoint.transform.position = m_grabbedHand.transform.position;
            m_SpringJoint.anchor = Vector3.zero;
            m_SpringJoint.maxDistance = k_Distance;
            m_SpringJoint.connectedBody = transform.GetComponent<Rigidbody>();

            if (m_grabMode.Equals(GrabMode.Drag))
            {
                m_SpringJoint.spring = d_Spring;
                m_SpringJoint.damper = d_Damper;
                m_SpringJoint.breakForce = d_BreakForce;
                m_SpringJoint.connectedBody.drag = d_Drag;
                m_SpringJoint.connectedBody.angularDrag = d_AngularDrag;
            }
            else if (!m_grabMode.Equals(GrabMode.None))
            {
                m_SpringJoint.spring = k_Spring;
                m_SpringJoint.damper = k_Damper;
                m_SpringJoint.breakForce = k_BreakForce;
                m_SpringJoint.tolerance = k_Tolerance;
                m_SpringJoint.connectedBody.drag = k_Drag;
                m_SpringJoint.connectedBody.angularDrag = k_AngularDrag;

            }

            if (m_grabMode.Equals(GrabMode.Grab) && m_grabbedGrabPoint.Rigidbody != null)
            {
                // Force to kinematic state
                m_grabbedKinematic = m_grabbedGrabPoint.Rigidbody.isKinematic;
                m_grabbedGrabPoint.Rigidbody.isKinematic = true;
            }

            // Send grab begin message
            GrabbableGrabMsg grabMsg = new GrabbableGrabMsg()
            {
                Sender = this,
            };
            SendMsg(GrabbableGrabMsg.MsgNameGrabBegin, grabMsg);
        }
        public void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
        {
            // Get rid of the spring joint
            if (m_SpringJoint != null)
                Destroy(m_SpringJoint);

            // Keep the object's velocity and angular velocity
            if (m_grabbedGrabPoint.Rigidbody != null)
            {
                m_grabbedGrabPoint.Rigidbody.isKinematic = m_grabbedKinematic;
                m_grabbedGrabPoint.Rigidbody.velocity = linearVelocity;
                m_grabbedGrabPoint.Rigidbody.angularVelocity = angularVelocity;
            }

            // Send grab end message
            GrabbableGrabMsg grabMsg = new GrabbableGrabMsg()
            {
                Sender = this,
            };
            SendMsg(GrabbableGrabMsg.MsgNameGrabEnd, grabMsg);

            // Clear the grabbed data
            m_grabbedHand = null;
            m_grabbedGrabPoint = null;
            isGrabbing = false;
        }

        void Update()
        {
            // If grabbed call GrabEnd() if the spring breaks
            if (isGrabbing && m_SpringJoint == null)
            {
                m_grabbedHand.GrabEnd();
            }
        }

        private void LateUpdate()
        {
            // If grabbed update the springjoint's transform
            if (isGrabbing && m_SpringJoint != null)
            {
                m_SpringJoint.transform.position = m_grabbedHand.transform.position;
            }
        }

        private void OnDestroy()
        {
            // If this object is destroyed while being grabbed end the grab animation of the hand
            if (isGrabbing)
            {
                m_grabbedHand.GrabEnd();
            }
        }

        //==============================================================================
        public void OverlapBegin(Hand hand)
        {
            if (this != null)
            {
                GrabbableOverlapMsg overlapMsg = new GrabbableOverlapMsg()
                {
                    Sender = this,
                    Hand = hand,
                };
                SendMsg(GrabbableOverlapMsg.MsgNameOverlapBegin, overlapMsg);
            }
        }

        //==============================================================================
        public void OverlapEnd(Hand hand)
        {
            GrabbableOverlapMsg overlapMsg = new GrabbableOverlapMsg()
            {
                Sender = this != null ? this : null,
                Hand = hand,
            };
            SendMsg(GrabbableOverlapMsg.MsgNameOverlapEnd, overlapMsg);
        }

        //==============================================================================
        private void Awake()
        {
            if (m_grabPoints.Length == 0)
            {
                // Get the collider from the grabbable
                Collider collider = this.GetComponent<Collider>();
                if (collider == null)
                {
                    throw new ArgumentException("Grabbable: Grabbables cannot have zero grab points and no collider -- please add a grab point or collider.");
                }

                // Create a default grab point
                m_grabPoints = new GrabPoint[1] { new GrabPoint(collider) };
            }

            foreach (GrabPoint grabPoint in m_grabPoints)
            {
                // Initialize the grab point
                grabPoint.Initialize();

                // Add the grab trigger and set the grabbable
                GameObject grabObject = grabPoint.GrabCollider.gameObject;
                GrabTrigger grabTrigger = grabObject.GetComponent<GrabTrigger>();
                if (grabTrigger == null)
                {
                    grabTrigger = grabObject.AddComponent<GrabTrigger>();
                }
                grabTrigger.SetGrabbable(this);
            }

            // Only allow offhand grab in if GrabMode is "Grab"
            if (!m_grabMode.Equals(GrabMode.Grab))
            {
                m_allowOffhandGrab = false;
            }
        }

        //==============================================================================
        private void SendMsg(string msgName, object msg)
        {
            this.transform.SendMessage(msgName, msg, SendMessageOptions.DontRequireReceiver);
        }
    }
}
