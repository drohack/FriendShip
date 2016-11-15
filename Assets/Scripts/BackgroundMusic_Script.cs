using UnityEngine;
using System.Collections;

public class BackgroundMusic_Script : Photon.MonoBehaviour {
    public AudioClip[] songList;
    public AudioSource BackGroundMusicSource;

    // Use this for initialization
    void Start ()
    {
        songList = new AudioClip[]
        {
            (AudioClip)Resources.Load("Music/8-Bit_Action_Free/Aggressive1"),
            (AudioClip)Resources.Load("Music/8-Bit_Action_Free/Quiet2"),
            (AudioClip)Resources.Load("Music/8-Bit_Action_Free/Soft1"),
            (AudioClip)Resources.Load("Music/8-Bit_Action_Free/Stage1"),
            (AudioClip)Resources.Load("Music/8-Bit_Action_Free/Title")
        };

        BackGroundMusicSource = GetComponent<AudioSource>();

        if (PhotonNetwork.isMasterClient)
        {
            if (!BackGroundMusicSource.playOnAwake)
            {
                BackGroundMusicSource.clip = songList[Random.Range(0, songList.Length)];
                BackGroundMusicSource.Play();
            }
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (!BackGroundMusicSource.isPlaying)
            {
                BackGroundMusicSource.clip = songList[Random.Range(0, songList.Length)];
                BackGroundMusicSource.Play();
                photonView.RPC("RPCPlaySong", PhotonTargets.Others, "BackGroundMusicSource.clip", "0");
            }
        }
        Debug.Log("audio.time: " + BackGroundMusicSource.time);
        Debug.Log("audio.clip.length: " + BackGroundMusicSource.clip.length);
        Debug.Log("audio.time / audio.clip.length: " + BackGroundMusicSource.time / BackGroundMusicSource.clip.length);
    }

    public void playSong(AudioClip song, float timestamp)
    {
        BackGroundMusicSource.clip = song;
        BackGroundMusicSource.time = BackGroundMusicSource.clip.length * timestamp;
        BackGroundMusicSource.Play();
    }

    [PunRPC]
    void RPCplaySong(AudioClip song, float timestamp)
    {
        BackGroundMusicSource.clip = song;
        BackGroundMusicSource.time = BackGroundMusicSource.clip.length * timestamp;
        BackGroundMusicSource.Play();
    }

    [PunRPC]
    AudioClip RPCgetSong()
    {
        return BackGroundMusicSource.clip;
    }

    [PunRPC]
    float RPCgetTimestamp()
    {
        float timestamp = BackGroundMusicSource.time / BackGroundMusicSource.clip.length;
        return timestamp;
    }
}
