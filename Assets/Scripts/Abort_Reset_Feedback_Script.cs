using UnityEngine;
using System.Collections;

public class Abort_Reset_Feedback_Script : Photon.MonoBehaviour
{

    [SerializeField]
    public GameObject p1_Abort_Light;
    [SerializeField]
    public GameObject p2_Abort_Light;
    [SerializeField]
    public GameObject p3_Abort_Light;
    [SerializeField]
    public GameObject p4_Abort_Light;
    [SerializeField]
    public GameObject p1_Reset_Light;
    [SerializeField]
    public GameObject p2_Reset_Light;
    [SerializeField]
    public GameObject p3_Reset_Light;
    [SerializeField]
    public GameObject p4_Reset_Light;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void p1_Resetting()
    {
        p1_Reset_Light.GetComponent<Renderer>().material.color = Color.green;
        photonView.RPC("RPC_p1_Resetting", PhotonTargets.Others);
    }

    public void p2_Resetting()
    {
        p2_Reset_Light.GetComponent<Renderer>().material.color = Color.green;
        photonView.RPC("RPC_p2_Resetting", PhotonTargets.Others);
    }

    public void p3_Resetting()
    {
        p3_Reset_Light.GetComponent<Renderer>().material.color = Color.green;
        photonView.RPC("RPC_p3_Resetting", PhotonTargets.Others);
    }

    public void p4_Resetting()
    {
        p4_Reset_Light.GetComponent<Renderer>().material.color = Color.green;
        photonView.RPC("RPC_p4_Resetting", PhotonTargets.Others);
    }

    public void p1_NotResetting()
    {
        p1_Reset_Light.GetComponent<Renderer>().material.color = Color.red;
        photonView.RPC("RPC_p1_NotResetting", PhotonTargets.Others);
    }

    public void p2_NotResetting()
    {
        p2_Reset_Light.GetComponent<Renderer>().material.color = Color.red;
        photonView.RPC("RPC_p2_NotResetting", PhotonTargets.Others);
    }

    public void p3_NotResetting()
    {
        p3_Reset_Light.GetComponent<Renderer>().material.color = Color.red;
        photonView.RPC("RPC_p3_NotResetting", PhotonTargets.Others);
    }

    public void p4_NotResetting()
    {
        p4_Reset_Light.GetComponent<Renderer>().material.color = Color.red;
        photonView.RPC("RPC_p4_NotResetting", PhotonTargets.Others);
    }

    public void p1_Aborting()
    {
        p1_Abort_Light.GetComponent<Renderer>().material.color = Color.green;
        photonView.RPC("RPC_p1_Aborting", PhotonTargets.Others);
    }

    public void p2_Aborting()
    {
        p2_Abort_Light.GetComponent<Renderer>().material.color = Color.green;
        photonView.RPC("RPC_p2_Aborting", PhotonTargets.Others);
    }

    public void p3_Aborting()
    {
        p3_Abort_Light.GetComponent<Renderer>().material.color = Color.green;
        photonView.RPC("RPC_p3_Aborting", PhotonTargets.Others);
    }

    public void p4_Aborting()
    {
        p4_Abort_Light.GetComponent<Renderer>().material.color = Color.green;
        photonView.RPC("RPC_p4_Aborting", PhotonTargets.Others);
    }

    public void p1_NotAborting()
    {
        p1_Abort_Light.GetComponent<Renderer>().material.color = Color.red;
        photonView.RPC("RPC_p1_NotAborting", PhotonTargets.Others);
    }

    public void p2_NotAborting()
    {
        p2_Abort_Light.GetComponent<Renderer>().material.color = Color.red;
        photonView.RPC("RPC_p2_NotAborting", PhotonTargets.Others);
    }

    public void p3_NotAborting()
    {
        p3_Abort_Light.GetComponent<Renderer>().material.color = Color.red;
        photonView.RPC("RPC_p3_NotAborting", PhotonTargets.Others);
    }

    public void p4_NotAborting()
    {
        p4_Abort_Light.GetComponent<Renderer>().material.color = Color.red;
        photonView.RPC("RPC_p4_NotAborting", PhotonTargets.Others);
    }

    [PunRPC]
    void RPC_p1_Resetting()
    {
        p1_Reset_Light.GetComponent<Renderer>().material.color = Color.green;
    }

    [PunRPC]
    void RPC_p2_Resetting()
    {
        p2_Reset_Light.GetComponent<Renderer>().material.color = Color.green;
    }

    [PunRPC]
    void RPC_p3_Resetting()
    {
        p3_Reset_Light.GetComponent<Renderer>().material.color = Color.green;
    }

    [PunRPC]
    void RPC_p4_Resetting()
    {
        p4_Reset_Light.GetComponent<Renderer>().material.color = Color.green;
    }

    [PunRPC]
    void RPC_p1_NotResetting()
    {
        p1_Reset_Light.GetComponent<Renderer>().material.color = Color.red;
    }

    [PunRPC]
    void RPC_p2_NotResetting()
    {
        p2_Reset_Light.GetComponent<Renderer>().material.color = Color.red;
    }

    [PunRPC]
    void RPC_p3_NotResetting()
    {
        p3_Reset_Light.GetComponent<Renderer>().material.color = Color.red;
    }

    [PunRPC]
    void RPC_p4_NotResetting()
    {
        p4_Reset_Light.GetComponent<Renderer>().material.color = Color.red;
    }

    [PunRPC]
    void RPC_p1_Aborting()
    {
        p1_Abort_Light.GetComponent<Renderer>().material.color = Color.green;
    }

    [PunRPC]
    void RPC_p2_Aborting()
    {
        p2_Abort_Light.GetComponent<Renderer>().material.color = Color.green;
    }

    [PunRPC]
    void RPC_p3_Aborting()
    {
        p3_Abort_Light.GetComponent<Renderer>().material.color = Color.green;
    }

    [PunRPC]
    void RPC_p4_Aborting()
    {
        p4_Abort_Light.GetComponent<Renderer>().material.color = Color.green;
    }

    [PunRPC]
    void RPC_p1_NotAborting()
    {
        p1_Abort_Light.GetComponent<Renderer>().material.color = Color.red;
    }

    [PunRPC]
    void RPC_p2_NotAborting()
    {
        p2_Abort_Light.GetComponent<Renderer>().material.color = Color.red;
    }

    [PunRPC]
    void RPC_p3_NotAborting()
    {
        p3_Abort_Light.GetComponent<Renderer>().material.color = Color.red;
    }

    [PunRPC]
    void RPC_p4_NotAborting()
    {
        p4_Abort_Light.GetComponent<Renderer>().material.color = Color.red;
    }
}