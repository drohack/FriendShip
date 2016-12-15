using UnityEngine;
using System.Collections;

public class Timer_Script : Photon.MonoBehaviour
{

    [SerializeField]
    GameObject fill;

    [SerializeField]
    GameObject fillMaterialObject;

    private Material fillMaterial;
    private Vector3 fillOriginalLocalScale;
    private bool isRunning = false;
    private float levelTimeoutSeconds = 100f;
    private System.DateTime levelStartTime = System.DateTime.Now;
    private float fillAmount = 1.0f;
    private double secondsDifference = 0d;

    // Use this for initialization
    void Start()
    {
        fillMaterial = fillMaterialObject.GetComponent<Renderer>().material;
        fillMaterial.color = Color.green;
        fillOriginalLocalScale = fill.transform.localScale;
    }

    public void StartTimer(float levelTimeoutSeconds, System.DateTime levelStartTime)
    {
        //Reset timer
        fillMaterial.color = Color.green;
        fill.transform.localScale = fillOriginalLocalScale;

        //Start new timer
        this.levelTimeoutSeconds = levelTimeoutSeconds;
        this.levelStartTime = levelStartTime;
        isRunning = true;
        photonView.RPC("RPCStartTimer", PhotonTargets.Others, levelTimeoutSeconds, levelStartTime.ToBinary());
    }

    public void StopTimer()
    {
        isRunning = false;
        photonView.RPC("RPCStopTimer", PhotonTargets.Others);
    }

    // Update is called once per frame
    void Update()
    {
        //If the timer has started reduce the FILL poportionally to time passed
        if (isRunning)
        {
            //Stop reducing the fill if time runs out
            System.DateTime now = System.DateTime.Now;
            if (levelStartTime.AddSeconds(levelTimeoutSeconds) > now)
            {
                //Set fillAmmount by getting the percent of time passed vs levelTimeoutSeconds
                secondsDifference = (now - levelStartTime).TotalSeconds;
                fillAmount = (float)(1f - (secondsDifference / levelTimeoutSeconds));
                if (fillAmount < 0)
                    fillAmount = 0f;
                fill.transform.localScale = Vector3.Lerp(fillOriginalLocalScale, new Vector3(fillOriginalLocalScale.x, 0.0f, fillOriginalLocalScale.z), (float)(secondsDifference / levelTimeoutSeconds));
                fillMaterial.color = Color.Lerp(Color.green, Color.red, (float)(secondsDifference / levelTimeoutSeconds));
            }
        }
    }

    [PunRPC]
    void RPCStartTimer(float levelTimeoutSeconds, long levelStartTimeBinary)
    {
        this.levelTimeoutSeconds = levelTimeoutSeconds;
        this.levelStartTime = System.DateTime.FromBinary(levelStartTimeBinary);
        isRunning = true;
    }

    [PunRPC]
    void RPCStopTimer(bool isGameOver)
    {
        isRunning = false;
        if (!isGameOver)
        {
            fillMaterial.color = Color.green;
            fill.transform.localScale = fillOriginalLocalScale;
        }
    }
}
