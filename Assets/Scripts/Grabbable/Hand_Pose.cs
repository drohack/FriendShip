/********************************************************************************//**
\file      HandPose.cs
\brief     Stores pose-specific data such as the animation id and allowing gestures.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

using UnityEngine;

public enum HandPoseId {
    Default,
    Generic,
    PingPongBall,
}

public enum HandPoseAttachType {
    None,
    Snap,
    SnapPosition,
}

public class Hand_Pose : MonoBehaviour {

    //==============================================================================
    // Fields
    //==============================================================================

    [SerializeField] private bool m_allowPointing = false;
    [SerializeField] private bool m_allowThumbsUp = false;
    [SerializeField] private HandPoseId m_poseId = HandPoseId.Default;
    [SerializeField] private HandPoseAttachType m_attachType = HandPoseAttachType.None;

    //==============================================================================
    // Properties
    //==============================================================================

    public bool AllowPointing {
        get { return m_allowPointing; }
    }

    public bool AllowThumbsUp {
        get { return m_allowThumbsUp; }
    }

    public HandPoseId PoseId {
        get { return m_poseId; }
    }

    public HandPoseAttachType AttachType {
        get { return m_attachType; }
    }

}
