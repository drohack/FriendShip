/********************************************************************************//**
\file      GrabTrigger.cs
\brief     GrabTriggers are automatically added by the Grabbable component to grab
           colliders. This provides a link between the grabbed collider and the
           grabbable component.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

using System;
using UnityEngine;

public class Grab_Trigger : MonoBehaviour {

    //==============================================================================
    // Fields
    //==============================================================================

    private Grabbable m_grabbable = null;

    //==============================================================================
    // Properties
    //==============================================================================

    public Grabbable Grabbable {
        get { return m_grabbable; }
    }

    //==============================================================================
    // Public
    //==============================================================================

    //==============================================================================
    public void SetGrabbable (Grabbable grabbable) {
        if (m_grabbable != null) {
            throw new InvalidOperationException(
                "GrabTrigger: Grabbable already set by a different grab point -- make sure multiple grab points are not referencing the same collider."
            );
        }
        m_grabbable = grabbable;
    }

}
