using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Item
{
    private Vector3 original;
    private bool toggled = false;

    public Shield(GameObject stateManager) : base(stateManager){
        LoadItem("Shield");
        original = itemObj.transform.localPosition;
        gameState.ShieldObtained();
    }

    public override void Secondary(){
        toggled = !toggled ? true : false;
        itemObj.transform.localPosition = toggled ? new Vector3((float) 0.36, (float) 0.43, (float) -0.54) : original;
    }

    public override bool IsToggled(){
        return toggled;
    }

    public void explode(){
        Debug.Log("EXPLODING SHIELD");
        // return SHinst.GetComponentInChildren<ShieldDeployer>().findWidth(SHinst.transform);
    }
}
