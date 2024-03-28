using UnityEngine;

[CreateAssetMenu(fileName = "SoundPool", menuName = "Scriptable object/Sound Pool")]
public class SOSoundPool : ScriptableObject
{
    [SerializeField] AudioClip[] _sounds;

    public int PlaySound(AudioSource source)
    {
        //Debug if there is no sound in the pool
        if (_sounds.Length < 1)
        {
            Debug.Log("Attention : pas d'audioclip dans la sound pool");
        }
        //Choose random sound
        int index = Random.Range(0, _sounds.Length);

        //Play it
        source.clip = _sounds[index];
        source.PlayOneShot(source.clip);
        return index;
    }

    public void PlayMusic(AudioSource source)
    {
        source.clip = _sounds[0];
        source.Play();
    }
    public void PlayMusic(AudioSource source, bool isLoop)
    {
        if (!source.loop)
        {
            source.loop = true;
        }
        source.clip = _sounds[0];
        source.Play();
    }

}