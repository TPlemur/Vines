using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PhysFootstepController : MonoBehaviour
{
    public enum TERRAIN_TYPES { CONCRETE, METAL, NULL };

    private PlayerMovement charController = null;
    private Rigidbody rb;

    [SerializeField]
    private TERRAIN_TYPES defaultTerrain = TERRAIN_TYPES.CONCRETE;
    private TERRAIN_TYPES currentTerrain = TERRAIN_TYPES.CONCRETE;

    private FMOD.Studio.EventInstance footstep;

    [SerializeField]
    private float minTime = 0.5f;
    [SerializeField]
    private float maxTime = 0.25f;

    private float timeUntilNextFootstep = 0.0f;

    [SerializeField]
    private float maxMoveSpeed = -1.0f;

    public float test1;
    public float test2;

    public void SetTerrainType(TERRAIN_TYPES type) { currentTerrain = type; }

    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();

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
        if (Input.GetMouseButtonDown(0))
        {
            SelectAndPlayFootstep();
        }
    }

    private bool ShouldTriggerFootstep()
    {
        if (!charController.IsGrounded())
            return false;
        const float veloCutoff = 0.1f;
        if ((new Vector3(rb.velocity.x, 0f, rb.velocity.z)).magnitude <= veloCutoff)
            return false;

        float speedRatio = minTime / maxTime;

        test1 = GetVeloRatio();
        test2 = Mathf.Lerp(1.0f, speedRatio, GetVeloRatio());

        timeUntilNextFootstep -= Time.deltaTime * Mathf.Lerp(1.0f, speedRatio, GetVeloRatio());

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
        const float normalMax = 0.5f;

        return Mathf.Lerp(normalMin, normalMax, GetVeloRatio());
    }

    public void PlayFootstep()
    {
        footstep = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Player Footsteps");
        footstep.setParameterByName("Terrain", (float)currentTerrain);
        footstep.setParameterByName("Intensity", GetIntensity());
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
    }
}
