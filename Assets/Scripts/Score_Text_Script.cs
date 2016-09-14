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
    public void GameOver(bool hasWon)
    {
        GetComponent<MeshRenderer>().material.color = Color.red;
        GetComponent<TextMesh>().text = "<b>GAME OVER</b>";
        GetComponent<TextMesh>().color = Color.red;
    }

    [PunRPC]
    public void UpdateScore(int level, int score)
    {
        GetComponent<TextMesh>().text = "Level = " + level + " | Score = " + score;
        GetComponent<MeshRenderer>().material.color = Color.black;
    }
}
