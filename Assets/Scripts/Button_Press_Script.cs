using UnityEngine;
using System.Collections;

public class Button_Press_Script : MonoBehaviour {

    private Color startcolor;
    private Renderer buttonTopRenderer;
    Animator anim;

    public int rCommand = -1;

    Mastermind_Script mastermindScript;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        GameObject buttonTopObj = getChildGameObject("Top");
        buttonTopRenderer = buttonTopObj.GetComponent<Renderer>();
        mastermindScript = GameObject.Find("Mastermind").GetComponent<Mastermind_Script>();
    }

    private GameObject getChildGameObject(string withName)
    {
        GameObject childObj = null;
        foreach (Transform child in transform)
        {
            if (child.name == withName)
                childObj = child.gameObject;
        }
        return childObj;
    }

    void OnMouseEnter()
    {
        startcolor = buttonTopRenderer.material.color;
        buttonTopRenderer.material.color = Color.yellow;
    }
    void OnMouseExit()
    {
        buttonTopRenderer.material.color = startcolor;
    }

    void OnMouseDown()
    {
        //Check to see if the command is to press the button (rCommand == 0)
        //send command tapped to the Console_Text_Script
        mastermindScript.TappedWaitForSecondsOrTap(rCommand);

        anim.Play("Button_Press_Anim");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
