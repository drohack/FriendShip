using UnityEngine;
using System.Collections;

public class Command_Feedback_Script : Photon.MonoBehaviour
{

    [SerializeField]
    public Light successLight;
    [SerializeField]
    public AudioSource successAudio;
    [SerializeField]
    public Light failLight;
    [SerializeField]
    public AudioSource failAudio;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlaySuccessFeedback()
    {
        successLight.enabled = true;
        successAudio.Play();
        photonView.RPC("RPCPlaySuccessFeedback", PhotonTargets.Others);
    }

    public void PlayFailFeedback()
    {
        failLight.enabled = true;
        failAudio.Play();
        photonView.RPC("RPCPlayFailFeedback", PhotonTargets.Others);
    }

    public void Reset()
    {
        failLight.enabled = false;
        successLight.enabled = false;
        photonView.RPC("RPCPlayResetFeedback", PhotonTargets.Others);
    }

    [PunRPC]
    void RPCPlayResetFeedback()
    {
        failLight.enabled = false;
        successLight.enabled = false;
    }

    [PunRPC]
    void RPCPlaySuccessFeedback()
    {
        successLight.enabled = true;
        successAudio.Play();
    }

    [PunRPC]
    void RPCPlayFailFeedback()
    {
        failLight.enabled = true;
        failAudio.Play();
    }
}