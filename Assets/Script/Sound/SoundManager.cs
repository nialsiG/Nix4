using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [Header("Volume")]
    [SerializeField] float _musicVolume;
    [SerializeField] float _soundVolume;
    [SerializeField] GameObject _volumeSlider;
    private float generalVolume;

    [Header("Music")]
    [SerializeField] SOSoundPool _intro;
    [SerializeField] SOSoundPool _musicChill, _musicStressed, _musicVictory;
    List<AudioSource> musicAudioSource;
    private int musicIntInQueue;

    [Header("Sound queue")]
    [SerializeField] int _soundQueueLength;
    [SerializeField] SOSoundPool _fanfare, _defeat, _hitWall;
    List<AudioSource> soundQueue;
    private int currentIntInQueue;

    //Singleton
    public static SoundManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Create soundqueue
        soundQueue = new List<AudioSource>();
        for (int i = 0; i < _soundQueueLength; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.volume = _soundVolume;
            soundQueue.Add(source);
        }

        //Create music audiosource
        musicAudioSource = new List<AudioSource>();
        for (int i = 0; i < 2; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.volume = _musicVolume * i;
            source.loop = true;
            musicAudioSource.Add(source);
        }
        musicIntInQueue = 1;

        //Set initial volume
        ChangeVolume();

        //Play theme
        PlayMusic(_intro);
    }

    public void ChangeVolume()
    {
        Slider slider = _volumeSlider?.GetComponent<Slider>();

        generalVolume = slider.value / slider.maxValue;
       
        foreach (AudioSource s in musicAudioSource)
        {
            s.volume = _musicVolume * generalVolume;
        }

        foreach (AudioSource s in soundQueue)
        {
            s.volume = _soundVolume * generalVolume;
        }
    }

    public void ChangeVolume(float factor)
    {
        generalVolume = generalVolume * factor;

        foreach (AudioSource s in musicAudioSource)
        {
            s.volume = _musicVolume * generalVolume;
        }

        foreach (AudioSource s in soundQueue)
        {
            s.volume = _soundVolume * generalVolume;
        }
    }

    public void PlayMusic(SOSoundPool music)
    {
        music.PlayMusic(musicAudioSource[musicIntInQueue % 2]);
    }

    public void ChangeMusic(SOSoundPool music)
    {
        //start coroutine fade out
        StartCoroutine(FadeOut(musicAudioSource[musicIntInQueue % 2]));
        //change music
        musicIntInQueue += 1;
        PlayMusic(music);
        //start coroutine fade in
        StartCoroutine(FadeIn(musicAudioSource[musicIntInQueue % 2]));
        
    }

    //Coroutines
    //...for decreasing volume when the music stops
    IEnumerator FadeOut(AudioSource audioSource)
    {
        for (float v = (_musicVolume * generalVolume); v >= 0f; v -= 0.005f)
        {
            audioSource.volume = v;
            
            if (audioSource.volume <= 0.001f)
            {
                audioSource.Stop();
            }

            yield return null;
        }
    }

    //...for increasing volume when the music starts
    IEnumerator FadeIn(AudioSource audioSource)
    {
        for (float v = 0f; v <= (_musicVolume * generalVolume); v += 0.005f)
        {
            audioSource.volume = v;
            yield return null;
        }
    }

    public int PlaySound(SOSoundPool soundPool)
    {
        //Select the next audiosource in queue
        currentIntInQueue += 1;
        Debug.Log("Currently playing audiosource n." + currentIntInQueue % (_soundQueueLength - 1));

        //option 1 : kill sound
        soundQueue[currentIntInQueue % (_soundQueueLength - 1)].Stop();
        //option 2 : let the sound play?
        //if (!soundQueue[currentIntInQueue].isPlayint) {}

        //...and play sound
        return soundPool.PlaySound(soundQueue[currentIntInQueue % (_soundQueueLength - 1)]);
    }

    public void PlayIntro()
    {
        ChangeMusic(_intro);
    }

    public void PlayChill()
    {
        ChangeMusic(_musicChill);
        musicAudioSource[musicIntInQueue % 2].loop = true;
    }

    public void PlayStress()
    {
        ChangeMusic(_musicStressed);
        musicAudioSource[musicIntInQueue % 2].loop = true;
    }

    public void PlayVictory()
    {
        ChangeMusic(_fanfare);
        musicAudioSource[musicIntInQueue % 2].loop = false;
    }

    public void PlayDefeat()
    {
        ChangeMusic(_defeat);
        musicAudioSource[musicIntInQueue % 2].loop = false;
    }

    public void PlayHitWall()
    {
        PlaySound(_hitWall);
    }
}