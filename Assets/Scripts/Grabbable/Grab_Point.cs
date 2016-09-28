/********************************************************************************//**
\file      GrabPoint.cs
\brief     Defines a point on a game object which can be grabbed.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

using System;
using UnityEngine;

[Serializable]
public class Grab_Point {

    //==============================================================================
    // Fields
    //==============================================================================

    [SerializeField] private Hand_Pose m_handPose = null;
    [SerializeField] private Collider m_grabCollider = null;
    [SerializeField] private Transform m_grabOverride = null;

    private Transform m_grabTransform = null;
    private Rigidbody m_rigidbody = null;

    //==============================================================================
    // Properties
    //==============================================================================

    public Hand_Pose HandPose {
        get { return m_handPose; }
    }

    public Collider GrabCollider {
        get { return m_grabCollider; }
    }

    public Transform GrabTransform {
        get { return m_grabTransform; }
    }

    public Rigidbody Rigidbody {
        get { return m_rigidbody; }
    }

    //==============================================================================
    // Public
    //==============================================================================

    //==============================================================================
    public Grab_Point (Collider collider) {
        m_grabCollider = collider;
    }

    //==============================================================================
    public void Initialize () {
        if (m_grabCollider == null) {
            throw new ArgumentException("GrabPoint: Grab points require a grab collider -- please set a collider.");
        }
            
        // Initialize
        m_grabTransform = (m_grabOverride != null) ? m_grabOverride : m_grabCollider.transform;
        m_rigidbody = m_grabCollider.attachedRigidbody;
    }

}
