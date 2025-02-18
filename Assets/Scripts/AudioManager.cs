using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    public RhythmChartParser chartParser;

    FMOD.ChannelGroup sfxChannelGroup;
    FMOD.Sound[] sfxs;
    FMOD.Channel[] sfxChannels;

    public EventReference musicEvent;  // FMOD event (bgm)
    private EventInstance musicInstance;
    private FMOD.ChannelGroup masterChannelGroup;
    private float currentMusicTime = 0f;  // in seconds
    
    float sfxVolume = 1;
    float masterVolume = 1;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        FMODUnity.RuntimeManager.CoreSystem.createChannelGroup("SFXGroup", out sfxChannelGroup);
        LoadSFX();
    }

    void Start()
    {
        musicInstance = RuntimeManager.CreateInstance(musicEvent);
        Invoke("StartMusic", 2.0f);
    }
    
    void StartMusic()
    {
        int offsetMilliseconds = (int)(chartParser.offset * 1000);  // ms
        Debug.Log("OFFSET: " + offsetMilliseconds);
        musicInstance.start();
        musicInstance.setTimelinePosition(offsetMilliseconds);
        musicInstance.setVolume(1.0f);
        musicInstance.setPaused(false);
    }

    void Update()
    {
        if (musicInstance.isValid())
        {
            ulong dspClock; // use dspClock for better sync
            FMODUnity.RuntimeManager.CoreSystem.getMasterChannelGroup(out masterChannelGroup);
            masterChannelGroup.getDSPClock(out dspClock, out _);

            double sampleRate;
            FMODUnity.RuntimeManager.CoreSystem.getSoftwareFormat(out int rate, out _, out _);
            sampleRate = rate; // Hz

            // Calculate current music time based on DSP Clock
            currentMusicTime = (float)(dspClock / sampleRate);
        }

        if(Input.anyKeyDown)
            PlaySFX(SFX.HitNote);
    }

    void LoadSFX()
    {
        int count = (int)SFX.Count; // Count of Enum

        sfxs = new FMOD.Sound[count];
        sfxChannels = new FMOD.Channel[count];

        for (int i = 0; i < count; i++)
        {
            string sfxFileName = System.Enum.GetName(typeof(SFX), i); // Enum to string
            string filePath = Path.Combine(Application.streamingAssetsPath, "SFXS", sfxFileName + ".wav");

            // Check if file exists
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"[AudioManager] File load error: {filePath}");
                continue;
            }

            FMODUnity.RuntimeManager.CoreSystem.createSound(filePath, FMOD.MODE.CREATESAMPLE, out sfxs[i]);
        }
    }
    
    public void PlaySFX(SFX sfx, float volume = 1)
    {
        sfxChannels[(int)sfx].stop();

        FMODUnity.RuntimeManager.CoreSystem.playSound(sfxs[(int)sfx], sfxChannelGroup, false, out sfxChannels[(int)sfx]);

        sfxChannels[(int)sfx].setPaused(true);
        sfxChannels[(int)sfx].setVolume((volume * sfxVolume) * masterVolume);
        sfxChannels[(int)sfx].setPaused(false);
    }

    public float GetMusicTime()
    {
        return currentMusicTime;
    }
}