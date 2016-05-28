/********************************************************************************//**
\file      TrackedController.cs
\brief     Wrapper class for OVRInput to cleanly deal with a fluctuating API,
           perform debouncing on cap touch values and handle simple haptics.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

using UnityEngine;

namespace OvrTouch.Controllers {

    public enum HandednessId {
        Left,
        Right,
    }

    public class TrackedController : MonoBehaviour {

        //==============================================================================
        // Nested Types
        //==============================================================================

         private static class Const {

            public const float TriggerDebounceTime = 0.05f;
            public const float ThumbDebounceTime = 0.15f;

         }

        //==============================================================================
        // Fields
        //==============================================================================

        [SerializeField] private HandednessId m_handedness = HandednessId.Left;
        [SerializeField] private Transform m_trackedTransform = null;

        private bool m_initialized = false;
        private OVRInput.Controller m_controllerType = OVRInput.Controller.None;

        private bool m_point = false;
        private bool m_thumbsUp = false;

        private float m_lastPoint = -1.0f;
        private float m_lastNonPoint = -1.0f;
        private float m_lastThumb = -1.0f;
        private float m_lastNonThumb = -1.0f;

        private float m_hapticDuration = -1.0f;
        private float m_hapticStartTime = -1.0f;

        //==============================================================================
        // Public
        //==============================================================================

        //==============================================================================
        public static TrackedController FindOrCreate (HandednessId handedness) {
            // Attempt to find an existing tracked controller
            TrackedController[] trackedControllers = GameObject.FindObjectsOfType<TrackedController>();
            foreach (TrackedController trackedContrller in trackedControllers) {
                if (trackedContrller.Handedness == handedness) {
                    return trackedContrller;
                }
            }
            
            // Create
            GameObject trackedControllerObject = new GameObject("TrackedController");
            TrackedController trackedController = trackedControllerObject.AddComponent<TrackedController>();

            // Attempt to find the tracked transform
            Transform trackedTransform = null;
            OVRCameraRig cameraRig = GameObject.FindObjectOfType<OVRCameraRig>();
            if (cameraRig != null) {
                trackedTransform = (handedness == HandednessId.Left)
                    ? cameraRig.leftHandAnchor
                    : cameraRig.rightHandAnchor;
            }

            // Initialize
            trackedController.Initialize(handedness, trackedTransform);
            return trackedController;
        }

        //==============================================================================
        public void PlayHapticEvent (float frequency, float amplitude, float duration) {
            m_hapticStartTime = Time.time;
            m_hapticDuration = duration;
            OVRInput.SetControllerVibration(frequency, amplitude, m_controllerType);
        }

        //==============================================================================
        // Properties
        //==============================================================================

        public HandednessId Handedness {
            get { return m_handedness; }
        }

        public bool IsLeft {
            get { return m_handedness == HandednessId.Left; }
        }

        public bool IsPoint {
            get { return m_point; }
        }

        public bool IsThumbsUp {
            get { return m_thumbsUp; }
        }

        public bool Button1 {
            get { return OVRInput.Get(OVRInput.Button.One, m_controllerType); }
        }

        public bool Button2 {
            get { return OVRInput.Get(OVRInput.Button.Two, m_controllerType); }
        }

        public bool ButtonJoystick {
            get { return OVRInput.Get(OVRInput.Button.PrimaryThumbstick, m_controllerType); }
        }

        public float Trigger {
            get { return OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controllerType); }
        }

        public float GripTrigger {
            get { return OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controllerType); }
        }

        public Vector2 Joystick {
            get { return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_controllerType); }
        }

        //==============================================================================
        // MonoBehaviour
        //==============================================================================

        //==============================================================================
        private void Awake () {
            if (m_trackedTransform != null) {
                Initialize(m_handedness, m_trackedTransform);
            }
        }

        //==============================================================================
        private void LateUpdate () {
            if (m_trackedTransform != null) {
                // Transform
                this.transform.position = m_trackedTransform.position;
                this.transform.rotation = m_trackedTransform.rotation;
            }

            // Haptics
            float elapsed = Time.time - m_hapticStartTime;
            if (elapsed >= m_hapticDuration) {
                OVRInput.SetControllerVibration(0.0f, 0.0f, m_controllerType);
            }

            // Cap touch
            float atT = Time.time;
            bool nowPoint = !OVRInput.Get(OVRInput.NearTouch.PrimaryIndexTrigger, m_controllerType);
            if (nowPoint) {
                m_lastPoint = atT;
            }
            else {
                m_lastNonPoint = atT;
            }

            bool nowThumb = !OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons, m_controllerType);
            if (nowThumb) {
                m_lastThumb = atT;
            }
            else {
                m_lastNonThumb = atT;
            }

            if (nowPoint != IsPoint) {
                // Check the hysteresis logic
                bool pointChanged = (
                    (nowPoint  && (atT - m_lastNonPoint) > Const.TriggerDebounceTime) ||
                    (!nowPoint && (atT - m_lastPoint) > Const.TriggerDebounceTime)
                );
                if (pointChanged) {
                    m_point = nowPoint;
                }
            }

            if (nowThumb != IsThumbsUp) {
                // Check the hysteresis logic
                bool thumbChanged = (
                    (nowThumb  && (atT - m_lastNonThumb) > Const.ThumbDebounceTime) ||
                    (!nowThumb && (atT - m_lastThumb) > Const.ThumbDebounceTime)
                );
                if (thumbChanged) {
                    m_thumbsUp = nowThumb;
                }
            }
        }

        //==============================================================================
        // Private
        //==============================================================================

        //==============================================================================
        private void Initialize (HandednessId handedness, Transform trackedTransform) {
            if (m_initialized) {
                return;
            }

            m_handedness = handedness;
            m_controllerType = (m_handedness == HandednessId.Left) ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;

            if (trackedTransform != null) {
                m_trackedTransform = trackedTransform;
                this.transform.position = m_trackedTransform.position;
                this.transform.rotation = m_trackedTransform.rotation;
            }

            m_initialized = true;
        }

    }

}
