using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PhysFootstepController : MonoBehaviour
{
    public enum TERRAIN_TYPES { CONCRETE, METAL, NONE };

    private PlayerMovement charController = null;
    private Rigidbody rb;

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

    [SerializeField] [Range(0.0f, 1.0f)]
    private float splashMultiplier = 1.0f;
    private float waterAmount = 0.0f;

    // if there is a studio event emiitor on the gameobject it will be used to set the parameters of each footstep created (though some may be overridden by the controller script)
    private FMODUnity.StudioEventEmitter referenecEvent = null;

    public void SetTerrainType(TERRAIN_TYPES type) { currentTerrain = type; }

    // encapsulated functions in case the character controller was to change for whatever reason, these functions could just be changed
    public bool IsGrounded() { return charController.IsGrounded(); }
    public bool IsCrouching() { return charController.IsCrouching(); }

    public event System.Action stepped;

    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();

        if (referenecEvent == null)
            referenecEvent = GetComponent<FMODUnity.StudioEventEmitter>();

        SetTerrainType(defaultTerrain);

        if (maxMoveSpeed == -1.0f)
            maxMoveSpeed = charController.moveSpeed;
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
        if (!charController.IsGrounded())
            return false;
        const float veloCutoff = 0.1f;
        if ((new Vector3(rb.velocity.x, 0f, rb.velocity.z)).magnitude <= veloCutoff)
            return false;

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
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        return (flatVel.magnitude / maxMoveSpeed);
    }

    private float GetIntensity()
    {
        const float normalMin = 0.0f;
        const float normalMax = 0.49f;

        return Mathf.Lerp(normalMin, normalMax, GetVeloRatio());
    }

    private float GetWaterAmount()
    {
        return waterAmount * splashMultiplier;
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
        }
        else
        {
            footstep = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Player Footsteps");
        }

        // footset controller param controls
        footstep.setParameterByName("Terrain", (float)currentTerrain);
        footstep.setParameterByName("Intensity", GetIntensity());
        footstep.setParameterByName("Water Splash", GetWaterAmount());
        // 3D position
        footstep.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        footstep.start();
        footstep.release();
    }

    public void SelectAndPlayFootstep()
    {
        DetermineTerrain();
        PlayFootstep();
        stepped?.Invoke();
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

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Water")
        {
            const float puddleWaterAmountMax = 1.0f;
            waterAmount = puddleWaterAmountMax;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.tag == "Water")
        {
            waterAmount = 0.0f;
            //StartCoroutine(LERPWaterAmount(0.0f, 0.25f));
        }
    }

    IEnumerator LERPWaterAmount(float target, float time)
    {
        float start = waterAmount;
        float duration = time;
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            waterAmount = Mathf.Lerp(start, target, 1.0f - (duration / time));
            yield return null;
        }
        waterAmount = target;
    }
}
