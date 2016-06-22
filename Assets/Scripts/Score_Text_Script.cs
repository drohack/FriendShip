using UnityEngine;
using System.Collections;

public class Score_Text_Script : Photon.MonoBehaviour
{
    [PunRPC]
    public void Win(bool hasWon)
    {
        GetComponent<MeshRenderer>().material.color = Color.green;
        GetComponent<TextMesh>().text = "<b>YOU WIN~</b>";
        GetComponent<TextMesh>().color = Color.green;
    }

    [PunRPC]
    public void ScoreUp (int score)
    {
        GetComponent<TextMesh>().text = "Score = " + score;
        GetComponent<MeshRenderer>().material.color = Color.black;
    }

    [PunRPC]
    public void ScoreDown(int score)
    {
        GetComponent<TextMesh>().text = "Score = " + score;
        GetComponent<MeshRenderer>().material.color = Color.red;
    }
}
