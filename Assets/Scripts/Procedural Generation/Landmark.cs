using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmark{
    public GameObject warehouseEmpty;
    public float maxDist;
    public float minDist;
    public string path;
    public int weight;

    /* GetRotationAndType() calculates the type of a room based off the number
     * of exits it has. It calculates a rotation based on the exits the room has
     * as well as the way in which they are oriented. It returns a tuple, containing
     * a string for the type of room and an int for the rotation.
     */
    public (string type, int rot) GetRotationAndType(Exits dirs){
        if(dirs.NumberOf() == 2){
            string opposite = dirs.OppositeDirection(dirs.types[0]);
            if(dirs.Has(opposite)){
                if(dirs.Has("Left") && dirs.Has("Right")){
                    return ("straight", 90);
                }
                return("straight", 0);
            }
            else{
                if(dirs.Has("Down") && dirs.Has("Right")){
                    return ("elbow", 90);
                }
                if(dirs.Has("Down") && dirs.Has("Left")){
                    return ("elbow", 180);
                }
                if(dirs.Has("Left") && dirs.Has("Up")){
                    return ("elbow", -90);
                }
                return ("elbow", 0);
            }
        }
        else if(dirs.NumberOf() == 3){
            if(dirs.Has("Down") && dirs.Has("Right") && dirs.Has("Up")){
                return ("tri", 180);
            }
            if(dirs.Has("Down") && dirs.Has("Right") && dirs.Has("Left")){
                return ("tri", -90);
            }
            if(dirs.Has("Left") && dirs.Has("Right") && dirs.Has("Up")){
                return ("tri", 90);
            }
            return("tri", 0);
        }
        return ("quad", 0);
    }

    // actually loading and placing prefab and attaching it to empty and parent objects
    /* LoadPrefab() will load a prefab given a file path. If the room isn't a generic room
     * then it will calculate the type of room as well as what y rotation to apply.
     * It returns a tuple containing the prefab and its appropriate rotation.
     */
    public (UnityEngine.Object prefab, int rotation) LoadPrefab(Exits dirs){
        UnityEngine.Object roomPrefab = null;
        int rotation = 0;
        if(this.GetType() != typeof(Generic)){
            (string type, int rot) = this.GetRotationAndType(dirs);
            Debug.Log("SPECIAL ROOM PATH " + path + type);
            roomPrefab = Resources.Load(path + type);
            rotation = rot;
        }
        else{
            string roomExits = System.String.Empty;
            roomExits += dirs.Has("Left")  ? "L" : "_";
            roomExits += dirs.Has("Up")    ? "U" : "_";
            roomExits += dirs.Has("Down")  ? "D" : "_";
            roomExits += dirs.Has("Right") ? "R" : "_";
            // List<string> types = new List<string>(new string[] {"-thin", "-wide", "-cam", "-hide"});
            // roomExits += types[UnityEngine.Random.Range(0, 4)];
            roomExits += UnityEngine.Random.Range(0, 2) == 0 ? "-thin" : "-wide";
            // Debug.Log("GENERIC ROOM TYPE" + path + roomExits);
            roomPrefab = Resources.Load(path + roomExits);
        }
        return (roomPrefab, rotation);
    }

    // PlaceLandmark() will determine whether a Landmark is chosen given its 
    // current weight. If it isn't chosen then it will decrement its weight.
    public bool PlaceLandmark(){
        if(UnityEngine.Random.Range(0, weight) == 0){
            return true;
        }
        weight -= 1;
        return false;
    }

    /* InRange() will determine if a provided distance is between the Landmarks
     * minimum and maximum spawn distance. This is used to place Landmarks 
     * at certain distances away from the starting room.
     */ 
    public bool InRange(float dist){
        if(dist >= minDist && dist <= maxDist){
            return true;
        }
        return false;
    }
}

// Child classes of Landmark, holds information about its weight, a
// file path where the prefab can be found and its minimum and 
// maximum spawn distance from the starting room.
public class Generic : Landmark{
    public Generic(){
        path = "ProcgenGreyboxes/room-";
    }
}

public class Start : Landmark{
    public Start(){
        path = "ProcgenGreyboxes/room-elevator-";
    }
}

public class Monster : Landmark{
    public Monster(){
        path = "ProcgenGreyboxes/room-pvtm-";
        weight  = 4;
        minDist = 2.2f;
        maxDist = 4f;
    }
}

public class AlphaTeam : Landmark{
    public AlphaTeam(){
        path = "ProcgenGreyboxes/room-pvtm-";
        weight  = 8;
        minDist = 1f;
        maxDist = 2f;
    }
}

public class PVTMCamera : Landmark{
    public PVTMCamera(){
        path = "ProcgenGreyboxes/room-pvtm-";
        weight  = 6;
        minDist = 1.5f;
        maxDist = 3f;
    }
}

public class Generator : Landmark{
    public Generator(){
        path = "ProcgenGreyboxes/room-pvtm-";
        weight  = 8;
        minDist = 1f;
        maxDist = 2f;
    }
}

