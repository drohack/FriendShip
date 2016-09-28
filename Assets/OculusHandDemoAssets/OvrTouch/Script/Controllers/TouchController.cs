/********************************************************************************//**
\file      TouchController.cs
\brief     Animating controller that updates with the tracked controller.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

using UnityEngine;

namespace OVRTouchSample
{
    public class TouchController : MonoBehaviour
    {
        [SerializeField]
        private OVRInput.Controller m_handedness;
        [SerializeField]
        private Animator m_animator = null;

        private TrackedController m_trackedController = null;

        private void Start()
        {
            m_trackedController = TrackedController.GetController(m_handedness);

        }

        private void Update()
        {
            if (m_trackedController != null)
            {
                m_animator.SetFloat("Button 1", m_trackedController.Button1 ? 1.0f : 0.0f);
                m_animator.SetFloat("Button 2", m_trackedController.Button2 ? 1.0f : 0.0f);
                Vector2 joyStick = m_trackedController.Joystick;
                m_animator.SetFloat("Joy X", joyStick.x);
                m_animator.SetFloat("Joy Y", joyStick.y);
                m_animator.SetFloat("Grip", m_trackedController.GripTrigger);
                m_animator.SetFloat("Trigger", m_trackedController.Trigger);
            }
        }
    }
}
