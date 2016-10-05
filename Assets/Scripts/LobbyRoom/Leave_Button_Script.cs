using UnityEngine;
using System.Collections;

public class Leave_Button_Script : Photon.MonoBehaviour {

    [SerializeField]
    Highlight_Handle_Top_Script handleScript;
    [SerializeField]
    Animator anim;
    public bool isButtonDown = false;
    private bool isAnimating = false;
    private bool isLocked = false;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (!isAnimating && isButtonDown && isLocked && !handleScript.isGrabbing && !handleScript.isColliding)
        {
            isLocked = false;
            isButtonDown = false;
            photonView.RPC("RPCPlayAnim", PhotonTargets.Others, "Button_Up_Anim");
            StartCoroutine(WaitForAnimation(anim, "Button_Up_Anim"));
        }

        if (!isAnimating && !isLocked && !isButtonDown && (handleScript.isGrabbing || handleScript.isColliding))
        {
            isLocked = true;
            isButtonDown = true;
            //send tapped rCommand to Server
            photonView.RPC("RPCPlayAnim", PhotonTargets.Others, "Button_Down_Anim");
            StartCoroutine(WaitForAnimation(anim, "Button_Down_Anim"));
            PhotonNetwork.LeaveRoom();  // we will load the menu level when we successfully left the room
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

    [PunRPC]
    void RPCDestroy()
    {
        if (photonView.isMine)
            PhotonNetwork.Destroy(gameObject);
    }
}
