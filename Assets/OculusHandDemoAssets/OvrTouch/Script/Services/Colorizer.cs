/********************************************************************************//**
\file      Colorizer.cs
\brief     Simple component that changes color based on grab state.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

using UnityEngine;
using OvrTouch.Hands;

namespace OvrTouch.Services {

    public class Colorizer : MonoBehaviour {

        //==============================================================================
        // Nested Types
        //==============================================================================

        private static class Const {

            public static readonly Color ColorGrab = new Color(1.0f, 0.5f, 0.0f, 1.0f);

        }

        //==============================================================================
        // Fields
        //==============================================================================

        private Color m_color = Color.black;
        private MeshRenderer[] m_meshRenderers = null;

        //==============================================================================
        // HandGrabbable
        //==============================================================================

        //==============================================================================
        private void OnGrabBegin (GrabbableGrabMsg grabMsg) {
            SetColor(Const.ColorGrab);
        }

        //==============================================================================
        private void OnGrabEnd (GrabbableGrabMsg grabMsg) {
            SetColor(m_color);
        }

        //==============================================================================
        private void OnOverlapBegin (GrabbableOverlapMsg overlapMsg) {
        }

        //==============================================================================
        private void OnOverlapEnd (GrabbableOverlapMsg overlapMsg) {
        }

        //==============================================================================
        // MonoBehaviour
        //==============================================================================

        //==============================================================================
        private void Awake () {
            // Get random color and mesh renderers
            m_color = RandomColor();
            m_meshRenderers = this.GetComponentsInChildren<MeshRenderer>();

            // Set the random color
            SetColor(m_color);
        }

        //==============================================================================
        // Private
        //==============================================================================

        //==============================================================================
        private void SetColor (Color color) {
            foreach (MeshRenderer meshRenderer in m_meshRenderers) {
                foreach (Material meshMaterial in meshRenderer.materials) {
                    meshMaterial.color = color;
                }
            }
        }

        //==============================================================================
        private Color RandomColor () {
            Color randomColor = new Color(
                Random.Range(0.1f, 0.95f),
                Random.Range(0.1f, 0.95f),
                Random.Range(0.1f, 0.95f),
                1.0f
            );
            return randomColor;
        }

    }

}
