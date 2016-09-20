using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Console_Text_Script : Photon.MonoBehaviour
{

    private TextMesh textMesh;
    public bool isTyping = false;
    private const int rowLimit = 20;

    // Use this for initialization
    void Start ()
    {
        textMesh = GetComponent<TextMesh>();
        textMesh.text = "";
    }

    [PunRPC]
    public void RpcTypeText(string message)
    {
        StopCoroutine("TypeText");
        StartCoroutine("TypeText", message);
    }

    //Type out the text that is loaded into the "message" variable
    public IEnumerator TypeText(string message)
    {
        isTyping = true;
        textMesh = GetComponent<TextMesh>();
        textMesh.text = "";

        //Remove all new line characters from message as new ones will be added if needed
        message = message.Replace(System.Environment.NewLine, "");

        //Split message into 2 lines if needed
        if (message.Length > rowLimit)
        {
            string[] parts = message.Split(' ');
            message = "";
            int currentLineLength = 0;
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts.Equals("[̲̅$̲̅(̲̅1̲̅)̲̅$̲̅]") || parts.Equals("[̲̅$̲̅(̲̅5̲̅)̲̅$̲̅]"))
                {
                    //Treat $1 and $5 as 7 characters (instead of their actual 19 character)
                    if ((currentLineLength + 1 + 7) > rowLimit)
                    {
                        message += System.Environment.NewLine;
                        currentLineLength = 0;
                    }
                    message += " " + parts[i];
                    currentLineLength += 7;
                }
                else if (parts.Equals("[̲̅$̲̅(̲̅ιοο̲̅)̲̅$̲̅]"))
                {
                    //Treat $100 as 9 characters (instead of it's actual 21 characters)
                    if ((currentLineLength + 1 + 9) > rowLimit)
                    {
                        message += System.Environment.NewLine;
                        currentLineLength = 0;
                    }
                    message += " " + parts[i];
                    currentLineLength += 9;
                }
                else if (parts.Equals("♫♪..|̲̅̅●̲̅̅|̲̅̅=̲̅̅|̲̅̅●̲̅̅|..♫♪"))
                {
                    //Treat the Boombox as 15 characters (instead of it's actual 33 characters)
                    if ((currentLineLength + 1 + 15) > rowLimit)
                    {
                        message += System.Environment.NewLine;
                        currentLineLength = 0;
                    }
                    message += " " + parts[i];
                    currentLineLength += 15;
                }
                else
                {
                    if ((currentLineLength + 1 + parts[i].Length) > rowLimit)
                    {
                        message += System.Environment.NewLine;
                        currentLineLength = 0;
                    }
                    message += " " + parts[i];
                    currentLineLength += parts[i].Length;
                }
            }
        }

        //Type each character with a 0.05 second delay between characters
        foreach (char letter in message.ToCharArray())
        {
            textMesh.text += letter;
            yield return new WaitForSeconds(0.035f);
        }
        isTyping = false;
    }

}
