using UnityEngine;
using System.Collections;

public class L_Lever_Script : MonoBehaviour {

    public bool isLLeverUp;
    Animator anim;

    Mastermind_Script mastermindScript;

    void Start ()
    {
        anim = GetComponent<Animator>();
        isLLeverUp = true;
        mastermindScript = GameObject.Find("Mastermind").GetComponent<Mastermind_Script>();
    }

	void OnMouseDown()
    {
        //Check to see if the command is to pull the L_Lever (rCommand == 1)
        //send command tapped to the Console_Text_Script
        mastermindScript.TappedWaitForSecondsOrTap(Mastermind_Script.lLeverCommand);

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
