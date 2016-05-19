using UnityEngine;
using System.Collections;

public class W_Lever_Script : MonoBehaviour {

    public bool isWLeverUp;
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        isWLeverUp = true;
    }

    void OnMouseDown()
    {
        //Check to see if the command is to pull the W_Lever (rCommand == 2)
        GameObject consoleText = GameObject.Find("Console_Text");
        Console_Text_Script consoleTextScript = consoleText.GetComponent<Console_Text_Script>();
        //send command tapped to the Console_Text_Script
        consoleTextScript.tappedWaitForSecondsOrTap(Console_Text_Script.wLeverCommand);

        if (isWLeverUp)
        {
            anim.Play("W_Lever_Down_Anim");
            isWLeverUp = false;
        }
        else
        {
            anim.Play("W_Lever_Up_Anim");
            isWLeverUp = true;
        }
    }
}
