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
            //Start the first song
            BackGroundMusicSource.clip = songList[Random.Range(0, songList.Length)];
            BackGroundMusicSource.Play();
        }
        else
        {
            //You joined the room, get the song from the Master Client
            Debug.Log("Requesting song");
            photonView.RPC("RPCRequestSong", PhotonTargets.MasterClient, PhotonNetwork.player.name);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (PhotonNetwork.isMasterClient)
        {
            //If the current clip finishes play a new one and send RPCPlaySong to all clients
            if (!BackGroundMusicSource.isPlaying)
            {
                BackGroundMusicSource.clip = songList[Random.Range(0, songList.Length)];
                BackGroundMusicSource.Play();
                photonView.RPC("RPCPlaySong", PhotonTargets.Others, BackGroundMusicSource.clip.name, 0f, null);
            }
            //Debug.Log("audio.time: " + BackGroundMusicSource.time);
            //Debug.Log("audio.clip.length: " + BackGroundMusicSource.clip.length);
            //Debug.Log("audio.time / audio.clip.length: " + BackGroundMusicSource.time / BackGroundMusicSource.clip.length);
        }
    }

    [PunRPC]
    void RPCRequestSong(string name)
    {
        //A user has requested your song, send the RPCPlaySong with correct song and timestamp
        Debug.Log("Song requested by: " + name);
        float timestamp = BackGroundMusicSource.time / BackGroundMusicSource.clip.length;
        photonView.RPC("RPCPlaySong", PhotonTargets.Others, BackGroundMusicSource.clip.name, timestamp, name);
    }

    [PunRPC]
    void RPCPlaySong(string songName, float timestamp, string name)
    {
        Debug.Log("RPCPlaySong: " + songName + " @" + timestamp + " from: " + name);
        //If name is null send it to everyone, else only start playing for the user that requested it
        if (name == null || name.Equals(PhotonNetwork.player.name))
        {
            AudioClip clip = null;
            foreach (AudioClip ac in songList)
            {
                if (ac.name.Equals(songName))
                {
                    clip = ac;
                    break;
                }
            }
            BackGroundMusicSource.clip = clip;
            BackGroundMusicSource.time = BackGroundMusicSource.clip.length * timestamp;
            BackGroundMusicSource.Play();
        }
    }
}
