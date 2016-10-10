using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Players_Text_Script : Photon.MonoBehaviour
{
    [SerializeField]
    private TextMesh textMesh;

    //Network variables
    public int playerPosition;

    // Use this for initialization
    void Awake ()
    {
        textMesh.text = "";
    }

    // Use this for initialization
    void Start()
    {
        object[] data = photonView.instantiationData;
        if (data != null)
        {
            playerPosition = (int)data[0];
        }
    }

    [PunRPC]
    public void RpcUpdateText(string message)
    {
        textMesh.text = message;
    }
}
