using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chirper : Item
{

    LayerMask layer;
    GameObject dotPrefab;
    ElectricalEquipment scanner;

    private void Start()
    {
        dotPrefab = Resources.Load("Prefabs/Chirper_I", typeof(GameObject)) as GameObject;
    }

    public void setup(Camera pCam, LayerMask mask, GameObject stateManager, GameObject UIElement)
    {
        playerCam = pCam;
        gameState = stateManager.GetComponent<GameStateManager>();
        ItemUI = UIElement;
        layer = mask;
        LoadItem("Chirper");
    }

    //Throw a scannerDot
    public override void Primary()
    {
        GameObject beacon = Instantiate(dotPrefab, playerCam.transform.position + playerCam.transform.forward, Quaternion.Euler( Vector3.zero), gameState.transform);
        beacon.GetComponentInChildren<Animator>().enabled = true;
        InvestigateHintBehaviour.Lure = beacon;
        Brain.currentTarget = Brain.target.lure;
        Brain.investigating = true;



        //removefromInventory
        ObjectiveScript.equipedisEE = true;
        InventoryManager im = FindObjectOfType<InventoryManager>();
        int ChirpIndex = im.inventory.getCurrentIndex();
        im.inventory.setToZeroth();
        im.inventory.items.RemoveAt(ChirpIndex);
        Destroy(itemObj);
        Destroy(this.gameObject);
    }

}
