using UnityEngine;
using System.Collections;

public class Tablet_Recent_Script : MonoBehaviour {

    [SerializeField]
    TextMesh textMesh;

	// Use this for initialization
	void Start () {
#if OCULUS
        textMesh.text = "To Recenter your Camera"
            + "\nPress in the Thumb Stick";
#else
        textMesh.text = "To Recenter your Camera"
            + "\nPress the top Menu button";
#endif
    }
}
