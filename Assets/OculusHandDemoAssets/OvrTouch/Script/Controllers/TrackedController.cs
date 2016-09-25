/********************************************************************************//**
\file      TrackedController.cs
\brief     Wrapper class for OVRInput to 
           perform debouncing on cap touch values and handle simple haptics.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

using UnityEngine;

namespace OVRTouchSample
{

    public class TrackedController : MonoBehaviour
    {
        public const float TRIGGER_DEBOUNCE_TIME = 0.05f;
        public const float THUMB_DEBOUNCE_TIME = 0.15f;

        static private TrackedController[] m_cachedControllers = new TrackedController[2];

        [SerializeField]
        private OVRInput.Controller m_controllerType = OVRInput.Controller.None;

        private bool m_point = false;
        private bool m_thumbsUp = false;

        private float m_lastPoint = -1.0f;
        private float m_lastNonPoint = -1.0f;
        private float m_lastThumb = -1.0f;
        private float m_lastNonThumb = -1.0f;

        static public TrackedController GetController(OVRInput.Controller controller)
        {
            return m_cachedControllers[controller == OVRInput.Controller.LTouch ? 0 : 1];
        }

        public bool IsLeft
        {
            get { return m_controllerType == OVRInput.Controller.LTouch; }
        }

        public bool IsPoint
        {
            get { return m_point; }
        }

        public bool IsThumbsUp
        {
            get { return m_thumbsUp; }
        }

        public bool Button1
        {
            get { return OVRInput.Get(OVRInput.Button.One, m_controllerType); }
        }

        public bool Button2
        {
            get { return OVRInput.Get(OVRInput.Button.Two, m_controllerType); }
        }

        public bool ButtonJoystick
        {
            get { return OVRInput.Get(OVRInput.Button.PrimaryThumbstick, m_controllerType); }
        }

        public float Trigger
        {
            get { return OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controllerType); }
        }

        public float GripTrigger
        {
            get { return OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controllerType); }
        }

        public Vector2 Joystick
        {
            get { return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_controllerType); }
        }

        private void Awake()
        {
            Debug.Assert(m_controllerType == OVRInput.Controller.LTouch || m_controllerType == OVRInput.Controller.RTouch);
            int idx = m_controllerType == OVRInput.Controller.LTouch ? 0 : 1;
            Debug.Assert(m_cachedControllers[idx] == null, "Attempted to create multiple TrackedControllers for same hand. TrackedControllers should only be one per hand.");
            m_cachedControllers[idx] = this;
        }

        private void LateUpdate()
        {
            // Cap touch
            float atT = Time.time;
            bool nowPoint = !OVRInput.Get(OVRInput.NearTouch.PrimaryIndexTrigger, m_controllerType);
            if (nowPoint)
            {
                m_lastPoint = atT;
            }
            else {
                m_lastNonPoint = atT;
            }

            bool nowThumb = !OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons, m_controllerType);
            if (nowThumb)
            {
                m_lastThumb = atT;
            }
            else {
                m_lastNonThumb = atT;
            }

            if (nowPoint != IsPoint)
            {
                // Check the hysteresis logic
                bool pointChanged = (
                    (nowPoint && (atT - m_lastNonPoint) > TRIGGER_DEBOUNCE_TIME) ||
                    (!nowPoint && (atT - m_lastPoint) > TRIGGER_DEBOUNCE_TIME)
                );
                if (pointChanged)
                {
                    m_point = nowPoint;
                }
            }

            if (nowThumb != IsThumbsUp)
            {
                // Check the hysteresis logic
                bool thumbChanged = (
                    (nowThumb && (atT - m_lastNonThumb) > THUMB_DEBOUNCE_TIME) ||
                    (!nowThumb && (atT - m_lastThumb) > THUMB_DEBOUNCE_TIME)
                );
                if (thumbChanged)
                {
                    m_thumbsUp = nowThumb;
                }
            }
        }
    }
}
