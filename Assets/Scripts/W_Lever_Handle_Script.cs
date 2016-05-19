using UnityEngine;
using System.Collections;

public class W_Lever_Handle_Script : MonoBehaviour {

    private HingeJoint m_HingeJoint;

    const int upPosition = 1;
    const int middlePosition = 0;
    const int downPosition = -1;

    private int lastHandlePosition;

    public bool isWLeverUp = true;

    private Color startcolor;
    private Renderer wLeverTopRenderer;
    private bool isMouseOver = false;

    void Start()
    {
        m_HingeJoint = GetComponent<HingeJoint>();
        isWLeverUp = true;
        lastHandlePosition = upPosition;
        GameObject wLeverTopObj = GameObject.Find("W_Lever/Handle/Top");
        wLeverTopRenderer = wLeverTopObj.GetComponent<Renderer>();
        startcolor = wLeverTopRenderer.material.color;
    }

    void OnMouseEnter()
    {
        isMouseOver = true;
        wLeverTopRenderer.material.color = Color.yellow;
    }
    void OnMouseExit()
    {
        isMouseOver = false;
        if (!Input.GetMouseButton(0))
            wLeverTopRenderer.material.color = startcolor;
    }
    void OnMouseUp()
    {
        if (!isMouseOver)
            wLeverTopRenderer.material.color = startcolor;
    }

    private void Update()
    {
        //snap lever into place near edges (on = transform.eulerAngles.x == 0; off = transform.eulerAngles.x == 35)
        if (transform.eulerAngles.x < 1.5)
        {
            transform.eulerAngles = new Vector3(
                0,
                transform.eulerAngles.y,
                transform.eulerAngles.z
            );

            //If the last position of the handle was in the middle, and now we are at the up position, then send the command that the W_Lever is now Up
            if (lastHandlePosition == middlePosition)
            {
                //send command tapped to the Console_Text_Script with wLeverUpCommand
                GameObject consoleText = GameObject.Find("Console_Text");
                Console_Text_Script consoleTextScript = consoleText.GetComponent<Console_Text_Script>();
                consoleTextScript.tappedWaitForSecondsOrTap(Console_Text_Script.wLeverUpCommand);
                //Lever changed positions
                isWLeverUp = true;
            }

            //update last handle position
            lastHandlePosition = upPosition;
        }
        else if (transform.eulerAngles.x > 33.5)
        {
            transform.eulerAngles = new Vector3(
                35,
                transform.eulerAngles.y,
                transform.eulerAngles.z
            );

            //If the last position of the handle was in the middle, and now we are at the down position, then send the command that the W_Lever is now Down
            if (lastHandlePosition == middlePosition)
            {
                //send command tapped to the Console_Text_Script with wLeverDownCommand
                GameObject consoleText = GameObject.Find("Console_Text");
                Console_Text_Script consoleTextScript = consoleText.GetComponent<Console_Text_Script>();
                consoleTextScript.tappedWaitForSecondsOrTap(Console_Text_Script.wLeverDownCommand);
                //Lever changed positions
                isWLeverUp = false;
            }

            //update last handle position
            lastHandlePosition = downPosition;
        }
        else
        {
            lastHandlePosition = middlePosition;
        }

        //push lever in direction to go towards edges
        if (transform.eulerAngles.x < 17.5)
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
