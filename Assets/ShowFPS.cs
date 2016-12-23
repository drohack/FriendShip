using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowFPS : MonoBehaviour {

    public Text text;
    private const int targetFPS = 85;
    float deltaTime = 0.0f;
    float msec;
    float fps;
    string textFPS;

    // Use this for initialization
    void Start () {
        if (text == null)
        {
            text = GetComponent<Text>();
        }
    }
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;
        textFPS = string.Format("{0:0.0} ms\n({1:0.} fps)", msec, fps);
        text.text = textFPS;
        text.color = (fps > (targetFPS - 5) ? Color.green :
                     (fps > (targetFPS - 30) ? Color.yellow :
                      Color.red));
#endif
    }
}