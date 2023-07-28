using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{

    public static AudioClip fireSound, reloadSound, backgroundMusic;
    static AudioSource audioSrc;

    // Start is called before the first frame update
    void Start()
    {

        fireSound = Resources.Load<AudioClip>("fire");
        reloadSound = Resources.Load<AudioClip>("reload");
        backgroundMusic = Resources.Load<AudioClip>("encounter_loop");

        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlaySound (string clip)
    {

        switch (clip)
        {
            case "fire":
                audioSrc.PlayOneShot(fireSound);
                break;
            case "reload":
                audioSrc.PlayOneShot(reloadSound);
                break;
            case "encounter_loop":
                audioSrc.PlayOneShot(backgroundMusic);
                break;
        }

    }

}
