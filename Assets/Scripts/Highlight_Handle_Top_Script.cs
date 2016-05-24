using UnityEngine;
using System.Collections;

public class Highlight_Handle_Top_Script : MonoBehaviour {

    private Color startcolor;
    private Color highlightColor = Color.yellow;
    private Renderer topRenderer;

    public bool isMouseOver = false;
    public bool isGrabbing = false;

    // Use this for initialization
    void Start () {
        Transform topTransform = transform.Find("Top");
        topRenderer = topTransform.GetComponent<Renderer>();
        isMouseOver = false;
        isGrabbing = false;
    }

    void OnMouseEnter()
    {
        isMouseOver = true;
        if (!Input.GetMouseButton(0))
        {
            startcolor = topRenderer.material.color;
            topRenderer.material.color = highlightColor;
        }
    }
    void OnMouseExit()
    {
        isMouseOver = false;
        if (!isGrabbing)
            topRenderer.material.color = startcolor;
    }
    void OnMouseUp()
    {
        isGrabbing = false;
        if (!isMouseOver)
            topRenderer.material.color = startcolor;
    }
    void OnMouseDown()
    {
        isGrabbing = true;
    }
    void Update()
    {
        if(isMouseOver && !Input.GetMouseButton(0))
        {
            topRenderer.material.color = highlightColor;
        }
    }
}
