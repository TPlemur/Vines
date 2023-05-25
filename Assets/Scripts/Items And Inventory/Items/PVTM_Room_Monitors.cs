using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PVTM_Room_Monitors : MonoBehaviour
{
    private GameObject[] cams;
    private GameObject[] worldCams;
    private bool positionsSet = false;
    public GameStateManager stateManager;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(!positionsSet && stateManager.IsPowerRestored()){
            // Finds cameras in the world space
            cams = GameObject.FindGameObjectsWithTag("Camera");
            worldCams = GameObject.FindGameObjectsWithTag("WorldCam");

            // Ensures only run once // Note to self: Potentially work as co-routine?
            positionsSet = true;

            // Sets camera positions and rotation
            for(int i=0; i<4; i++){
                worldCams[i].transform.SetParent(cams[i].transform);
                worldCams[i].gameObject.transform.localPosition = cams[i].transform.GetChild(0).gameObject.transform.localPosition;
                worldCams[i].gameObject.transform.localRotation = cams[i].transform.GetChild(0).gameObject.transform.localRotation;
            }
        }
    }
}
