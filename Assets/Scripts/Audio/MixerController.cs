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
        DontDestroyOnLoad(this);
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
    [SerializeField]
    [Range(0.0F, 1.0F)]
    private float defaultCameraReceiverVolume = 1.0f;

    [SerializeField]
    private float hideLERPSeconds = 1.0f;

    public enum MIXER_BUS { MASTER, MUSIC, SFX, SFX_ENVIRONMENT, SFX_MONSTER, SFX_PLAYER, CAMERARECEIVER };

    static string masterBusString = "Bus:/";
    static string musicBusString = "Bus:/Music";
    static string sfxBusString = "Bus:/SFX";
    static string sfxEnvironmentBusString = "Bus:/SFX/Environment";
    static string sfxMonsterBusString = "Bus:/SFX/Monster";
    static string sfxPlayerBusString = "Bus:/SFX/Player";
    static string cameraReceiverBusString = "Bus:/SFX/CameraReceiver";

    public enum CHASE_STATE { CHASING, PATROLLING };
    public enum CHASE_TYPE { NORMAL, FINAL };

    static public string GetBusName(MIXER_BUS bus)
    {
        switch (bus)
        {
            case MIXER_BUS.MASTER:
                return masterBusString;
            case MIXER_BUS.MUSIC:
                return musicBusString;
            case MIXER_BUS.SFX:
                return sfxBusString;
            case MIXER_BUS.SFX_ENVIRONMENT:
                return sfxEnvironmentBusString;
            case MIXER_BUS.SFX_MONSTER:
                return sfxMonsterBusString;
            case MIXER_BUS.SFX_PLAYER:
                return sfxPlayerBusString;
            case MIXER_BUS.CAMERARECEIVER:
                return cameraReceiverBusString;
        }

        return "";
    }

    static string hideParameterName = "HideFilter";

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
        if (defaultCameraReceiverVolume != -1.0f)
            SetCameraReceiverVolume(defaultCameraReceiverVolume);
    }

    // Update is called once per frame
    void Update()
    {

    }

    static public void SetBusVolume(string busName, float volume)
    {
        FMOD.Studio.Bus bus;
        bus = FMODUnity.RuntimeManager.GetBus(busName);
        var result = bus.setVolume(volume);
    }
    static public float GetBusVolume(string busName)
    {
        FMOD.Studio.Bus bus;
        bus = FMODUnity.RuntimeManager.GetBus(busName);
        float volume;
        bus.getVolume(out volume);
        return volume;
    }
    static public float GetBusVolume(MIXER_BUS busID)
    {
        FMOD.Studio.Bus bus;
        bus = GetBus(busID);
        float volume;
        bus.getVolume(out volume);
        return volume;
    }
    static public FMOD.Studio.Bus GetBus(string busName)
    {
        return FMODUnity.RuntimeManager.GetBus(busName);
    }
    static public FMOD.Studio.Bus GetBus(MIXER_BUS busID)
    {
        return FMODUnity.RuntimeManager.GetBus(GetBusName(busID));
    }

    static public void SetGlobalParameter(string paramName, float value)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName(paramName, value);
    }
    static public float GetGlobalParameter(string paramName)
    {
        float value;
        FMODUnity.RuntimeManager.StudioSystem.getParameterByName(paramName, out value);
        return value;
    }
    static public IEnumerator LERPGlobalParameter(string paramName, float target, float seconds)
    {
        float start = GetGlobalParameter(paramName);
        float elapsed = 0.0f;
        while (elapsed < seconds)
        {
            SetGlobalParameter(paramName, Mathf.Lerp(start, target, elapsed / seconds));
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    static public void SetMasterVolume(float volume)
    {
        SetBusVolume(masterBusString, volume);
    }
    static public float GetMasterVolume()
    {
        return GetBusVolume(masterBusString);
    }
    static public FMOD.Studio.Bus GetMasterBus()
    {
        return FMODUnity.RuntimeManager.GetBus(masterBusString);
    }

    static public void SetMusicVolume(float volume)
    {
        SetBusVolume(musicBusString, volume);
    }
    static public float GetMusicVolume()
    {
        return GetBusVolume(musicBusString);
    }
    static public FMOD.Studio.Bus GetMusicBus()
    {
        return FMODUnity.RuntimeManager.GetBus(musicBusString);
    }

    static public void SetSFXVolume(float volume)
    {
        SetBusVolume(sfxBusString, volume);
    }
    static public float GetSFXVolume()
    {
        return GetBusVolume(sfxBusString);
    }
    static public FMOD.Studio.Bus GetSFXBus()
    {
        return FMODUnity.RuntimeManager.GetBus(sfxBusString);
    }

    static public void SetSFXEnvironmentVolume(float volume)
    {
        SetBusVolume(sfxEnvironmentBusString, volume);
    }
    static public float GetSFXEnvironmentVolume()
    {
        return GetBusVolume(sfxEnvironmentBusString);
    }
    static public FMOD.Studio.Bus GetSFXEnvironmentBus()
    {
        return FMODUnity.RuntimeManager.GetBus(sfxEnvironmentBusString);
    }

    static public void SetSFXMonsterVolume(float volume)
    {
        SetBusVolume(sfxMonsterBusString, volume);
    }
    static public float GetSFXMonsterVolume()
    {
        return GetBusVolume(sfxBusString);
    }
    static public FMOD.Studio.Bus GetSFXMonsterBus()
    {
        return FMODUnity.RuntimeManager.GetBus(sfxMonsterBusString);
    }

    static public void SetSFXPlayerVolume(float volume)
    {
        SetBusVolume(sfxPlayerBusString, volume);
    }
    static public float GetSFXPlayerVolume()
    {
        return GetBusVolume(sfxPlayerBusString);
    }
    static public FMOD.Studio.Bus GetSFXPlayerBus()
    {
        return FMODUnity.RuntimeManager.GetBus(sfxPlayerBusString);
    }

    static public void SetCameraReceiverVolume(float volume)
    {
        SetBusVolume(cameraReceiverBusString, volume);
    }
    static public float GetCameraReceiverVolume()
    {
        return GetBusVolume(cameraReceiverBusString);
    }
    static public FMOD.Studio.Bus GetCameraReceiverBus()
    {
        return FMODUnity.RuntimeManager.GetBus(cameraReceiverBusString);
    }

    static public void SetHiding(bool state)
    {
        if (state)
        {
            instance.StartCoroutine(LERPGlobalParameter(hideParameterName, 1.0f, instance.hideLERPSeconds));
        }
        else
        {
            instance.StartCoroutine(LERPGlobalParameter(hideParameterName, 0.0f, instance.hideLERPSeconds));
        }
    }
}
