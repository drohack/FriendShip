using UnityEngine;
using System.Collections;
using OVRTouchSample;

public class PhotonNetworkOvrRig : Photon.MonoBehaviour
{
    [SerializeField]
    Transform ovrCameraRig;
    [SerializeField]
    Transform centerEyeAnchor;
    [SerializeField]
    Transform LeftHandPf;
    [SerializeField]
    Hand handScriptL;
    [SerializeField]
    VelocityTracker velocityTrackerL;
    [SerializeField]
    TrackedController trackedControllerL;
    [SerializeField]
    Grabbed_Hand_Script grabbedHandScriptL;
    [SerializeField]
    Velocity_Tracker velocity_TrackerL;
    [SerializeField]
    GameObject l_hand_world;
    [SerializeField]
    GameObject GrabVolumeBigL;
    [SerializeField]
    Animator animatorL;
    [SerializeField]
    SkinnedMeshRenderer skinnedMeshRendererL;
    [SerializeField]
    Transform RightHandPf;
    [SerializeField]
    Hand handScriptR;
    [SerializeField]
    VelocityTracker velocityTrackerR;
    [SerializeField]
    Grabbed_Hand_Script grabbedHandScriptR;
    [SerializeField]
    TrackedController trackedControllerR;
    [SerializeField]
    Velocity_Tracker velocity_TrackerR;
    [SerializeField]
    GameObject r_hand_world;
    [SerializeField]
    GameObject GrabVolumeBigR;
    [SerializeField]
    Animator animatorR;
    [SerializeField]
    SkinnedMeshRenderer skinnedMeshRendererR;
    [SerializeField]
    GameObject playerArea;
    [SerializeField]
    PhotonVoiceRecorder photonVoiceRecorder;

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

    private bool leftIsEnabled = true;
    private bool rightIsEnabled = true;
    private Transform oldLeftHandPf = null;
    private Transform oldRightHandPf = null;

    private static PhotonNetworkOvrRig instance;
    private static object instanceLock = new object();

    void Awake()
    {
        // Don't destroy the object when another client loads in
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        if (photonView.isMine)
        {
            // Enable your own camera and scripts
            gameObject.AddComponent<RecenterController>();
            centerEyeAnchor.GetComponent<Camera>().enabled = true;
            centerEyeAnchor.GetComponent<AudioListener>().enabled = true;
            trackedControllerL.enabled = true;
            handScriptL.enabled = true;
            velocityTrackerL.enabled = true;
            grabbedHandScriptL.enabled = true;
            velocity_TrackerL.enabled = true;
            l_hand_world.SetActive(true);
            GrabVolumeBigL.SetActive(true);
            trackedControllerR.enabled = true;
            handScriptR.enabled = true;
            velocityTrackerR.enabled = true;
            grabbedHandScriptR.enabled = true;
            velocity_TrackerR.enabled = true;
            r_hand_world.SetActive(true);
            GrabVolumeBigR.SetActive(true);
            playerArea.SetActive(true);
            photonVoiceRecorder.enabled = true;
            ovrCameraRig.gameObject.AddComponent<OVRCameraRig>();
            ovrCameraRig.gameObject.AddComponent<OVRManager>();
        }

        m_animLayerIndexPointL = animatorL.GetLayerIndex(Const.AnimLayerNamePoint);
        m_animLayerIndexThumbL = animatorL.GetLayerIndex(Const.AnimLayerNameThumb);
        m_animLayerIndexPointR = animatorR.GetLayerIndex(Const.AnimLayerNamePoint);
        m_animLayerIndexThumbR = animatorR.GetLayerIndex(Const.AnimLayerNameThumb);
        m_animParamIndexFlex = Animator.StringToHash(Const.AnimParamNameFlex);
        m_animParamIndexPose = Animator.StringToHash(Const.AnimParamNamePose);
    }

    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("OnDisconnectedFromPhoton");

        PhotonNetwork.player.customProperties.Clear();
    }

    public void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom (local)");

        PhotonNetwork.player.customProperties.Clear();
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(centerEyeAnchor.position);
            stream.SendNext(centerEyeAnchor.rotation);
            if(leftIsEnabled)
            {
                oldLeftHandPf = null;
                stream.SendNext(LeftHandPf.position);
                stream.SendNext(LeftHandPf.rotation);
            }
            else
            {
                if(oldLeftHandPf == null)
                {
                    oldLeftHandPf = LeftHandPf;
                }
                stream.SendNext(oldLeftHandPf.position);
                stream.SendNext(oldLeftHandPf.rotation);
            }
            if (rightIsEnabled)
            {
                oldRightHandPf = null;
                stream.SendNext(RightHandPf.position);
                stream.SendNext(RightHandPf.rotation);
            }
            else
            {
                if (oldRightHandPf == null)
                {
                    oldRightHandPf = RightHandPf;
                }
                stream.SendNext(oldRightHandPf.position);
                stream.SendNext(oldRightHandPf.rotation);
            }
            stream.SendNext((int)handScriptL.m_handednessId);
            stream.SendNext((int)handScriptL.handPoseId);
            stream.SendNext(handScriptL.m_flex);
            stream.SendNext(handScriptL.canPoint);
            stream.SendNext(handScriptL.m_point);
            stream.SendNext(handScriptL.canThumbsUp);
            stream.SendNext(handScriptL.m_thumbsUp);
            stream.SendNext((int)handScriptR.m_handednessId);
            stream.SendNext((int)handScriptR.handPoseId);
            stream.SendNext(handScriptR.m_flex);
            stream.SendNext(handScriptR.canPoint);
            stream.SendNext(handScriptR.m_point);
            stream.SendNext(handScriptR.canThumbsUp);
            stream.SendNext(handScriptR.m_thumbsUp);
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
            m_handednessL = (HandednessId)stream.ReceiveNext();
            handPoseIdL = (HandPoseId)stream.ReceiveNext();
            m_flexL = (float)stream.ReceiveNext();
            canPointL = (bool)stream.ReceiveNext();
            m_pointL = (float)stream.ReceiveNext();
            canThumbsUpL = (bool)stream.ReceiveNext();
            m_thumbsUpL = (float)stream.ReceiveNext();
            m_handednessR = (HandednessId)stream.ReceiveNext();
            handPoseIdR = (HandPoseId)stream.ReceiveNext();
            m_flexR = (float)stream.ReceiveNext();
            canPointR = (bool)stream.ReceiveNext();
            m_pointR = (float)stream.ReceiveNext();
            canThumbsUpR = (bool)stream.ReceiveNext();
            m_thumbsUpR = (float)stream.ReceiveNext();
        }
    }

    private Vector3 centerEyeAnchorPos = Vector3.zero; //We lerp towards this
    private Quaternion centerEyeAnchorRot = Quaternion.identity; //We lerp towards this
    private Vector3 LeftHandPfPos = Vector3.zero; //We lerp towards this
    private Quaternion LeftHandPfRot = Quaternion.identity; //We lerp towards this
    private Vector3 RightHandPfPos = Vector3.zero; //We lerp towards this
    private Quaternion RightHandPfRot = Quaternion.identity; //We lerp towards this
    //Animation variables to pass
    private HandednessId m_handednessL = HandednessId.Left;
    private HandPoseId handPoseIdL = HandPoseId.Default;
    private float m_flexL = 0.0f;
    private bool canPointL = true;
    private float m_pointL = 0.0f;
    private bool canThumbsUpL = true;
    private float m_thumbsUpL = 0.0f;
    private HandednessId m_handednessR = HandednessId.Left;
    private HandPoseId handPoseIdR = HandPoseId.Default;
    private float m_flexR = 0.0f;
    private bool canPointR = true;
    private float m_pointR = 0.0f;
    private bool canThumbsUpR = true;
    private float m_thumbsUpR = 0.0f;

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
            UpdateHandAnimation(m_handednessL, handPoseIdL, m_flexL, canPointL, m_pointL, canThumbsUpL, m_thumbsUpL);
            UpdateHandAnimation(m_handednessR, handPoseIdR, m_flexR, canPointR, m_pointR, canThumbsUpR, m_thumbsUpR);
        }

        //if (OVRInput.Get(OVRInput.Button.Four))
        //{
        //    photonVoiceRecorder.Transmit = true;
        //    Debug.Log("You're holding the right button!  Hooray!");
        //}
        //else if (!OVRInput.Get(OVRInput.Button.Four))
        //{
        //    photonVoiceRecorder.Transmit = false;
        //}

        if (photonVoiceRecorder != null && photonVoiceRecorder.enabled && !photonVoiceRecorder.Transmit)
        {
            //Debug.Log("Turn On Transmit");
            photonVoiceRecorder.Transmit = true;
        }
    }

    public void UpdateHandAnimation(HandednessId m_handedness, HandPoseId handPoseId, float m_flex, bool canPoint, float m_point, bool canThumbsUp, float m_thumbsUp)
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
        m_animator.SetInteger(m_animParamIndexPose, (int)handPoseId);

        // Flex
        m_animator.SetFloat(m_animParamIndexFlex, Mathf.Lerp(m_animator.GetFloat(m_animParamIndexFlex), m_flex, Time.deltaTime * 20));

        // Point
        float point = canPoint ? Mathf.Lerp(m_animator.GetLayerWeight(m_animLayerIndexPoint), m_point, Time.deltaTime * 20) : 0.0f;
        m_animator.SetLayerWeight(m_animLayerIndexPoint, point);

        // Thumbs up
        float thumbsUp = canThumbsUp ? Mathf.Lerp(m_animator.GetLayerWeight(m_animLayerIndexThumb), m_thumbsUp, Time.deltaTime * 20) : 0.0f;
        m_animator.SetLayerWeight(m_animLayerIndexThumb, thumbsUp);
    }

    public void ToggleMeshRenderer(HandednessId m_handedness, bool isEnabled)
    {
        if (m_handedness.Equals(HandednessId.Left))
        {
            leftIsEnabled = isEnabled;
        }
        else
        {
            //Debug.Log("rightIsEnabled: " + isEnabled);
            rightIsEnabled = isEnabled;
        }
        photonView.RPC("RpcToggleMeshRenderer", PhotonTargets.Others, m_handedness, isEnabled);
    }

    [PunRPC]
    public void RpcToggleMeshRenderer(HandednessId m_handedness, bool isEnabled)
    {
        if (m_handedness.Equals(HandednessId.Left))
        {
            skinnedMeshRendererL.enabled = isEnabled;
        }
        else
        {
            skinnedMeshRendererR.enabled = isEnabled;
        }
    }

    [PunRPC]
    public void RpcLeaveRoom()
    {
        if (photonView.isMine) 
        {
            PhotonNetwork.LeaveRoom();
        }
    }

}