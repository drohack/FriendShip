using UnityEngine;
using System.Collections;
using OvrTouch.Hands;
using UnityEngine.UI;

public class Highlight_Lobby_Row : MonoBehaviour
{

    private Color startcolor;
    private Color highlightColor = Color.yellow;
    private Text topRenderer;

    public bool isGrabbing = false;
    public bool isColliding = false;
    int numColliding = 0;

    // Use this for initialization
    void Start()
    {
        Transform topTransform = transform.GetChild(0);
        topRenderer = topTransform.GetComponent<Text>();
        startcolor = topRenderer.color;

        isGrabbing = false;
        isColliding = false;
        numColliding = 0;
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("collided with Row");
        if (col.tag.Equals("Hand"))
        {

            isColliding = true;
            numColliding++;
            topRenderer.color = highlightColor;
        }
    }

    void OnTriggerExit(Collider col)
    {
        numColliding--;
        if (numColliding == 0 && !isGrabbing)
        {
            isColliding = false;
            topRenderer.color = startcolor;
        }
    }

    private void OnGrabBegin(GrabbableGrabMsg grabMsg)
    {
        numColliding = 0;
        isGrabbing = true;
        topRenderer.color = highlightColor;
    }
    private void OnGrabEnd(GrabbableGrabMsg grabMsg)
    {
        isGrabbing = false;
        topRenderer.color = startcolor;
    }

   
    void Update()
    {
        if (isGrabbing && !transform.GetComponent<Grabbable>().isGrabbed)
        {
            isGrabbing = false;
            topRenderer.color = startcolor;
        }
    }
}
