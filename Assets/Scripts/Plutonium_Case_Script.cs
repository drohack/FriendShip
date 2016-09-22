using UnityEngine;
using System.Collections;

public class Plutonium_Case_Script : Photon.MonoBehaviour
{
    [SerializeField]
    GameObject lockedPlutoniumRod;

    private Vector3 lockedPlutoniumRodOriginalScale;
    private float targetScale = 0.001f;
    private float shrinkSpeed = 2f;

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

        lockedPlutoniumRodOriginalScale = lockedPlutoniumRod.transform.localScale;
        isRodLoaded = false;
        isRodLeftColliding = false;
        isRodRightColliding = false;

        if (PhotonNetwork.isMasterClient)
            mastermindScript = GameObject.Find("Mastermind").GetComponent<Mastermind_Script>();
    }

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
        if (!isRodLoaded && isRodLeftColliding && isRodRightColliding && other != null && other.transform.parent != null && other.transform.parent.parent != null
                && other.transform.parent.parent.tag.Equals("PlutoniumRod"))
        {
            //Destroy the free floating Plutonium Rod
            other.transform.parent.parent.gameObject.GetPhotonView().RPC("RPCDestroy", PhotonTargets.All);

            //Enable the locked plutonium rod so it looks like it snapped into place
            lockedPlutoniumRod.SetActive(true);

            //send tapped command to Mastermind
            photonView.RPC("CmdSendTappedCommand", PhotonTargets.MasterClient, rCommand, playerNum);

            isRodLoaded = true;

            //Load animation on all other versions of me
            photonView.RPC("RPCLoadLockedPlutoniumRod", PhotonTargets.Others, isRodLoaded);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("PlutoniumRodLeft"))
            isRodLeftColliding = false;
        else if (other.tag.Equals("PlutoniumRodRight"))
            isRodRightColliding = false;
    }

    // Update is called once per frame
    void Update () {
        if (isRodLoaded)
        {
            if (lockedPlutoniumRod.transform.localScale.x >= (targetScale * 2))
            {
                lockedPlutoniumRod.transform.localScale = Vector3.Lerp(lockedPlutoniumRod.transform.localScale, new Vector3(targetScale, targetScale, targetScale), Time.deltaTime * shrinkSpeed);
            }
            else
            {
                //The rod has shrunk enough, disable it and return it to it's original size (so it can be used again) and let people reload more Plutonium Rods
                lockedPlutoniumRod.SetActive(false);
                lockedPlutoniumRod.transform.localScale = lockedPlutoniumRodOriginalScale;
                isRodLoaded = false;
                isRodLeftColliding = false;
                isRodRightColliding = false;
            }
        }
    }

    [PunRPC]
    void RPCLoadLockedPlutoniumRod(bool sentIsRodLoaded)
    {
        //Enable the locked plutonium rod so it looks like it snapped into place
        lockedPlutoniumRod.SetActive(true);

        isRodLoaded = true;
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
