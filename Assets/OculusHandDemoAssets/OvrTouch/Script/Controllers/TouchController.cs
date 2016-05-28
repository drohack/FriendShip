/********************************************************************************//**
\file      TouchController.cs
\brief     Animating controller that updates with the tracked controller.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

using UnityEngine;

namespace OvrTouch.Controllers {

    public class TouchController : MonoBehaviour {

        //==============================================================================
        // Nested Types
        //==============================================================================

        private enum AnimParamId {
            Button1,
            Button2,
            Trigger,
            Grip,
            JoyX,
            JoyY,
            Count
        }

        private static class Const {

            public static readonly string[] AnimParamNames = new string[(int)AnimParamId.Count] {
                "Button 1",
                "Button 2",
                "Trigger",
                "Grip",
                "Joy X",
                "Joy Y"
            };

        }

        //==============================================================================
        // Fields
        //==============================================================================

        [SerializeField] private HandednessId m_handedness = HandednessId.Left;
        [SerializeField] private Animator m_animator = null;
        [SerializeField] private Transform m_meshRoot = null;

        private TrackedController m_trackedController = null;
        private int[] m_animParamIndices = new int[(int)AnimParamId.Count];

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
            // Get animator indices by name
            for (int i = 0; i < m_animParamIndices.Length; ++i) {
                m_animParamIndices[i] = Animator.StringToHash(Const.AnimParamNames[i]);
            }

            // Find the tracked controller
            m_trackedController = TrackedController.FindOrCreate(m_handedness);
        }

        //==============================================================================
        private void LateUpdate () {
            if (m_trackedController == null) {
                return;
            }

            // Transform
            this.transform.position = m_trackedController.transform.position;
            this.transform.rotation = m_trackedController.transform.rotation;

            // Buttons
            float button1 = m_trackedController.Button1 ? 1.0f : 0.0f;
            SetAnimParam(AnimParamId.Button1, button1);

            float button2 = m_trackedController.Button2 ? 1.0f : 0.0f;
            SetAnimParam(AnimParamId.Button2, button2);

            // Joystick
            Vector2 joyStick = m_trackedController.Joystick;
            SetAnimParam(AnimParamId.JoyX, joyStick.x);
            SetAnimParam(AnimParamId.JoyY, joyStick.y);

            // Triggers
            SetAnimParam(AnimParamId.Grip, m_trackedController.GripTrigger);
            SetAnimParam(AnimParamId.Trigger, m_trackedController.Trigger);
        }

        //==============================================================================
        // Private
        //==============================================================================

        //==============================================================================
        private void SetAnimParam (AnimParamId paramId, float paramValue) {
            m_animator.SetFloat(m_animParamIndices[(int)paramId], paramValue);
        }

    }

}
