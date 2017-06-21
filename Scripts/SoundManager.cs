using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

    Bag bag;
    mainMenuManager MM;
    public AudioSource[] audioSources;



    void Awake()
    {
        MM = GameObject.FindGameObjectWithTag("UI").GetComponent<mainMenuManager>();
        bag = GameObject.FindGameObjectWithTag("Bag").GetComponent<Bag>();
    }

    void Start()
    {
        Load_Volumes();
    }

    void Load_Volumes()
    {
        foreach (AudioSource source in audioSources)
        {
            source.mute = bag.isMuted;
        }
        MM.SetVolumeButton(bag.isMuted);
    }

    public void Set_Voumes()
    {
        bag.Volume_Setting();
        foreach (AudioSource source in audioSources)
        {
            source.mute = bag.isMuted;
        }
        MM.SetVolumeButton(bag.isMuted);
    }

}
