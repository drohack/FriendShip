using UnityEngine;
using System.Collections;

public class L_Lever_Script : MonoBehaviour {

    public bool isLLeverUp;
    Animator anim;

    void Start ()
    {
        anim = GetComponent<Animator>();
        isLLeverUp = true;
    }

	void OnMouseDown()
    {
        //Check to see if the command is to pull the L_Lever (rCommand == 1)
        GameObject consoleText = GameObject.Find("Console_Text");
        Console_Text_Script consoleTextScript = consoleText.GetComponent<Console_Text_Script>();
        //send command tapped to the Console_Text_Script
        consoleTextScript.tappedWaitForSecondsOrTap(Console_Text_Script.lLeverCommand);

        if (isLLeverUp)
        {
            anim.Play("L_Lever_Down_Anim");
            isLLeverUp = false;
        }
        else
        {
            anim.Play("L_Lever_Up_Anim");
            isLLeverUp = true;
        }
    }
}
