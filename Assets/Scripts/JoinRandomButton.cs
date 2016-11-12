using UnityEngine;
using System.Collections;

public class JoinRandomButton : Photon.MonoBehaviour
{

    private Highlight_Handle_Top_Script handleScript;
    Animator anim;
    private bool isButtonDown = false;
    private bool isAnimating = false;
    private bool isLocked = false;


    // Use this for initialization
    void Start()
    {
        //Load Network data

        handleScript = transform.Find("Handle").GetComponent<Highlight_Handle_Top_Script>();
        anim = transform.Find("Handle").GetComponent<Animator>();
        isButtonDown = false;
        isAnimating = false;
        isLocked = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (!isAnimating && isButtonDown && isLocked && !handleScript.isGrabbing && !handleScript.isColliding)
        {
            isLocked = false;
            isButtonDown = false;
#if OCULUS
            //Save your OVRCameraRig to be added to your PlayerObject
            Transform ovrCameraRig = GameObject.FindGameObjectWithTag("Player").transform.Find("OVRCameraRig");
            ovrCameraRig.parent = null;
            DontDestroyOnLoad(ovrCameraRig);
#endif
            PhotonNetwork.JoinRandomRoom();
            StartCoroutine(WaitForAnimation(anim, "Button_Up_Anim"));
        }

        if (!isAnimating && !isLocked && !isButtonDown && (handleScript.isGrabbing || handleScript.isColliding))
        {
            isLocked = true;
            isButtonDown = true;
            Debug.Log("Pushed Button");
            StartCoroutine(WaitForAnimation(anim, "Button_Down_Anim"));
        }
    }

    private IEnumerator WaitForAnimation(Animator animation, string animationName)
    {
        isAnimating = true;
        animation.Play(animationName);
        do
        {
            yield return null;
        } while (animation.GetCurrentAnimatorStateInfo(0).IsName(animationName) && !animation.IsInTransition(0));

        isAnimating = false;
    }
}
