using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixerController : MonoBehaviour
{

    public static MixerController instance;

    // Make sure there is only 1 GameManager
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(transform);
    }

    [SerializeField]
    [Range(0.0F, 1.0F)]
    private float defaultMasterVolume = 1.0f;
    [SerializeField]
    [Range(0.0F, 1.0F)]
    private float defaultMusicVolume = 1.0f;
    [SerializeField]
    [Range(0.0F, 1.0F)]
    private float defaultSFXVolume = 1.0f;
    [SerializeField]
    [Range(0.0F, 1.0F)]
    private float defaultSFXEnvironmentVolume = 1.0f;
    [SerializeField]
    [Range(0.0F, 1.0F)]
    private float defaultSFXMonsterVolume = 1.0f;
    [SerializeField]
    [Range(0.0F, 1.0F)]
    private float defaultSFXPlayerVolume = 1.0f;

    string masterBusString = "Bus:/";
    string musicBusString = "Bus:/Music";
    string sfxBusString = "Bus:/SFX";
    string sfxEnvironmentBusString = "Bus:/SFX/Environment";
    string sfxMonsterBusString = "Bus:/SFX/Monster";
    string sfxPlayerBusString = "Bus:/SFX/Player";

    // Start is called before the first frame update
    void Start()
    {
        if (defaultMasterVolume != -1.0f)
            SetMasterVolume(defaultMasterVolume);
        if (defaultMusicVolume != -1.0f)
            SetMusicVolume(defaultMusicVolume);
        if (defaultSFXVolume != -1.0f)
            SetSFXVolume(defaultSFXVolume);
        if (defaultSFXEnvironmentVolume != -1.0f)
            SetSFXEnvironmentVolume(defaultSFXEnvironmentVolume);
        if (defaultSFXMonsterVolume != -1.0f)
            SetSFXMonsterVolume(defaultSFXMonsterVolume);
        if (defaultSFXPlayerVolume != -1.0f)
            SetSFXPlayerVolume(defaultSFXPlayerVolume);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetBusVolume(string busName, float volume)
    {
        FMOD.Studio.Bus bus;
        bus = FMODUnity.RuntimeManager.GetBus(busName);
        bus.setVolume(volume);
    }
    public float GetBusVolume(string busName)
    {
        FMOD.Studio.Bus bus;
        bus = FMODUnity.RuntimeManager.GetBus(busName);
        float volume;
        bus.getVolume(out volume);
        return volume;
    }

    public void SetMasterVolume(float volume)
    {
        SetBusVolume(masterBusString, volume);
    }
    public float GetMasterVolume()
    {
        return GetBusVolume(masterBusString);
    }

    public void SetMusicVolume(float volume)
    {
        SetBusVolume(musicBusString, volume);
    }
    public float GetMusicVolume()
    {
        return GetBusVolume(musicBusString);
    }

    public void SetSFXVolume(float volume)
    {
        SetBusVolume(sfxBusString, volume);
    }
    public float GetSFXVolume()
    {
        return GetBusVolume(sfxBusString);
    }

    public void SetSFXEnvironmentVolume(float volume)
    {
        SetBusVolume(sfxEnvironmentBusString, volume);
    }
    public float GetSFXEnvironmentVolume()
    {
        return GetBusVolume(sfxEnvironmentBusString);
    }

    public void SetSFXMonsterVolume(float volume)
    {
        SetBusVolume(sfxMonsterBusString, volume);
    }
    public float GetSFXMonsterVolume()
    {
        return GetBusVolume(sfxBusString);
    }

    public void SetSFXPlayerVolume(float volume)
    {
        SetBusVolume(sfxPlayerBusString, volume);
    }
    public float GetSFXPlayerVolume()
    {
        return GetBusVolume(sfxPlayerBusString);
    }
}
