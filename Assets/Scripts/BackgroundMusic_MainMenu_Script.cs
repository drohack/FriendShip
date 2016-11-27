using UnityEngine;
using System.Collections;

public class BackgroundMusic_MainMenu_Script : MonoBehaviour
{
    public AudioClip[] songList;
    public AudioSource BackGroundMusicSource;

    // Use this for initialization
    void Start()
    {
        songList = new AudioClip[]
        {
            (AudioClip)Resources.Load("Music/8Bit Classic Free/Ave Maria"),
            (AudioClip)Resources.Load("Music/8Bit Classic Free/Courante"),
            (AudioClip)Resources.Load("Music/8Bit Classic Free/Je te veux"),
            (AudioClip)Resources.Load("Music/8Bit Classic Free/Pachelbel's Canon"),
            (AudioClip)Resources.Load("Music/8Bit Classic Free/Pavane for a Dead Princess")
        };

        BackGroundMusicSource = GetComponent<AudioSource>();

        if (!BackGroundMusicSource.playOnAwake)
        {
            BackGroundMusicSource.clip = songList[Random.Range(0, songList.Length)];
            BackGroundMusicSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!BackGroundMusicSource.isPlaying)
        {
            BackGroundMusicSource.clip = songList[Random.Range(0, songList.Length)];
            BackGroundMusicSource.Play();
        }
    }
}