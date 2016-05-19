using UnityEngine;
using System.Collections;

public class Score_Text_Script : MonoBehaviour {

    private int score = 0;

	// Use this for initialization
	void Start () {
        score = 0;
    }
	
	// Update is called once per frame
	void Update () {
        //Quit if the user presses the esc key
        if (Input.GetKey("escape"))
            Application.Quit();

        //If the user scores 10 or more, change text to Green and to say "YOU WIN~"
        if (score >= 10)
        {
            GetComponent<MeshRenderer>().material.color = Color.green;
            GetComponent<TextMesh>().text = "<b>YOU WIN~</b>";
            GetComponent<TextMesh>().color = Color.green;
        }
	}

    public void scoreUp ()
    {
        score++;
        GetComponent<TextMesh>().text = "Score = " + score;
        GetComponent<MeshRenderer>().material.color = Color.black;
    }

    public void scoreDown()
    {
        score--;
        GetComponent<TextMesh>().text = "Score = " + score;
        GetComponent<MeshRenderer>().material.color = Color.red;
    }
}
