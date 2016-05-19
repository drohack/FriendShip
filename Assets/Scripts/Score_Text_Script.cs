using UnityEngine;
using System.Collections;

public class Score_Text_Script : MonoBehaviour {

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        
	}

    public void Win()
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
