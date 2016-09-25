/********************************************************************************//**
\file      TouchVisualizer.cs
\brief     Toggle visibility for hands and controllers.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

using UnityEngine;
using OVRTouchSample;

namespace OVRTouchSample {

    public class TouchVisualizer : MonoBehaviour {
        private enum DisplayMode {
            Hand,
            Controller,
            HandAndController,
            Count,
        }

        [SerializeField] private DisplayMode m_displayMode = DisplayMode.Controller;
        [SerializeField] private Hand m_hand = null;
        [SerializeField] private TouchController m_controller = null;

        private bool m_wasButtonDown = false;

        private void Awake () {
            ModeChange(m_displayMode);
        }

        // Cycles through controller visualization types on thumbstick click.
        private void Update () {
            TrackedController controller = m_hand != null ? TrackedController.GetController(m_hand.Handedness) : null;
            if (controller != null)
            {
                DisplayMode nextDisplayMode = m_displayMode;
                bool isButtonDown = controller.ButtonJoystick;
                if (isButtonDown && !m_wasButtonDown)
                {
                    nextDisplayMode = (DisplayMode)((int)(m_displayMode + 1) % (int)DisplayMode.Count);
                }
                m_wasButtonDown = isButtonDown;

                if (m_displayMode != nextDisplayMode)
                {
                    ModeChange(nextDisplayMode);
                }
            }
        }

        private void ModeChange (DisplayMode nextDisplayMode) {
            m_controller.gameObject.SetActive(nextDisplayMode != DisplayMode.Hand);
            m_hand.HandVisible = nextDisplayMode == DisplayMode.Hand || nextDisplayMode == DisplayMode.HandAndController;
            m_displayMode = nextDisplayMode;
        }
    }
}
