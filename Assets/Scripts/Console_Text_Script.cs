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
        message = message.Replace("  ", " ");

        //Split message into 2 lines if needed
        if (message.Length > rowLimit)
        {
            string[] parts = message.Split(' ');
            message = "";
            int currentLineLength = 0;
            for (int i = 0; i < parts.Length; i++)
            {
                if (!parts[i].Equals(""))
                {
                    //Add space if nessesary
                    if (currentLineLength > 0 && i != parts.Length && (currentLineLength + 1) <= rowLimit)
                    {
                        message += " ";
                        currentLineLength += 1;
                    }

                    if (parts[i].Equals("\n"))
                    {
                        //Add new line here
                        message = message.TrimEnd(' ');
                        message += System.Environment.NewLine;
                        currentLineLength = 0;
                    }
                    else if (parts[i].Equals("[̲̅$̲̅(̲̅1̲̅)̲̅$̲̅]") || parts[i].Equals("[̲̅$̲̅(̲̅5̲̅)̲̅$̲̅]"))
                    {
                        //Treat $1 and $5 as 7 characters (instead of their actual 19 character)
                        if ((currentLineLength + 7) > rowLimit)
                        {
                            message = message.TrimEnd(' ');
                            message += System.Environment.NewLine;
                            currentLineLength = 0;
                        }
                        message += parts[i];
                        currentLineLength += 7;
                    }
                    else if (parts[i].Equals("[̲̅$̲̅(̲̅ιοο̲̅)̲̅$̲̅]"))
                    {
                        //Treat $100 as 9 characters (instead of it's actual 21 characters)
                        if ((currentLineLength + 9) > rowLimit)
                        {
                            message = message.TrimEnd(' ');
                            message += System.Environment.NewLine;
                            currentLineLength = 0;
                        }
                        message += parts[i];
                        currentLineLength += 9;
                    }
                    else if (parts[i].Equals("♫♪..|̲̅̅●̲̅̅|̲̅̅=̲̅̅|̲̅̅●̲̅̅|..♫♪"))
                    {
                        //Treat the Boombox as 15 characters (instead of it's actual 33 characters)
                        if ((currentLineLength + 15) > rowLimit)
                        {
                            message = message.TrimEnd(' ');
                            message += System.Environment.NewLine;
                            currentLineLength = 0;
                        }
                        message += parts[i];
                        currentLineLength += 15;
                    }
                    else
                    {
                        if ((currentLineLength + parts[i].Length) > rowLimit)
                        {
                            message = message.TrimEnd(' ');
                            message += System.Environment.NewLine;
                            currentLineLength = 0;
                        }
                        message += parts[i];
                        currentLineLength += parts[i].Length;
                    }
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
