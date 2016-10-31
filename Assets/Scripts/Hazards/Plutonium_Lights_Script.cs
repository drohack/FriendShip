using UnityEngine;
using System.Collections;

public class Plutonium_Lights_Script : Photon.MonoBehaviour
{
    [SerializeField]
    Plutonium_Lights_Trigger_Script plutoniumCaseTriggerScript;
    [SerializeField]
    GameObject lockedPlutoniumRod;

    private Vector3 lockedPlutoniumRodOriginalScale;
    private float targetScale = 0.001f;
    private float shrinkSpeed = 2f;

    public bool isRodLoaded = false;

    Mastermind_Script mastermindScript;

    //Network variables
    public int playerNum;

    // Use this for initialization
    void Start () {
        //Load Network data
        object[] data = photonView.instantiationData;
        if (data != null)
        {
            playerNum = (int)data[0];
        }

        transform.parent = GameObject.Find("Back Panel " + playerNum).transform;

        lockedPlutoniumRodOriginalScale = lockedPlutoniumRod.transform.localScale;
        isRodLoaded = false;
        
        mastermindScript = GameObject.FindGameObjectWithTag("Mastermind").GetComponent<Mastermind_Script>();
    }

    public void LoadRod()
    {
        //Enable the locked plutonium rod so it looks like it snapped into place
        lockedPlutoniumRod.SetActive(true);

        //send tapped command to Mastermind
        photonView.RPC("CmdTurnOnAmbientLight", PhotonTargets.MasterClient, null);

        isRodLoaded = true;

        //Load animation on all other versions of me
        photonView.RPC("RPCLoadLockedPlutoniumRod", PhotonTargets.Others, isRodLoaded);
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
                plutoniumCaseTriggerScript.isRodLeftColliding = false;
                plutoniumCaseTriggerScript.isRodRightColliding = false;
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
    void CmdTurnOnAmbientLight()
    {
        mastermindScript.photonView.RPC("TurnOnAmbientLight", PhotonTargets.All, null);
    }

    [PunRPC]
    void RPCDestroy()
    {
        if (photonView.isMine)
            PhotonNetwork.Destroy(gameObject);
    }
}
