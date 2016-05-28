using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Score_Text_Script : NetworkBehaviour {

    [SyncVar(hook = "ScoreUp")]
    public int scoreUp;
    [SyncVar(hook = "ScoreDown")]
    public int scoreDown;
    [SyncVar(hook = "Win")]
    public bool hasWon;

    public void Win(bool hasWon)
    {
        GetComponent<MeshRenderer>().material.color = Color.green;
        GetComponent<TextMesh>().text = "<b>YOU WIN~</b>";
        GetComponent<TextMesh>().color = Color.green;
    }

    public void ScoreUp (int score)
    {
        GetComponent<TextMesh>().text = "Score = " + score;
        GetComponent<MeshRenderer>().material.color = Color.black;
    }

    public void ScoreDown(int score)
    {
        GetComponent<TextMesh>().text = "Score = " + score;
        GetComponent<MeshRenderer>().material.color = Color.red;
    }
}
