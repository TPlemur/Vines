using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
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

    [Header("Other")]
    public GameObject Warehouse;
    
    void Start()
    {
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
        if(Input.GetKeyDown(KeyCode.P) && !GeneratorOn){
            PowerRestored();
        }
        if(Input.GetKeyDown(KeyCode.LeftBracket) && !SplitjawTrapped){
            SplitjawContained();
        }
        if(Input.GetKeyDown(KeyCode.RightBracket) && !ValidSplitjawPic){
            SplitjawDocumented();
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
        // code/sounds/animations/UI for after turning on power
    }

    public void PVTMObtained(){
        PVTMAcquired = true;
        Debug.Log("PVTM ACQUIRED");
        // code/sounds/animations/UI for after acquiring PVTM
    }

    public void FlashlightObtained(){
        FlashlightAcquired = true;
        Debug.Log("FLASHLIGHT ACQUIRED");
        // code/sounds/animations/UI for after acquiring Flashlight
    }

    public void ShieldObtained(){
        ShieldAcquired = true;
        Debug.Log("SHIELD OBTAINED");
        // code/sounds/animations/UI for after acquiring Shield
    }

    public void FirstCameraLinked(){
        FirstCameraConnected = true;
        Debug.Log("FIRST CAMERA LINKED");
        // code/sounds/animations/UI for after linking the first camera
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
    }

    public void SplitjawDocumented(){
        ValidSplitjawPic = true;
        Debug.Log("SPLITJAW DOCUMENTED");
        // code/sounds/animations/UI for after documenting splitjaw
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
