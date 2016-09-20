using UnityEngine;
using System.Collections;

public class Plutonium_Case_Script : Photon.MonoBehaviour
{
    private bool isRodLoaded = false;
    private bool isRodLeftColliding = false;
    private bool isRodRightColliding = false;

    Mastermind_Script mastermindScript;

    //Network variables
    public string newName;
    public int rCommand = -1;
    public int playerNum;

    // Use this for initialization
    void Start () {
        //Load Network data
        object[] data = photonView.instantiationData;
        if (data != null)
        {
            newName = transform.Find("Labels/Name").GetComponent<TextMesh>().text = (string)data[0];
            rCommand = (int)data[1];
            playerNum = (int)data[2];
        }

        isRodLoaded = false;
        isRodLeftColliding = false;
        isRodRightColliding = false;

        if (PhotonNetwork.isMasterClient)
            mastermindScript = GameObject.Find("Mastermind").GetComponent<Mastermind_Script>();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter: " + other.tag);
        // Check to see if it is a rod colliding
        if (other.tag.Equals("PlutoniumRodLeft"))
            isRodLeftColliding = true;
        else if (other.tag.Equals("PlutoniumRodRight"))
            isRodRightColliding = true;

        // If both halves are colliding Destroy the object
        // Enable the disabbled rod in the case
        // Send tapped command to Mastermind
        if (isRodLeftColliding && isRodRightColliding && other != null && other.transform.parent != null && other.transform.parent.parent != null
                && other.transform.parent.parent.tag.Equals("PlutoniumRod"))
        {
            //other.transform.parent.parent.gameObject.GetPhotonView().RPC("Destroy", PhotonTargets.All);
            Destroy(other.transform.parent.parent.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit: " + other.tag);
        if (other.tag.Equals("PlutoniumRodLeft"))
            isRodLeftColliding = false;
        else if (other.tag.Equals("PlutoniumRodRight"))
            isRodRightColliding = false;
    }

    // Update is called once per frame
    void Update () {
	    
	}

    [PunRPC]
    void CmdSendTappedCommand(int sentRCommand, int sentPlayerNum)
    {
        mastermindScript.TappedWaitForSecondsOrTap(sentRCommand, sentPlayerNum);
    }

    [PunRPC]
    void RPCDestroy()
    {
        if (photonView.isMine)
            PhotonNetwork.Destroy(gameObject);
    }
}
