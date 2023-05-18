using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public ObjectiveScript OBJSC;
    [SerializeField] InfoPopup infoPop;
    [Header("Objective Realted")]
    public static bool GeneratorOn;
    public static bool PVTMAcquired;
    public static bool FlashlightAcquired;
    public static bool ShieldAcquired;
    public static bool FirstCameraConnected;
    public static bool AnotherCameraConnected;
    public static bool TrapArmed;
    public static bool SplitjawTrapped;
    public static bool ValidSplitjawPic;
    bool switchTextNeeded = true;

    [Header("Other")]
    public GameObject Warehouse;
    public static bool debug = false;

    void Start()
    {
        debug = false;
        GeneratorOn = false;
        PVTMAcquired = false;
        FlashlightAcquired = false;
        ShieldAcquired = false;
        FirstCameraConnected = false;
        AnotherCameraConnected = false;
        TrapArmed = false;
        SplitjawTrapped = false;
        ValidSplitjawPic = false;
    }

    void Update()
    {
        // MORE SECRET DEV TOOLS SHHHHHHHHHHHHHHH :)
        if(debug && Input.GetKeyDown(KeyCode.P) && !GeneratorOn){
            PowerRestored();
        }
        if(debug && Input.GetKeyDown(KeyCode.LeftBracket) && !SplitjawTrapped){
            SplitjawContained();
        }
        if(debug && Input.GetKeyDown(KeyCode.RightBracket) && !ValidSplitjawPic){
            SplitjawDocumented();
        }
        if(!debug && Input.GetKeyDown(KeyCode.BackQuote)){
            debug = true;
        }
    }

    public void PowerRestored(){
        GeneratorOn = true;
        Debug.Log("POWER RESTORED");
        Warehouse.GetComponent<WarehouseMaker>().warehouse.TurnOnLights();
        // toggle on light SFX
        foreach (var lightGen in Warehouse.GetComponent<WarehouseMaker>().lightSFXGenerators)
        {
            foreach (var emitter in lightGen.GetEmitters())
                emitter.Play();
        }
        //OBJSC.RemoveFromActiveUI(OBJSC.PowerObj);
        if(!PVTMAcquired){
            //OBJSC.AddToActiveUI(OBJSC.PVTMObj);
            OBJSC.activateObjective(ObjectiveScript.ojbectives.pvtm);
        }
        else if(PVTMAcquired){
            //OBJSC.AddToActiveUI(OBJSC.CameraObj);
            OBJSC.activateObjective(ObjectiveScript.ojbectives.camera);
        }
        // code/sounds/animations/UI for after turning on power
        OBJSC.deActivateObjective(ObjectiveScript.ojbectives.power);
    }

    public void PVTMObtained(){
        PVTMAcquired = true;
        Debug.Log("PVTM ACQUIRED");
        // code/sounds/animations/UI for after acquiring PVTM
        //OBJSC.RemoveFromActiveUI(OBJSC.PVTMObj);
        //add switch ui if first pickup
        if (switchTextNeeded) { OBJSC.activateObjective(ObjectiveScript.ojbectives.switchItems); switchTextNeeded = false; }
        infoPop.wakeUp(PauseMenu.techPage.pvtm);
        if(GeneratorOn){
            //OBJSC.AddToActiveUI(OBJSC.CameraObj);
            OBJSC.activateObjective(ObjectiveScript.ojbectives.camera);
        }
        OBJSC.deActivateObjective(ObjectiveScript.ojbectives.pvtm);
    }

    public void FlashlightObtained(){
        FlashlightAcquired = true;
        OBJSC.inputCounter = 1;
        Debug.Log("FLASHLIGHT ACQUIRED");
        // code/sounds/animations/UI for after acquiring Flashlight
        //OBJSC.RemoveFromActiveUI(OBJSC.FlashObj);
        //add switch ui if first pickup
        if (switchTextNeeded) { OBJSC.activateObjective(ObjectiveScript.ojbectives.switchItems); switchTextNeeded = false; }
        OBJSC.deActivateObjective(ObjectiveScript.ojbectives.flash);
    }

    public void ShieldObtained(){
        ShieldAcquired = true;
        Debug.Log("SHIELD OBTAINED");
        infoPop.wakeUp(PauseMenu.techPage.trpSh);
        // code/sounds/animations/UI for after acquiring Shield
        //add switch ui if first pickup
        if (switchTextNeeded) { OBJSC.activateObjective(ObjectiveScript.ojbectives.switchItems); switchTextNeeded = false; }
    }

    public void FirstCameraLinked(){
        FirstCameraConnected = true;
        Debug.Log("FIRST CAMERA LINKED");
        // code/sounds/animations/UI for after linking the first camera
        OBJSC.activateObjective(ObjectiveScript.ojbectives.document);
        OBJSC.deActivateObjective(ObjectiveScript.ojbectives.camera);
        //OBJSC.RemoveFromActiveUI(OBJSC.CameraObj);
        //OBJSC.AddToActiveUI(OBJSC.DocumentObj);
    }

    public void AnotherCameraLinked(){
        AnotherCameraConnected = true;
        Debug.Log("ANOTHER CAMERA LINKED");
        // code/sounds/animations/UI for after linking another camera
    }

    public void TrapSet(){
        TrapArmed = true;
        Debug.Log("TRAP ARMED");
        // code/sounds/animations/UI for after setting a trap
    }

    public void SplitjawContained(){
        SplitjawTrapped = true;
        Debug.Log("SPLITJAW CONTAINED");
        // code/sounds/animations/UI for after containing splitjaw
        //OBJSC.AddToActiveUI(OBJSC.EscapeObj);
        OBJSC.activateObjective(ObjectiveScript.ojbectives.escape);
    }

    public void SplitjawDocumented(){
        ValidSplitjawPic = true;
        Debug.Log("SPLITJAW DOCUMENTED");
        // code/sounds/animations/UI for after documenting splitjaw
        OBJSC.activateObjective(ObjectiveScript.ojbectives.escape);
        OBJSC.deActivateObjective(ObjectiveScript.ojbectives.document);
        //OBJSC.RemoveFromActiveUI(OBJSC.DocumentObj);
        //OBJSC.AddToActiveUI(OBJSC.EscapeObj);

        // Audio
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("ChaseType", (float)MixerController.CHASE_TYPE.FINAL);
    }

    public bool IsPowerRestored(){
        return GeneratorOn;
    }

    public bool IsPVTMObtained(){
        return PVTMAcquired;
    }

    public bool IsFlashlightObtained(){
        return FlashlightAcquired;
    }

    public bool IsSheildObtained(){
        return ShieldAcquired;
    }

    public bool IsFirstCameraLinked(){
        return FirstCameraConnected;
    }

    public bool IsAnotherCameraLinked(){
        return AnotherCameraConnected;
    }

    public bool IsTrapSet(){
        return TrapArmed;
    }
    
    public bool IsSplitjawConatied(){
        return SplitjawTrapped;
    }

    public bool IsSplitjawDocumented(){
        return ValidSplitjawPic;
    }
}
