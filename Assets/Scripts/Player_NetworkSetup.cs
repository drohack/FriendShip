﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_NetworkSetup : NetworkBehaviour {

    [SerializeField] Camera FPSCharacterCam;
	[SerializeField] AudioListener audioListener;
	// Use this for initialization
	void Start () 
	{
        Debug.Log("In Player_NetworkSetup, isLocalPlayer: " + isLocalPlayer);
		if (isLocalPlayer) 
		{
			GameObject.Find ("Main Camera").SetActive (false);
            //transform.Find("OvrTouch").gameObject.SetActive(true);
            //GetComponent<CharacterController> ().enabled = true;
            //GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().enabled = true;
            //FPSCharacterCam.name = "Player Camera";
            FPSCharacterCam.enabled = true;
            audioListener.enabled = true;
        }
	}
}
