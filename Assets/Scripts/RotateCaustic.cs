using UnityEngine;
using System.Collections;

public class RotateCaustic : MonoBehaviour {

    public Transform myTransform;
    public float rotationsPerMinute = 1.0f;

    // Use this for initialization
    void Start () {
        myTransform = transform;
    }
	
	// Update is called once per frame
	void Update () {
        myTransform.Rotate(0, 0, 6.0f * rotationsPerMinute * Time.deltaTime);
    }
}
