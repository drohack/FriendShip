using UnityEngine;
using System.Collections;

public class Canvas_Text_Script : MonoBehaviour {

    private int score = 0;

	// Use this for initialization
	void Start () {
        score = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey("escape"))
            Application.Quit();

        if (score >= 10)
        {
            //Material mat = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Fonts/FONT_3D_CANVAS_MTL.mat", typeof(Material)) as Material;
            //GetComponent<MeshRenderer>().material = mat;
            GetComponent<MeshRenderer>().material.color = Color.green;
            GetComponent<TextMesh>().text = "<b><color=#008000ff>YOU WIN~</color></b>";
            GetComponent<TextMesh>().color = Color.green;
        }
	}

    public void scoreUp ()
    {
        score++;
        GetComponent<TextMesh>().text = "Score = " + score;
    }

    public void scoreDown()
    {
        score--;
        GetComponent<TextMesh>().text = "Score = <color=#ff0000ff>" + score + "</color>";
    }
}
