using UnityEngine;
using System.Collections;

public class Abort_Reset_Rotate_Feedback_Script : Photon.MonoBehaviour
{
    public float smooth = 1f;
    public Vector3 targetAngles;
    public bool isRotating = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isRotating)
        {
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetAngles, smooth * Time.deltaTime);
            float angleDifference = Mathf.Abs(transform.eulerAngles.y - targetAngles.y);
            if (angleDifference <= 1f)
                isRotating = false;
        }
    }

    [PunRPC]
    void RPCRotateBackPanel()
    {
        if (!isRotating)
        {
            targetAngles = transform.eulerAngles + 180f * Vector3.up;
            if (targetAngles.y > 360)
                targetAngles.y -= 360;
            isRotating = true;
        }
    }

    public void p1_Resetting()
    {
        //p1_Reset_Light.GetComponent<Renderer>().material.color = Color.green;
        GameObject[] ResetLights = GameObject.FindGameObjectsWithTag("p1_Reset_Light");
        foreach(GameObject light in ResetLights)
        {
            light.GetComponent<Renderer>().material.color = Color.green;
        }
        photonView.RPC("RPC_p1_Resetting", PhotonTargets.Others);
    }

    public void p2_Resetting()
    {
        GameObject[] ResetLights = GameObject.FindGameObjectsWithTag("p2_Reset_Light");
        foreach (GameObject light in ResetLights)
        {
            light.GetComponent<Renderer>().material.color = Color.green;
        }
        photonView.RPC("RPC_p2_Resetting", PhotonTargets.Others);
    }

    public void p3_Resetting()
    {
        GameObject[] ResetLights = GameObject.FindGameObjectsWithTag("p3_Reset_Light");
        foreach (GameObject light in ResetLights)
        {
            light.GetComponent<Renderer>().material.color = Color.green;
        }
        photonView.RPC("RPC_p3_Resetting", PhotonTargets.Others);
    }

    public void p4_Resetting()
    {
        GameObject[] ResetLights = GameObject.FindGameObjectsWithTag("p4_Reset_Light");
        foreach (GameObject light in ResetLights)
        {
            light.GetComponent<Renderer>().material.color = Color.green;
        }
        photonView.RPC("RPC_p4_Resetting", PhotonTargets.Others);
    }

    public void p1_NotResetting()
    {
        //p1_Reset_Light.GetComponent<Renderer>().material.color = Color.red;
        GameObject[] ResetLights = GameObject.FindGameObjectsWithTag("p1_Reset_Light");
        foreach (GameObject light in ResetLights)
        {
            light.GetComponent<Renderer>().material.color = Color.red;
        }
        photonView.RPC("RPC_p1_NotResetting", PhotonTargets.Others);
    }

    public void p2_NotResetting()
    {
        GameObject[] ResetLights = GameObject.FindGameObjectsWithTag("p2_Reset_Light");
        foreach (GameObject light in ResetLights)
        {
            light.GetComponent<Renderer>().material.color = Color.red;
        }
        photonView.RPC("RPC_p2_NotResetting", PhotonTargets.Others);
    }

    public void p3_NotResetting()
    {
        GameObject[] ResetLights = GameObject.FindGameObjectsWithTag("p3_Reset_Light");
        foreach (GameObject light in ResetLights)
        {
            light.GetComponent<Renderer>().material.color = Color.red;
        }
        photonView.RPC("RPC_p3_NotResetting", PhotonTargets.Others);
    }

    public void p4_NotResetting()
    {
        GameObject[] ResetLights = GameObject.FindGameObjectsWithTag("p4_Reset_Light");
        foreach (GameObject light in ResetLights)
        {
            light.GetComponent<Renderer>().material.color = Color.red;
        }
        photonView.RPC("RPC_p4_NotResetting", PhotonTargets.Others);
    }

    public void p1_Aborting()
    {
        GameObject[] AbortLights = GameObject.FindGameObjectsWithTag("p1_Abort_Light");
        foreach (GameObject light in AbortLights)
        {
            light.GetComponent<Renderer>().material.color = Color.green;
        }
        photonView.RPC("RPC_p1_Aborting", PhotonTargets.Others);
    }

    public void p2_Aborting()
    {
        GameObject[] AbortLights = GameObject.FindGameObjectsWithTag("p2_Abort_Light");
        foreach (GameObject light in AbortLights)
        {
            light.GetComponent<Renderer>().material.color = Color.green;
        }
        photonView.RPC("RPC_p2_Aborting", PhotonTargets.Others);
    }

    public void p3_Aborting()
    {
        GameObject[] AbortLights = GameObject.FindGameObjectsWithTag("p3_Abort_Light");
        foreach (GameObject light in AbortLights)
        {
            light.GetComponent<Renderer>().material.color = Color.green;
        }
        photonView.RPC("RPC_p3_Aborting", PhotonTargets.Others);
    }

    public void p4_Aborting()
    {
        GameObject[] AbortLights = GameObject.FindGameObjectsWithTag("p4_Abort_Light");
        foreach (GameObject light in AbortLights)
        {
            light.GetComponent<Renderer>().material.color = Color.green;
        }
        photonView.RPC("RPC_p4_Aborting", PhotonTargets.Others);
    }

    public void p1_NotAborting()
    {
        GameObject[] AbortLights = GameObject.FindGameObjectsWithTag("p1_Abort_Light");
        foreach (GameObject light in AbortLights)
        {
            light.GetComponent<Renderer>().material.color = Color.red;
        }
        photonView.RPC("RPC_p1_NotAborting", PhotonTargets.Others);
    }

    public void p2_NotAborting()
    {
        GameObject[] AbortLights = GameObject.FindGameObjectsWithTag("p2_Abort_Light");
        foreach (GameObject light in AbortLights)
        {
            light.GetComponent<Renderer>().material.color = Color.red;
        }
        photonView.RPC("RPC_p2_NotAborting", PhotonTargets.Others);
    }

    public void p3_NotAborting()
    {
        GameObject[] AbortLights = GameObject.FindGameObjectsWithTag("p3_Abort_Light");
        foreach (GameObject light in AbortLights)
        {
            light.GetComponent<Renderer>().material.color = Color.red;
        }
        photonView.RPC("RPC_p3_NotAborting", PhotonTargets.Others);
    }

    public void p4_NotAborting()
    {
        GameObject[] AbortLights = GameObject.FindGameObjectsWithTag("p4_Abort_Light");
        foreach (GameObject light in AbortLights)
        {
            light.GetComponent<Renderer>().material.color = Color.red;
        }
        photonView.RPC("RPC_p4_NotAborting", PhotonTargets.Others);
    }

    [PunRPC]
    void RPC_p1_Resetting()
    {
        //p1_Reset_Light.GetComponent<Renderer>().material.color = Color.green;
        GameObject[] ResetLights = GameObject.FindGameObjectsWithTag("p1_Reset_Light");
        foreach (GameObject light in ResetLights)
        {
            light.GetComponent<Renderer>().material.color = Color.green;
        }
    }

    [PunRPC]
    void RPC_p2_Resetting()
    {
        GameObject[] ResetLights = GameObject.FindGameObjectsWithTag("p2_Reset_Light");
        foreach (GameObject light in ResetLights)
        {
            light.GetComponent<Renderer>().material.color = Color.green;
        }
    }

    [PunRPC]
    void RPC_p3_Resetting()
    {
        GameObject[] ResetLights = GameObject.FindGameObjectsWithTag("p3_Reset_Light");
        foreach (GameObject light in ResetLights)
        {
            light.GetComponent<Renderer>().material.color = Color.green;
        }
    }

    [PunRPC]
    void RPC_p4_Resetting()
    {
        GameObject[] ResetLights = GameObject.FindGameObjectsWithTag("p4_Reset_Light");
        foreach (GameObject light in ResetLights)
        {
            light.GetComponent<Renderer>().material.color = Color.green;
        }
    }

    [PunRPC]
    void RPC_p1_NotResetting()
    {
        //p1_Reset_Light.GetComponent<Renderer>().material.color = Color.red;
        GameObject[] ResetLights = GameObject.FindGameObjectsWithTag("p1_Reset_Light");
        foreach (GameObject light in ResetLights)
        {
            light.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    [PunRPC]
    void RPC_p2_NotResetting()
    {
        GameObject[] ResetLights = GameObject.FindGameObjectsWithTag("p2_Reset_Light");
        foreach (GameObject light in ResetLights)
        {
            light.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    [PunRPC]
    void RPC_p3_NotResetting()
    {
        GameObject[] ResetLights = GameObject.FindGameObjectsWithTag("p3_Reset_Light");
        foreach (GameObject light in ResetLights)
        {
            light.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    [PunRPC]
    void RPC_p4_NotResetting()
    {
        GameObject[] ResetLights = GameObject.FindGameObjectsWithTag("p4_Reset_Light");
        foreach (GameObject light in ResetLights)
        {
            light.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    [PunRPC]
    void RPC_p1_Aborting()
    {
        GameObject[] AbortLights = GameObject.FindGameObjectsWithTag("p1_Abort_Light");
        foreach (GameObject light in AbortLights)
        {
            light.GetComponent<Renderer>().material.color = Color.green;
        }
    }

    [PunRPC]
    void RPC_p2_Aborting()
    {
        GameObject[] AbortLights = GameObject.FindGameObjectsWithTag("p2_Abort_Light");
        foreach (GameObject light in AbortLights)
        {
            light.GetComponent<Renderer>().material.color = Color.green;
        }
    }

    [PunRPC]
    void RPC_p3_Aborting()
    {
        GameObject[] AbortLights = GameObject.FindGameObjectsWithTag("p3_Abort_Light");
        foreach (GameObject light in AbortLights)
        {
            light.GetComponent<Renderer>().material.color = Color.green;
        }
    }

    [PunRPC]
    void RPC_p4_Aborting()
    {
        GameObject[] AbortLights = GameObject.FindGameObjectsWithTag("p4_Abort_Light");
        foreach (GameObject light in AbortLights)
        {
            light.GetComponent<Renderer>().material.color = Color.green;
        }
    }

    [PunRPC]
    void RPC_p1_NotAborting()
    {
        GameObject[] AbortLights = GameObject.FindGameObjectsWithTag("p1_Abort_Light");
        foreach (GameObject light in AbortLights)
        {
            light.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    [PunRPC]
    void RPC_p2_NotAborting()
    {
        GameObject[] AbortLights = GameObject.FindGameObjectsWithTag("p2_Abort_Light");
        foreach (GameObject light in AbortLights)
        {
            light.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    [PunRPC]
    void RPC_p3_NotAborting()
    {
        GameObject[] AbortLights = GameObject.FindGameObjectsWithTag("p3_Abort_Light");
        foreach (GameObject light in AbortLights)
        {
            light.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    [PunRPC]
    void RPC_p4_NotAborting()
    {
        GameObject[] AbortLights = GameObject.FindGameObjectsWithTag("p4_Abort_Light");
        foreach (GameObject light in AbortLights)
        {
            light.GetComponent<Renderer>().material.color = Color.red;
        }
    }
}