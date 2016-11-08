using UnityEngine;
using System.Collections;

public class BackgroundMusic_Script : MonoBehaviour {
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

        if (!BackGroundMusicSource.playOnAwake)
        {
            BackGroundMusicSource.clip = songList[Random.Range(0, songList.Length)];
            BackGroundMusicSource.Play();
        }


	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!BackGroundMusicSource.isPlaying)
        {
            BackGroundMusicSource.clip = songList[Random.Range(0, songList.Length)];
            BackGroundMusicSource.Play();
        }
	}
}
