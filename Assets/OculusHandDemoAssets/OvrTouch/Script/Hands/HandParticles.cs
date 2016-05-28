/********************************************************************************//**
\file      HandParticles.cs
\brief     Manage particle systems for the hand by using a dummy rigidbody, collider
           and fixed joint to inherit initial particle velocity.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

using UnityEngine;

namespace OvrTouch.Hands {

    [RequireComponent(typeof(ParticleSystem))]
    public class HandParticles : MonoBehaviour {

        //==============================================================================
        // Fields
        //==============================================================================

        [SerializeField] private Hand m_hand = null;
        [SerializeField] private float m_emissionRateVelocityScale = 25.0f;

        private ParticleSystem m_particleSystem = null;
        private Rigidbody m_rigidbody = null;
        private FixedJoint m_fixedJoint = null;

        //==============================================================================
        // Public
        //==============================================================================

        //==============================================================================
        public void SetHand (Hand hand) {
            m_hand = hand;
            m_rigidbody.transform.position = m_hand.transform.position;
            m_rigidbody.transform.rotation = m_hand.transform.rotation;
            m_fixedJoint.connectedBody = m_hand.Rigidbody;
        }

        //==============================================================================
        // MonoBehaviour
        //==============================================================================

        //==============================================================================
        private void Awake () {
            // Get the particle system
            m_particleSystem = this.GetComponent<ParticleSystem>();

            // Collider
            SphereCollider sphereCollider = this.gameObject.AddComponent<SphereCollider>();
            sphereCollider.radius = 0.01f;
            sphereCollider.isTrigger = true;

            // Rigidbody
            m_rigidbody = this.gameObject.AddComponent<Rigidbody>();

            // Fixed joint
            m_fixedJoint = this.gameObject.AddComponent<FixedJoint>();

            if (m_hand != null) {
                SetHand(m_hand);
            }
        }

        //==============================================================================
        private void Update () {
            if (m_hand != null) {
                // Set particle emission rate
                float emissionRate = m_hand.LinearVelocity.magnitude * m_emissionRateVelocityScale;
                m_particleSystem.emissionRate = emissionRate;
            }
        }

    }

}
