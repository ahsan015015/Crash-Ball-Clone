using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Clips")]
    public AudioClip winSound;
    public AudioClip loseHealthSound;
    public AudioClip BallhitSound;

    public bool SoundOn = true;

    [Space(10)]
    [SerializeField] private Image soundIconRenderer;
    
    [SerializeField] private Sprite SoundOnIcon, SoundOffIcon;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        // Load saved sound setting if exists
        SoundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;

        ApplySoundStatus();
    }

    // Method to change sound on/off
    public void SoundOnStatus(bool status)
    {
        SoundOn = status;

        // Save status
        PlayerPrefs.SetInt("SoundOn", SoundOn ? 1 : 0);
        PlayerPrefs.Save();

        ApplySoundStatus();
    }

    // Apply the actual volume and update icon
    private void ApplySoundStatus()
    {
        AudioListener.volume = SoundOn ? 1f : 0f;

        if (soundIconRenderer != null)
        {
            soundIconRenderer.sprite = SoundOn ? SoundOnIcon : SoundOffIcon;
        }

       
    }
    /// <summary>
    /// Toggle the sound on/off and update the icon
    /// </summary>
    public void ToggleSound()
    {
        SoundOnStatus(!SoundOn); // invert current state
    }


    // // Optional: Play sound clips
    // public void PlaySound(AudioClip clip)
    // {
    //     if (SoundOn && clip != null)
    //     {
    //         AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    //     }
    // }
}
