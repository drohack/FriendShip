using UnityEngine;
using System.Collections;

public class Score_Text_Script : Photon.MonoBehaviour
{
    [PunRPC]
    public void Win(bool hasWon)
    {
        GetComponent<MeshRenderer>().material.color = Color.green;
        GetComponent<TextMesh>().text = "<b>Level Complete~</b>";
        GetComponent<TextMesh>().color = Color.green;
    }

    [PunRPC]
    public void GameOver(bool hasWon)
    {
        GetComponent<MeshRenderer>().material.color = Color.red;
        GetComponent<TextMesh>().text = "<b>GAME OVER</b>";
        GetComponent<TextMesh>().color = Color.red;
    }

    [PunRPC]
    public void UpdateScore(int level, int score, int scoreToWin)
    {
        float percent = (float)score / scoreToWin;
        int count = 0;

        string text = "Level = " + level + " | [";

        if (percent > 0)
        {
            text += "<color=#001900>■</color>";
            count++;
        }
        if (percent >= 0.15)
        { 
            text += "<color=#003300>■</color>";
            count++;
        }
        if (percent >= 0.3)
        {
            text += "<color=#004C00>■</color>";
           count++;
        }
        if (percent >= 0.4)
        {
            text += "<color=#006600>■</color>";
            count++;
        }
        if (percent >= 0.5)
        {
            text += "<color=#007F00>■</color>";
            count++;
        }
        if (percent >= 0.6)
        {
            text += "<color=#009900>■</color>";
            count++;
        }
        if (percent >= 0.7)
        {
            text += "<color=#00B200>■</color>";
            count++;
        }
        if (percent >= 0.8)
        {
            text += "<color=#00E500>■</color>";
            count++;
        }
        if (percent >= 0.9)
        {
            text += "<color=#00FF00>■</color>";
            count++;
        }
        if (percent >= 0.98)
        {
            text += "<color=#00FF00>■</color>";
            count++;
        }

        while (count <= 9)
        {
            text += "_";
            count++;
        }

        text += "]";

        GetComponent<TextMesh>().text = text;
        GetComponent<TextMesh>().color = Color.black;
        GetComponent<MeshRenderer>().material.color = Color.black;
    }
}
