using UnityEngine;
using System.Collections;

public class Command_Array : MonoBehaviour {
    private const string button1Text = "Button1";
    private const string button2Text = "Button2";
    private const string button3Text = "Button3";
    private const string button4Text = "Button4";
    private const string button5Text = "Button5";
    private const string button6Text = "Button6";
    private const string button7Text = "Button7";
    private const string button8Text = "Button8";
    private const string button9Text = "Button9";
    private const string lLever1Text = "L 1";
    private const string lLever2Text = "L 2";
    private const string lLever3Text = "L 3";
    private const string lLever4Text = "L 4";
    private const string lLever5Text = "L 5";
    private const string lLever6Text = "L 6";
    private const string lLever7Text = "L 7";
    private const string lLever8Text = "L 8";
    private const string lLever9Text = "L 9";
    private const string wLever1Text = "W 1";
    private const string wLever2Text = "W 2";
    private const string wLever3Text = "W 3";
    private const string wLever4Text = "W 4";
    private const string wLever5Text = "W 5";
    private const string wLever6Text = "W 6";
    private const string wLever7Text = "W 7";
    private const string wLever8Text = "W 8";
    private const string wLever9Text = "W 9";

    public ArrayList buttonCommandArray = new ArrayList { button1Text, button2Text, button3Text, button4Text, button5Text, button6Text, button7Text, button8Text, button9Text };
    public ArrayList lLeverCommandArray = new ArrayList { lLever1Text, lLever2Text, lLever3Text, lLever4Text, lLever5Text, lLever6Text, lLever7Text, lLever8Text, lLever9Text };
    public ArrayList wLeverCommandArray = new ArrayList { wLever1Text, wLever2Text, wLever3Text, wLever4Text, wLever5Text, wLever6Text, wLever7Text, wLever8Text, wLever9Text };
}
