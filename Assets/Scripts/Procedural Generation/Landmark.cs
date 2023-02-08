using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Landmark{
    // comments for rotating prefab
    // private int RotatePrefab(List<Direction> exits){
    //     if(exits.Count == 2){
    //         var opposite = exits[0].OppositeDirection();
    //         if(exits.Contains(opposite)){
    //             if(exits.Contains(Direction.LEFT) && exits.Contains(Direction.RIGHT)){
    //                 return 90;
    //             }
    //         }
    //         else{
    //             if(exits.Contains(Direction.DOWN) && exits.Contains(Direction.RIGHT)){
    //                 return 90;
    //             }
    //             if(exits.Contains(Direction.DOWN) && exits.Contains(Direction.LEFT)){
    //                 return 180;
    //             }
    //             if(exits.Contains(Direction.LEFT) && exits.Contains(Direction.UP)){
    //                 return -90;
    //             }
    //         }
    //     }
    //     else if(exits.Count == 3){
    //         if(exits.Contains(Direction.DOWN) && 
    //             exits.Contains(Direction.RIGHT) && 
    //             exits.Contains(Direction.UP)){
    //             return 180;
    //         }
    //         if(exits.Contains(Direction.DOWN) && 
    //             exits.Contains(Direction.RIGHT) && 
    //             exits.Contains(Direction.LEFT)){
    //             return -90;
    //         }
    //         if(exits.Contains(Direction.LEFT) && 
    //             exits.Contains(Direction.RIGHT) && 
    //             exits.Contains(Direction.UP)){
    //             return 90;
    //         }
    //     }
    //     return 0;
    // }

    
    // public GameObject LoadPrefab(int x, int z){

    // }
}

public class GenericRoom : Landmark{
    private string pathToPrefab = "ProcgenGreyboxes/room-";
}

public class StartRoom : Landmark{
    private string pathToPrefab = "ProcgenGreyboxes/room-elevator-";
}

public class MonsterRoom : Landmark{
    private string pathToPrefab = "ProcgenGreyboxes/room-";
    
}

public class PVTMRoom : Landmark{
    private string pathToPrefab = "ProcgenGreyboxes/room-pvtm-";
    
}

public class CameraRoom : Landmark{
    private string pathToPrefab = "ProcgenGreyboxes/room-";
    
}

public class GeneratorRoom : Landmark{
    private string pathToPrefab = "ProcgenGreyboxes/room-";
    
}

