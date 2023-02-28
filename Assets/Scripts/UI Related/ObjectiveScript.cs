using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveScript : MonoBehaviour
{
    private int inputCounter = 0;

    [SerializeField] public static bool FlashObjBool = false;
    [SerializeField] public static bool PVTMObjBool = false;
    [SerializeField] public static bool PowerObjBool = false;
    [SerializeField] public static bool DocumentObjBool = false;
    [SerializeField] public static bool EscapeObjBool = false;
    public GameObject MoveText;
    public GameObject PowerObj;
    public GameObject PVTMObj;
    public GameObject DocumentObj;
    public GameObject EscapeObj;
    public GameObject FlashObj;
    // Start is called before the first frame update
    void Start()
    {
        MoveText.SetActive(true);
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)) && (inputCounter < 3))
        {
            inputCounter++;
            if (inputCounter == 3)
            {
                MoveText.SetActive(false);
                FlashObjBool = true;
            }
            
        }

        FlashObj.SetActive(FlashObjBool);
        PowerObj.SetActive(PowerObjBool);
        PVTMObj.SetActive(PVTMObjBool);
        DocumentObj.SetActive(DocumentObjBool);
        EscapeObj.SetActive(EscapeObjBool);

    }
    
}
