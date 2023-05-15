using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ScannerBeacon : Item
{
    LayerMask layer;
    GameObject dotPrefab;
    ElectricalEquipment scanner;

    private void Start()
    {
        dotPrefab = Resources.Load("Prefabs/ScannerBeacon_I", typeof(GameObject)) as GameObject;
    }

    public void setup(Camera pCam, LayerMask mask, GameObject stateManager, GameObject UIElement, ElectricalEquipment scannerScript)
    {
        playerCam = pCam;
        gameState = stateManager.GetComponent<GameStateManager>();
        ItemUI = UIElement;
        layer = mask;
        LoadItem("Scanner");
        scanner = scannerScript;
    }

    //Throw a scannerDot
    public override void Primary(){
        GameObject beacon = Instantiate(dotPrefab, playerCam.transform.position + playerCam.transform.forward, playerCam.transform.rotation, gameState.transform);
        scanner.addTarget(beacon);
        beacon.GetComponentInChildren<MeshRenderer>().materials[1] = null;

        //removefromInventory
        InventoryManager im = FindObjectOfType<InventoryManager>();
        int BeaconIndex = im.inventory.getCurrentIndex();
        im.inventory.setToZeroth();
        im.inventory.items.RemoveAt(BeaconIndex);
        Destroy(this.gameObject);
    }


}
