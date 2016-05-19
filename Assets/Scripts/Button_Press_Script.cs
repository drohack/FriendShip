using UnityEngine;
using System.Collections;

public class Button_Press_Script : MonoBehaviour {

    private Color startcolor;
    private Renderer buttonTopRenderer;
    Animator anim;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        GameObject buttonTopObj = GameObject.Find("Button/Top/Sphere_001_MeshPart0");
        buttonTopRenderer = buttonTopObj.GetComponent<Renderer>();
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
        GameObject consoleText = GameObject.Find("Console_Text");
        Console_Text_Script consoleTextScript = consoleText.GetComponent<Console_Text_Script>();
        //send command tapped to the Console_Text_Script
        consoleTextScript.tappedWaitForSecondsOrTap(Console_Text_Script.buttonCommand);

        anim.Play("Button_Press_Anim");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
