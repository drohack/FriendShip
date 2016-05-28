/********************************************************************************//**
\file      PhysicsSettings.cs
\brief     Allows setting world gravity and bounce threshold.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

using UnityEngine;

namespace OvrTouch.Services {

    public class PhysicsSettings : MonoBehaviour {

        //==============================================================================
        // Fields
        //==============================================================================

        [SerializeField] private Vector3 m_gravity = new Vector3(0.0f, -6.8f, 0.0f);

        //==============================================================================
        // MonoBehaviour
        //==============================================================================

        //==============================================================================
	    private void Start () {	
            Physics.gravity = m_gravity;
            Physics.bounceThreshold = Mathf.Max(1.0f, m_gravity.magnitude * 0.15f);
	    }

    }

}
