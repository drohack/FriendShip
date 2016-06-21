using UnityEngine;
using System.Collections;
using OvrTouch.Hands;
using OvrTouch.Controllers;

public class PhotonNetworkOvrRig : Photon.MonoBehaviour
{

    [SerializeField]
    Transform centerEyeAnchor;
    [SerializeField]
    Transform LeftHandPf;
    [SerializeField]
    Hand handScriptL;
    [SerializeField]
    VelocityTracker velocityTrackerL;
    [SerializeField]
    Animator animatorL;
    [SerializeField]
    Transform RightHandPf;
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

    void Start()
    {
        m_animLayerIndexPointL = animatorL.GetLayerIndex(Const.AnimLayerNamePoint);
        m_animLayerIndexThumbL = animatorL.GetLayerIndex(Const.AnimLayerNameThumb);
        m_animLayerIndexPointR = animatorR.GetLayerIndex(Const.AnimLayerNamePoint);
        m_animLayerIndexThumbR = animatorR.GetLayerIndex(Const.AnimLayerNameThumb);
        m_animParamIndexFlex = Animator.StringToHash(Const.AnimParamNameFlex);
        m_animParamIndexPose = Animator.StringToHash(Const.AnimParamNamePose);
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(centerEyeAnchor.position);
            stream.SendNext(centerEyeAnchor.rotation);
            stream.SendNext(LeftHandPf.position);
            stream.SendNext(LeftHandPf.rotation);
            stream.SendNext(RightHandPf.position);
            stream.SendNext(RightHandPf.rotation);
        }
        else
        {
            //Network player, receive data
            centerEyeAnchorPos = (Vector3)stream.ReceiveNext();
            centerEyeAnchorRot = (Quaternion)stream.ReceiveNext();
            LeftHandPfPos = (Vector3)stream.ReceiveNext();
            LeftHandPfRot = (Quaternion)stream.ReceiveNext();
            RightHandPfPos = (Vector3)stream.ReceiveNext();
            RightHandPfRot = (Quaternion)stream.ReceiveNext();
        }
    }

    private Vector3 centerEyeAnchorPos = Vector3.zero; //We lerp towards this
    private Quaternion centerEyeAnchorRot = Quaternion.identity; //We lerp towards this
    private Vector3 LeftHandPfPos = Vector3.zero; //We lerp towards this
    private Quaternion LeftHandPfRot = Quaternion.identity; //We lerp towards this
    private Vector3 RightHandPfPos = Vector3.zero; //We lerp towards this
    private Quaternion RightHandPfRot = Quaternion.identity; //We lerp towards this

    void Update()
    {
        if (!photonView.isMine)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            centerEyeAnchor.position = Vector3.Lerp(centerEyeAnchor.position, centerEyeAnchorPos, Time.deltaTime * 20);
            centerEyeAnchor.rotation = Quaternion.Lerp(centerEyeAnchor.rotation, centerEyeAnchorRot, Time.deltaTime * 20);
            LeftHandPf.position = Vector3.Lerp(LeftHandPf.position, LeftHandPfPos, Time.deltaTime * 20);
            LeftHandPf.rotation = Quaternion.Lerp(LeftHandPf.rotation, LeftHandPfRot, Time.deltaTime * 20);
            RightHandPf.position = Vector3.Lerp(RightHandPf.position, RightHandPfPos, Time.deltaTime * 20);
            RightHandPf.rotation = Quaternion.Lerp(RightHandPf.rotation, RightHandPfRot, Time.deltaTime * 20);
        }
    }

    public void SendAnimation(HandednessId m_handedness, int handPoseId, float m_flex, bool canPoint, float m_point, bool canThumbsUp, float m_thumbsUp)
    {
        photonView.RPC("RpcSendAnimation", PhotonTargets.Others, m_handedness, handPoseId, m_flex, canPoint, m_point, canThumbsUp, m_thumbsUp);
    }

    [PunRPC]
    public void RpcSendAnimation(HandednessId m_handedness, int handPoseId, float m_flex, bool canPoint, float m_point, bool canThumbsUp, float m_thumbsUp)
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

    public void ToggleMeshRenderer(HandednessId m_handedness, bool isEnabled)
    {
        photonView.RPC("RpcToggleMeshRenderer", PhotonTargets.Others, m_handedness, isEnabled);
    }

    [PunRPC]
    public void RpcToggleMeshRenderer(HandednessId m_handedness, bool isEnabled)
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