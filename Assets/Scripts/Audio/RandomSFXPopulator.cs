using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RandomSFXPopulator : MonoBehaviour
{
    private FMODUnity.StudioEventEmitter referenceEvent = null;

    public enum MATCH_TYPES { EXACT, CONTAINS };

    [SerializeField]
    private string gameObjectName = "";
    [SerializeField]
    private MATCH_TYPES matchType = MATCH_TYPES.EXACT;
    [SerializeField]
    private float probability;
    [SerializeField]
    private int maxNumber = -1;

    [SerializeField]
    private bool createNewObject = true;

    [SerializeField]
    private bool populateOnStart = false;
    [SerializeField]
    private bool destroyReference = true;
    [SerializeField]
    private bool playSoundOnStart = false;

    [SerializeField]
    private bool retainReferenecs = false;

    private List<FMODUnity.StudioEventEmitter> emitters = new List<FMODUnity.StudioEventEmitter>();

    public List<FMODUnity.StudioEventEmitter> GetEmitters() { return emitters; }

    // Start is called before the first frame update
    void Start()
    {
        if (referenceEvent == null)
            referenceEvent = GetComponent<FMODUnity.StudioEventEmitter>();

        if (populateOnStart)
            StartCoroutine(PopulateAfterSeconds(0.001f));

        if (destroyReference)
            referenceEvent.Stop();
    }

    IEnumerator PopulateAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        Populate();
    }

    public void Populate()
    {
        foreach (GameObject matchObj in GetGameObjects())
        {
            if (Random.value > probability)
                continue;

            GameObject obj;
            if (createNewObject)
            {
                obj = new GameObject("Generated SFX");
                obj.transform.parent = gameObject.transform;
                obj.transform.position = matchObj.transform.position;
                obj.transform.rotation = matchObj.transform.rotation;
            }
            else
                obj = matchObj;

            var emitter = obj.AddComponent<FMODUnity.StudioEventEmitter>();
            // copy referenec event component fields
            System.Reflection.FieldInfo[] fields = referenceEvent.GetType().GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(emitter, field.GetValue(referenceEvent));
            }
            // because the object is already started, manually fulfill this requirement
            if (playSoundOnStart || emitter.PlayEvent == FMODUnity.EmitterGameEvent.ObjectStart)
                emitter.Play();

            if (retainReferenecs)
                emitters.Add(emitter);
        }

        if (destroyReference)
        {
            referenceEvent.Stop();
            Destroy(referenceEvent);
        }
    }

    private bool IsMatch(GameObject obj)
    {
        switch(matchType)
        {
            case MATCH_TYPES.EXACT:
                return obj.name == gameObjectName;
            case MATCH_TYPES.CONTAINS:
                return obj.name.Contains(gameObjectName);
        }

        return false;
    }

    private IEnumerable<GameObject> GetGameObjects()
    {
        return Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => IsMatch(obj));
    }
}
