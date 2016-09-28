using UnityEngine;
using System.Collections;
using OvrTouch.Hands;

public class Highlight_Handle_Top_Script : MonoBehaviour {

    private Color startcolor;
    private Color highlightColor = Color.yellow;
    private Renderer topRenderer;
    
    public bool isGrabbing = false;
    public bool isColliding = false;
    int numColliding = 0;

    // Use this for initialization
    void Start () {
        Transform topTransform = transform.Find("Top");
        topRenderer = topTransform.GetComponent<Renderer>();
        startcolor = topRenderer.material.color;
        isGrabbing = false;
        isColliding = false;
        numColliding = 0;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag.Equals("Hand"))
        {
            isColliding = true;
            numColliding++;
            topRenderer.material.color = highlightColor;

            PhotonView photonView = null;
            if (GetComponent<PhotonView>() != null)
            {
                photonView = GetComponent<PhotonView>();
            }
            else if (transform.parent != null && transform.parent.GetComponent<PhotonView>() != null)
            {
                photonView = transform.parent.GetComponent<PhotonView>();
            }

            if (photonView != null)
            {
                if (photonView.ownerId != PhotonNetwork.player.ID)
                {
                    photonView.RequestOwnership();
                }
            }
        }
    }
    void OnTriggerExit(Collider col)
    {
        numColliding--;
        if (numColliding == 0 && !isGrabbing)
        {
            isColliding = false;
            topRenderer.material.color = startcolor;
        }
    }

    private void OnGrabBegin(GrabbableGrabMsg grabMsg)
    {
        numColliding = 0;
        isGrabbing = true;
        topRenderer.material.color = highlightColor;
    }
    private void OnGrabEnd(GrabbableGrabMsg grabMsg)
    {
        isGrabbing = false;
        topRenderer.material.color = startcolor;
    }

    void Update()
    {

        if(isGrabbing && !transform.GetComponent<Grabbable>().isGrabbed)
        {
            isGrabbing = false;
            topRenderer.material.color = startcolor;
        }
    }
}
