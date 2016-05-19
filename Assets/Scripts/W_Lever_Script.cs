using UnityEngine;
using System.Collections;

public class W_Lever_Script : MonoBehaviour {

    public bool isWLeverUp;
    Animator anim;

    Mastermind_Script mastermindScript;

    void Start()
    {
        anim = GetComponent<Animator>();
        isWLeverUp = true;
        mastermindScript = GameObject.Find("Mastermind").GetComponent<Mastermind_Script>();
    }

    void OnMouseDown()
    {
        //Check to see if the command is to pull the W_Lever (rCommand == 2)
        //send command tapped to the Console_Text_Script
        mastermindScript.TappedWaitForSecondsOrTap(Mastermind_Script.wLeverCommand);

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
