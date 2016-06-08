using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using OvrTouch.Hands;
using OvrTouch.Controllers;

public class Player_NetworkSetup : NetworkBehaviour
{

    [SerializeField]
    OVRCameraRig ovrCameraRig;
    [SerializeField]
    Camera FPSCharacterCam;
    [SerializeField]
    AudioListener audioListener;
    [SerializeField]
    Hand handScriptL;
    [SerializeField]
    VelocityTracker velocityTrackerL;
    [SerializeField]
    Animator animatorL;
    [SerializeField]
    Hand handScriptR;
    [SerializeField]
    VelocityTracker velocityTrackerR;
    [SerializeField]
    Animator animatorR;

    private static class Const
    {
        public const string AnimLayerNamePoint = "Point Layer";
        public const string AnimLayerNameThumb = "Thumb Layer";
        public const string AnimParamNameFlex = "Flex";
        public const string AnimParamNamePose = "Pose";
    }

    private int m_animLayerIndexThumbL = -1;
    private int m_animLayerIndexPointL = -1;
    private int m_animLayerIndexThumbR = -1;
    private int m_animLayerIndexPointR = -1;
    private int m_animParamIndexFlex = -1;
    private int m_animParamIndexPose = -1;

    private bool isPlayerThatSentCommand = false;

    // Use this for initialization
    void Start()
    {
        Debug.Log("In Player_NetworkSetup, isLocalPlayer: " + isLocalPlayer);
        if (isLocalPlayer)
        {
            GameObject.Find("Main Camera").SetActive(false);
            // Add OVRManager
            transform.Find("OVRCameraRig").gameObject.AddComponent<OVRManager>();
            ovrCameraRig.enabled = true;
            FPSCharacterCam.enabled = true;
            audioListener.enabled = true;
            handScriptL.enabled = true;
            velocityTrackerL.enabled = true;
            handScriptR.enabled = true;
            velocityTrackerR.enabled = true;
        }

        m_animLayerIndexPointL = animatorL.GetLayerIndex(Const.AnimLayerNamePoint);
        m_animLayerIndexThumbL = animatorL.GetLayerIndex(Const.AnimLayerNameThumb);
        m_animLayerIndexPointR = animatorR.GetLayerIndex(Const.AnimLayerNamePoint);
        m_animLayerIndexThumbR = animatorR.GetLayerIndex(Const.AnimLayerNameThumb);
        m_animParamIndexFlex = Animator.StringToHash(Const.AnimParamNameFlex);
        m_animParamIndexPose = Animator.StringToHash(Const.AnimParamNamePose);
    }

    public void SendAnimation(HandednessId m_handedness, int handPoseId, float m_flex, bool canPoint, float m_point, bool canThumbsUp, float m_thumbsUp)
    {
        isPlayerThatSentCommand = true;
        CmdSendAnimation(m_handedness, handPoseId, m_flex, canPoint, m_point, canThumbsUp, m_thumbsUp);
    }

    [Command]
    public void CmdSendAnimation(HandednessId m_handedness, int handPoseId, float m_flex, bool canPoint, float m_point, bool canThumbsUp, float m_thumbsUp)
    {
        RpcSendAnimation(m_handedness, handPoseId, m_flex, canPoint, m_point, canThumbsUp, m_thumbsUp);
    }

    [ClientRpc]
    public void RpcSendAnimation(HandednessId m_handedness, int handPoseId, float m_flex, bool canPoint, float m_point, bool canThumbsUp, float m_thumbsUp)
    {
        if (!isPlayerThatSentCommand)
        {
            Animator m_animator = animatorL;
            int m_animLayerIndexPoint = m_animLayerIndexPointL;
            int m_animLayerIndexThumb = m_animLayerIndexThumbL;
            if (m_handedness.Equals(HandednessId.Right))
            {
                m_animator = animatorR;
                m_animLayerIndexPoint = m_animLayerIndexPointR;
                m_animLayerIndexThumb = m_animLayerIndexThumbR;
            }

            // Pose
            m_animator.SetInteger(m_animParamIndexPose, handPoseId);

            // Flex
            m_animator.SetFloat(m_animParamIndexFlex, m_flex);

            // Point
            float point = canPoint ? m_point : 0.0f;
            m_animator.SetLayerWeight(m_animLayerIndexPoint, point);

            // Thumbs up
            float thumbsUp = canThumbsUp ? m_thumbsUp : 0.0f;
            m_animator.SetLayerWeight(m_animLayerIndexThumb, thumbsUp);
        }
        else
        {
            isPlayerThatSentCommand = false;
        }
    }

    public void ToggleMeshRenderer(HandednessId m_handedness, bool isEnabled)
    {
        isPlayerThatSentCommand = true;
        CmdToggleMeshRenderer(m_handedness, isEnabled);
    }

    [Command]
    public void CmdToggleMeshRenderer(HandednessId m_handedness, bool isEnabled)
    {
        RpcToggleMeshRenderer(m_handedness, isEnabled);
    }

    [ClientRpc]
    public void RpcToggleMeshRenderer(HandednessId m_handedness, bool isEnabled)
    {
        if (!isPlayerThatSentCommand)
        {
            if (m_handedness.Equals(HandednessId.Left))
            {
                transform.Find("LeftHandPf").GetChild(0).GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().enabled = isEnabled;
            }
            else
            {
                transform.Find("RightHandPf").GetChild(0).GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().enabled = isEnabled;
            }
        }
    }
}
