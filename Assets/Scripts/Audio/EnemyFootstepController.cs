using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Enemy))]
public class EnemyFootstepController : MonoBehaviour
{
    public enum TERRAIN_TYPES { CONCRETE, METAL, NONE };

    private NavMeshAgent meshAgent = null;
    private Enemy enemy = null;

    // must have a default terrain unless an fmod event emmitter with the terrain set is on the script
    [SerializeField]
    private TERRAIN_TYPES defaultTerrain = TERRAIN_TYPES.NONE;
    private TERRAIN_TYPES currentTerrain = TERRAIN_TYPES.NONE;

    private FMOD.Studio.EventInstance footstep;

    // the minimum time between footstep hits (i.e. at normal walking speed)
    [SerializeField]
    private float minTime = 0.5f;

    private float timeUntilNextFootstep = 0.0f;

    // if -1 is set, the script will grab this value from the character controller in some way
    [SerializeField]
    private float maxMoveSpeed = -1.0f;

    // if there is a studio event emiitor on the gameobject it will be used to set the parameters of each footstep created (though some may be overridden by the controller script)
    public FMODUnity.StudioEventEmitter referenecEvent = null;

    public float test1;
    public float test2;

    public void SetTerrainType(TERRAIN_TYPES type) { currentTerrain = type; }

    // encapsulated functions in case the character controller was to change for whatever reason, these functions could just be changed
    public bool IsGrounded() { return true; }
    public bool IsCrouching() { return false; }
    public Vector3 GetVelocity() { return meshAgent.velocity; }

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy>();
        meshAgent = enemy.Mob;

        if (referenecEvent == null)
            referenecEvent = GetComponent<FMODUnity.StudioEventEmitter>();

        SetTerrainType(defaultTerrain);

        if (maxMoveSpeed == -1.0f)
            maxMoveSpeed = meshAgent.speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (ShouldTriggerFootstep())
            SelectAndPlayFootstep();

        // for testing
        //if (Input.GetMouseButtonDown(0))
        //{
        //    SelectAndPlayFootstep();
        //}
    }

    private bool ShouldTriggerFootstep()
    {
        if (!IsGrounded())
            return false;
        const float veloCutoff = 0.1f;
        test1 = GetVelocity().magnitude;
        if ((new Vector3(GetVelocity().x, 0f, GetVelocity().z)).magnitude <= veloCutoff)
            return false;

        test2 = GetVeloRatio();
        timeUntilNextFootstep -= Time.deltaTime * Mathf.Clamp(GetVeloRatio(), 0.1f, 1.0f); //Mathf.Lerp(1.0f, speedRatio, GetVeloRatio());

        if (timeUntilNextFootstep <= 0.0f)
        {
            timeUntilNextFootstep = minTime;
            return true;
        }

        return false;
    }

    private float GetVeloRatio()
    {
        Vector3 flatVel = new Vector3(GetVelocity().x, 0f, GetVelocity().z);
        return (flatVel.magnitude / maxMoveSpeed);
    }

    private float GetIntensity()
    {
        const float normalMin = 0.0f;
        const float normalMax = 0.49f;

        return Mathf.Lerp(normalMin, normalMax, GetVeloRatio());
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
