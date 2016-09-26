using UnityEngine;
using System.Collections;

public class Plutonium_Case_Trigger_Script : MonoBehaviour {

    [SerializeField]
    Plutonium_Case_Script plutoniumCaseScript;
    
    public bool isRodLeftColliding = false;
    public bool isRodRightColliding = false;

    void OnTriggerEnter(Collider other)
    {
        // Check to see if it is a rod colliding
        if (other.tag.Equals("PlutoniumRodLeft"))
            isRodLeftColliding = true;
        else if (other.tag.Equals("PlutoniumRodRight"))
            isRodRightColliding = true;

        // Make sure there isn't a rod loaded already
        // If both halves are colliding Destroy the object
        // Enable the disabbled rod in the case
        // Send tapped command to Mastermind
        if (!plutoniumCaseScript.isRodLoaded && isRodLeftColliding && isRodRightColliding && other != null && other.transform.parent != null && other.transform.parent.parent != null
                && other.transform.parent.parent.tag.Equals("PlutoniumRod"))
        {
            //Destroy the free floating Plutonium Rod
            other.transform.parent.parent.gameObject.GetPhotonView().RPC("RPCDestroy", PhotonTargets.All);

            plutoniumCaseScript.LoadRod();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("PlutoniumRodLeft"))
            isRodLeftColliding = false;
        else if (other.tag.Equals("PlutoniumRodRight"))
            isRodRightColliding = false;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
