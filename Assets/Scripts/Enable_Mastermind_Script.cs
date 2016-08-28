using UnityEngine;
using System.Collections;

public class Enable_Mastermind_Script : MonoBehaviour {

    [SerializeField]
    Mastermind_Script mastermindScript;

	// Use this for initialization
	void Start () {
	    if(PhotonNetwork.isMasterClient)
        {
            mastermindScript.enabled = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
