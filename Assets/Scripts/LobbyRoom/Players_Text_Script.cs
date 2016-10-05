using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Players_Text_Script : Photon.MonoBehaviour
{
    [SerializeField]
    private TextMesh textMesh;

    // Use this for initialization
    void Awake ()
    {
        textMesh.text = "";
    }

    [PunRPC]
    public void RpcUpdateText(string message)
    {
        textMesh.text = message;
    }
}
