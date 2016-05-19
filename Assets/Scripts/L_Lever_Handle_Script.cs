using UnityEngine;
using System.Collections;

public class L_Lever_Handle_Script : MonoBehaviour {

    private HingeJoint m_HingeJoint;

    const int upPosition = 1;
    const int middlePosition = 0;
    const int downPosition = -1;
    
    private int lastHandlePosition;

    public bool isLLeverUp;

    private Color startcolor;
    private Renderer lLeverTopRenderer;
    private bool isMouseOver = false;

    void Start()
    {
        m_HingeJoint = GetComponent<HingeJoint>();
        isLLeverUp = true;
        lastHandlePosition = upPosition;
        GameObject lLeverTopObj = GameObject.Find("L_Lever/Handle/Top");
        lLeverTopRenderer = lLeverTopObj.GetComponent<Renderer>();
        isMouseOver = false;
        startcolor = lLeverTopRenderer.material.color;
    }

    void OnMouseEnter()
    {
        isMouseOver = true;
        lLeverTopRenderer.material.color = Color.yellow;
    }
    void OnMouseExit()
    {
        isMouseOver = false;
        if(!Input.GetMouseButton(0))
            lLeverTopRenderer.material.color = startcolor;
    }
    void OnMouseUp()
    {
        if(!isMouseOver)
            lLeverTopRenderer.material.color = startcolor;
    }

    private void Update()
    {
        //snap lever into place near edges (on = transform.eulerAngles.x == 0; off = transform.eulerAngles.x == 45)
        if (transform.eulerAngles.x < 1.5)
        {
            transform.eulerAngles = new Vector3(
                0,
                transform.eulerAngles.y,
                transform.eulerAngles.z
            );

            //If the last position of the handle was in the middle, and now we are at the up position, then send the command that the L_Lever is now Up
            if(lastHandlePosition == middlePosition)
            {
                //send command tapped to the Console_Text_Script with the lLeverUpCommand
                GameObject consoleText = GameObject.Find("Console_Text");
                Console_Text_Script consoleTextScript = consoleText.GetComponent<Console_Text_Script>();
                consoleTextScript.tappedWaitForSecondsOrTap(Console_Text_Script.lLeverUpCommand);
                //Lever changed positions
                isLLeverUp = true;
            }

            //update last handle position
            lastHandlePosition = upPosition;
        }
        else if (transform.eulerAngles.x > 43.5)
        {
            transform.eulerAngles = new Vector3(
                45,
                transform.eulerAngles.y,
                transform.eulerAngles.z
            );

            //If the last position of the handle was in the middle, and now we are at the down position, then send the command that the L_Lever is now Down
            if (lastHandlePosition == middlePosition)
            {
                //send command tapped to the Console_Text_Script with the lLeverDownCommand
                GameObject consoleText = GameObject.Find("Console_Text");
                Console_Text_Script consoleTextScript = consoleText.GetComponent<Console_Text_Script>();
                consoleTextScript.tappedWaitForSecondsOrTap(Console_Text_Script.lLeverDownCommand);
                //Lever changed positions
                isLLeverUp = false;
            }

            //update last handle position
            lastHandlePosition = downPosition;
        }
        else
        {
            lastHandlePosition = middlePosition;
        }

        //push lever in direction to go towards edges
        if (transform.eulerAngles.x < 22.5)
        {
            JointMotor motor = m_HingeJoint.motor;
            motor.targetVelocity = 20;
            m_HingeJoint.motor = motor;
        }
        else
        {
            JointMotor motor = m_HingeJoint.motor;
            motor.targetVelocity = -20;
            m_HingeJoint.motor = motor;
        }
    }
}
