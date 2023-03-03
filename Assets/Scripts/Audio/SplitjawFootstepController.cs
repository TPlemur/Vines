using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SplitjawFootstepController : MonoBehaviour
{
    public enum TERRAIN_TYPES { CONCRETE, METAL, NONE };

    // must have a default terrain unless an fmod event emmitter with the terrain set is on the script
    [SerializeField]
    private TERRAIN_TYPES defaultTerrain = TERRAIN_TYPES.NONE;
    private TERRAIN_TYPES currentTerrain = TERRAIN_TYPES.NONE;

    private FMOD.Studio.EventInstance footstep;

    private float intensity = -1f;

    // if there is a studio event emiitor on the gameobject it will be used to set the parameters of each footstep created (though some may be overridden by the controller script)
    public FMODUnity.StudioEventEmitter referenecEvent = null;

    public void SetTerrainType(TERRAIN_TYPES type) { currentTerrain = type; }

    // encapsulated functions in case the character controller was to change for whatever reason, these functions could just be changed
    public bool IsGrounded() { return true; }

    // Start is called before the first frame update
    void Start()
    {
        if (referenecEvent == null)
            referenecEvent = GetComponent<FMODUnity.StudioEventEmitter>();

        SetTerrainType(defaultTerrain);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public float GetIntensity()
    {
        return intensity;
    }
    public void SetIntensity(float i)
    {
        intensity = i;
    }

    public void PlayFootstep()
    {
        if (referenecEvent != null)
        {
            // create new instance from event name of reference object
            footstep = FMODUnity.RuntimeManager.CreateInstance(referenecEvent.EventReference);
            // set parameters from reference event object
            foreach (FMODUnity.ParamRef param in referenecEvent.Params)
            {
                footstep.setParameterByName(param.Name, param.Value);
            }
            // copy over override attenuation properties from reference event if applicable
            if (referenecEvent.OverrideAttenuation)
            {
                footstep.setProperty(FMOD.Studio.EVENT_PROPERTY.MINIMUM_DISTANCE, referenecEvent.OverrideMinDistance);
                footstep.setProperty(FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, referenecEvent.OverrideMaxDistance);
            }
        }
        else
        {
            footstep = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Player Footsteps");
        }

        // footset controller param controls
        footstep.setParameterByName("Terrain", (float)currentTerrain);
        if (GetIntensity() != -1.0f)
            footstep.setParameterByName("Intensity", GetIntensity());
        // 3D position
        footstep.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        footstep.start();
        footstep.release();
    }

    public void SelectAndPlayFootstep()
    {
        DetermineTerrain();
        PlayFootstep();
    }

    void DetermineTerrain()
    {
        // Temp
        if (referenecEvent != null)
        {
            float terrain = 0.0f;
            // should be a more elegant way to do this, but can't use Array.Find() for some reason...
            foreach (FMODUnity.ParamRef param in referenecEvent.Params)
            {
                if (param.Name == "Terrain")
                {
                    terrain = param.Value;
                    break;
                }
            }
            currentTerrain = (TERRAIN_TYPES)terrain;
        }
        else
        {
            currentTerrain = defaultTerrain;
        }
    }
}
