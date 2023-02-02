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
    private bool populateOnStart = false;
    [SerializeField]
    private bool muteAndDestroyReference = true;

    // Start is called before the first frame update
    void Start()
    {
        if (referenceEvent == null)
            referenceEvent = GetComponent<FMODUnity.StudioEventEmitter>();

        if (populateOnStart)
            StartCoroutine(PopulateAfterSeconds(0.001f));

        if (muteAndDestroyReference)
            referenceEvent.Stop();
    }

    IEnumerator PopulateAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        Populate();
    }

    public void Populate()
    {
        foreach (GameObject obj in GetGameObjects())
        {
            if (Random.value > probability)
                continue;

            var emitter = obj.AddComponent<FMODUnity.StudioEventEmitter>();
            // copy referenec event component fields
            System.Reflection.FieldInfo[] fields = referenceEvent.GetType().GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(emitter, field.GetValue(referenceEvent));
            }
            // because the object is already started, manually fulfill this requirement
            if (emitter.PlayEvent == FMODUnity.EmitterGameEvent.ObjectStart)
                emitter.Play();
        }

        if (muteAndDestroyReference)
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
