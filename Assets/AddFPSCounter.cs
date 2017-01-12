using UnityEngine;
using System.Collections;

public class AddFPSCounter : MonoBehaviour {

	// Use this for initialization
	void Start () {
#if UNITY_EDITOR
        GameObject go = (GameObject)Instantiate(Resources.Load("ShowFPSCanvas"));
        go.transform.parent = this.transform;
        go.transform.localPosition = new Vector3(-0.062f, -0.022f, 0.277f);
#endif
    }
}
