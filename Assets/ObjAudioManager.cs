using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Clip
{
    public AudioSource source;
    public string name;
    public int hash;
    public float volume;
    public bool isFX;
}

public class ObjAudioManager : MonoBehaviour
{
    public AudioClipArray clipArray;
    private List<Clip> clips = new List<Clip>();

    private void Awake()
    {
        if(clipArray != null)
        {
            clipArray.Initialize();
            foreach(Sound s in clipArray.sounds)
            {
                Clip newClip = new Clip();
                newClip.name = s.name;
                newClip.hash = s.hashCode;
                newClip.isFX = s.isFX;
                newClip.volume = s.volume;
                newClip.source = gameObject.AddComponent<AudioSource>();
                newClip.source.playOnAwake = false;
                newClip.source.clip = s.clip;
                newClip.source.volume = s.volume;
                newClip.source.pitch = s.pitch;
                newClip.source.loop = s.loop;
                newClip.source.spatialBlend = s.blend;
                clips.Add(newClip);
            }
        }
    }

    public void PlayByName(string name)
    {
        int hash = name.GetHashCode();
        foreach (Clip s in clips)
        {
            if (hash == s.hash && name.Equals(s.name))
            {
                s.source.Play();
                return;
            }
        }
        Debug.Log("The audio clip " + name + " is not in the array.");
    }

    public void StopByName(string name)
    {
        int hash = name.GetHashCode();
        foreach (Clip s in clips)
        {
            if (hash == s.hash && name.Equals(s.name))
            {
                if(s.source.isPlaying)
                    s.source.Stop();
                return;
            }
        }
        Debug.Log("The audio clip " + name + " is not in the array.");
    }
}
