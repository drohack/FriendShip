using UnityEngine;
using System.Collections;

public class Console_Timer_Script : Photon.MonoBehaviour
{

    [SerializeField]
    GameObject fill;

    private Material fillMaterial;
    private Vector3 fillOriginalLocalScale;
    private bool isRunning = false;
    private float commandTimeoutSeconds = 10f;
    private float secondsDifference = 10f;
    private float fillAmount = 1.0f;

    // Use this for initialization
    void Start()
    {
        fillMaterial = fill.GetComponent<Renderer>().material;
        fillMaterial.color = Color.grey;
        fillOriginalLocalScale = fill.transform.localScale;
    }

    public void StartTimer(float commandTimeoutSeconds)
    {
        this.commandTimeoutSeconds = commandTimeoutSeconds;
        secondsDifference = commandTimeoutSeconds;
        isRunning = true;
        photonView.RPC("RPCStartTimer", PhotonTargets.Others, commandTimeoutSeconds);
    }

    public void StopTimer(bool isCommandver)
    {
        isRunning = false;
        if (!isCommandver)
        {
            fillMaterial.color = Color.grey;
            fill.transform.localScale = fillOriginalLocalScale;
        }
        photonView.RPC("RPCStopTimer", PhotonTargets.Others, isCommandver);
    }

    // Update is called once per frame
    void Update()
    {
        //If the timer has started reduce the FILL poportionally to time passed
        if (isRunning)
        {
            //Stop reducing the fill if time runs out
            if (secondsDifference > 0)
            {
                //reduce command time by time difference
                secondsDifference -= Time.deltaTime;
                fill.transform.localScale = Vector3.Lerp(fillOriginalLocalScale, new Vector3(fillOriginalLocalScale.x, 0.0f, fillOriginalLocalScale.z), 1f - (secondsDifference / commandTimeoutSeconds));
                fillMaterial.color = Color.Lerp(Color.grey, Color.red, 1f - (secondsDifference / commandTimeoutSeconds));
            }
        }
    }

    [PunRPC]
    void RPCStartTimer(float commandTimeoutSeconds)
    {
        this.commandTimeoutSeconds = commandTimeoutSeconds;
        secondsDifference = commandTimeoutSeconds;
        isRunning = true;
    }

    [PunRPC]
    void RPCStopTimer(bool isCommandOver)
    {
        isRunning = false;
        if (!isCommandOver)
        {
            fillMaterial.color = Color.grey;
            fill.transform.localScale = fillOriginalLocalScale;
        }
    }
}
