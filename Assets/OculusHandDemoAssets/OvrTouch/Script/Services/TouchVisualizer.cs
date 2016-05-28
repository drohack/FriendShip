/********************************************************************************//**
\file      TouchVisualizer.cs
\brief     Toggle visibility for hands and controllers.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

using UnityEngine;
using OvrTouch.Controllers;
using OvrTouch.Hands;

namespace OvrTouch.Services {

    public class TouchVisualizer : MonoBehaviour {

        //==============================================================================
        // Nested Types
        //==============================================================================

        private enum DisplayMode {
            Hand,
            Controller,
            HandAndController,
            Count,
        }

        //==============================================================================
        // Fields
        //==============================================================================

        [SerializeField] private DisplayMode m_displayMode = DisplayMode.Controller;
        [SerializeField] private Hand m_hand = null;
        [SerializeField] private TouchController m_controller = null;

        private bool m_wasButtonDown = false;

        //==============================================================================
        // MonoBehaviour
        //==============================================================================

        //==============================================================================
        private void Awake () {
            ModeChange(m_displayMode);
        }

        //==============================================================================
        private void LateUpdate () {
            bool isValid = (
                (m_hand != null) &&
                (m_hand.TrackedController != null)
            );
            if (!isValid) {
                return;
            }

            DisplayMode nextDisplayMode = m_displayMode;
            bool isButtonDown = m_hand.TrackedController.ButtonJoystick;
            if (isButtonDown && !m_wasButtonDown) {
                nextDisplayMode = (DisplayMode)((int)(m_displayMode + 1) % (int)DisplayMode.Count);
            }
            m_wasButtonDown = isButtonDown;

            if (m_displayMode != nextDisplayMode) {
                ModeChange(nextDisplayMode);
            }
        }

        //==============================================================================
        // Private
        //==============================================================================

        //==============================================================================
        private void ModeChange (DisplayMode nextDisplayMode) {
            // Hide visuals
            m_controller.SetVisible(false);
            m_hand.SetVisible(false);

            // Change display mode
            switch (nextDisplayMode) {
                // Hand
                case DisplayMode.Hand:
                    m_hand.SetVisible(true);
                    break;
                
                // Controller
                case DisplayMode.Controller:
                    m_controller.SetVisible(true);
                    break;

                // HandAndController
                case DisplayMode.HandAndController:
                    m_hand.SetVisible(true);
                    m_controller.SetVisible(true);
                    break;
            }
            m_displayMode = nextDisplayMode;
        }

    }

}
