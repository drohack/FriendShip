/********************************************************************************//**
\file      Grabbable.cs
\brief     Component that allows objects to be grabbed by the hand.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

using System;
using UnityEngine;

namespace OvrTouch.Hands {

    public struct GrabbableGrabMsg {

        //==============================================================================
        // Fields
        //==============================================================================

        public const string MsgNameGrabBegin = "OnGrabBegin";
        public const string MsgNameGrabEnd = "OnGrabEnd";

        public  Grabbable Sender;

    }

    public struct GrabbableOverlapMsg {

        //==============================================================================
        // Fields
        //==============================================================================

        public const string MsgNameOverlapBegin = "OnOverlapBegin";
        public const string MsgNameOverlapEnd = "OnOverlapEnd";

        public Grabbable Sender;
        public Hand Hand;

    }

    public class Grabbable : MonoBehaviour {

        //==============================================================================
        // Fields
        //==============================================================================

        [SerializeField] private bool m_allowOffhandGrab = true;
        [SerializeField] private GrabPoint[] m_grabPoints = null;

        private bool m_grabbedKinematic = false;
        private GrabPoint m_grabbedGrabPoint = null;
        private Hand m_grabbedHand = null;

        //==============================================================================
        // Properties
        //==============================================================================

        public bool AllowOffhandGrab {
            get { return m_allowOffhandGrab; }
        }

        public HandPose HandPose {
            get { return m_grabbedGrabPoint.HandPose; }
        }

        public bool IsGrabbed {
            get { return m_grabbedHand != null; }
        }

        public Hand GrabbedHand {
            get { return m_grabbedHand; }
        }

        public Transform GrabTransform {
            get { return m_grabbedGrabPoint.GrabTransform; }
        }

        public GrabPoint[] GrabPoints {
            get { return m_grabPoints; }
        }

        //==============================================================================
        // Public
        //==============================================================================

        //==============================================================================
        public void GrabBegin (Hand hand, GrabPoint grabPoint) {
            // Store the grabbed data
            m_grabbedHand = hand;
            m_grabbedGrabPoint = grabPoint;

            if (m_grabbedGrabPoint.Rigidbody != null) {
                // Force to kinematic state
                m_grabbedKinematic = m_grabbedGrabPoint.Rigidbody.isKinematic;
                m_grabbedGrabPoint.Rigidbody.isKinematic = true;
            }

            // Send grab begin message
            GrabbableGrabMsg grabMsg = new GrabbableGrabMsg () {
                Sender = this,
            };
            SendMsg(GrabbableGrabMsg.MsgNameGrabBegin, grabMsg);
        }

        //==============================================================================
        public void GrabEnd (Vector3 linearVelocity, Vector3 angularVelocity) {
            if (m_grabbedGrabPoint.Rigidbody != null) {
                // Restore kinematic state and apply velocities
                m_grabbedGrabPoint.Rigidbody.isKinematic = m_grabbedKinematic;
                m_grabbedGrabPoint.Rigidbody.velocity = linearVelocity;
                m_grabbedGrabPoint.Rigidbody.angularVelocity = angularVelocity;
            }

            // Send grab end message
            GrabbableGrabMsg grabMsg = new GrabbableGrabMsg () {
                Sender = this,
            };
            SendMsg(GrabbableGrabMsg.MsgNameGrabEnd, grabMsg);

            // Clear the grabbed data
            m_grabbedHand = null;
            m_grabbedGrabPoint = null;
        }

        //==============================================================================
        public void OverlapBegin (Hand hand) {
            GrabbableOverlapMsg overlapMsg = new GrabbableOverlapMsg () {
                Sender = this,
                Hand = hand,
            };
            SendMsg(GrabbableOverlapMsg.MsgNameOverlapBegin, overlapMsg);
        }

        //==============================================================================
        public void OverlapEnd (Hand hand) {
            GrabbableOverlapMsg overlapMsg = new GrabbableOverlapMsg () {
                Sender = this,
                Hand = hand,
            };
            SendMsg(GrabbableOverlapMsg.MsgNameOverlapEnd, overlapMsg);
        }

        //==============================================================================
        // MonoBehaviour
        //==============================================================================

        //==============================================================================
        private void Awake () {
            if (m_grabPoints.Length == 0) {
                // Get the collider from the grabbable
                Collider collider = this.GetComponent<Collider>();
                if (collider == null) {
                    throw new ArgumentException("Grabbable: Grabbables cannot have zero grab points and no collider -- please add a grab point or collider.");
                }

                // Create a default grab point
                m_grabPoints = new GrabPoint[1] { new GrabPoint(collider) };
            }

            foreach (GrabPoint grabPoint in m_grabPoints) {
                // Initialize the grab point
                grabPoint.Initialize();

                // Add the grab trigger and set the grabbable
                GameObject grabObject = grabPoint.GrabCollider.gameObject;
                GrabTrigger grabTrigger = grabObject.GetComponent<GrabTrigger>();
                if (grabTrigger == null) {
                    grabTrigger = grabObject.AddComponent<GrabTrigger>();
                }
                grabTrigger.SetGrabbable(this);
            }
        }

        //==============================================================================
        // Private
        //==============================================================================

        //==============================================================================
        private void SendMsg (string msgName, object msg) {
            this.transform.SendMessage(msgName, msg, SendMessageOptions.DontRequireReceiver);
        }

    }

}
