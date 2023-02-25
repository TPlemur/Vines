using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmark{
    public GameObject warehouseEmpty;
    public float maxDist;
    public float minDist;
    public string path;
    public int weight;
    public int rotation;
    public string num;

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

    /* LoadPrefab() is overwritten by child classes but essentially loads
     * a prefab based on a Landmarks type.
     */
    public virtual (UnityEngine.Object prefab, int rotation) LoadPrefab(Exits dirs){
        // overwritten by child classes
        return (null, 0);
    }
    
    // load landmark comments
    public (UnityEngine.Object prefab, int rotation) LoadLandmark(Exits dirs){
        UnityEngine.Object roomPrefab = null;
        int rotation = 0;
        (string type, int rot) = this.GetRotationAndType(dirs);
        roomPrefab = Resources.Load(path + type);
        rotation = rot;
        return (roomPrefab, rotation);
    }

    // Used for Generic and PVTMCamera room prefab paths
    public string BuildExitString(Exits dirs){
        string exits = System.String.Empty;
        exits += dirs.Has("Left")  ? "L" : "_";
        exits += dirs.Has("Up")    ? "U" : "_";
        exits += dirs.Has("Down")  ? "D" : "_";
        exits += dirs.Has("Right") ? "R" : "_";
        return exits;
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

// Child classes of Landmark, holds information about a file 
// path where the prefab can be found and its minimum and the
// maximum spawn distance from the starting room.
public class Generic : Landmark{
    public Generic(){
        path = "ProcgenGreyboxesTextured/room-";
    }
    public override (UnityEngine.Object prefab, int rotation) LoadPrefab(Exits dirs){
        path += this.BuildExitString(dirs);
        path += UnityEngine.Random.Range(0, 2) == 0 ? "-thin" : "-wide";
        return (Resources.Load(path), 0);
    }
}

public class Start : Landmark{
    public Start(){
        path = "ProcgenGreyboxesTextured/room-elevator-";
    }
    public override (UnityEngine.Object prefab, int rotation) LoadPrefab(Exits dirs){
        return this.LoadLandmark(dirs);
    }
}

//  Monster Spawn Specific
public class Monster : Landmark{
    public Monster(string type, int rot){
        path = "ProcgenGreyboxesTextured/room-monster-quadrant";
        rotation = rot;
        num = type;
    }
    public override (UnityEngine.Object prefab, int rotation) LoadPrefab(Exits dirs){
        return (Resources.Load(path + num), rotation);
    }
}

// Notable Landmark rooms like AlphaTeam, Generator, etc.
public class AlphaTeam : Landmark{
    public AlphaTeam(){
        path = "ProcgenGreyboxesTextured/room-pvtm-";
        minDist = 1.3f;
        maxDist = 2.5f;
    }
    public override (UnityEngine.Object prefab, int rotation) LoadPrefab(Exits dirs){
        return this.LoadLandmark(dirs);
    }
}

public class Generator : Landmark{
    public Generator(){
        path = "ProcgenGreyboxesTextured/room-generator-";
        minDist = 1f;
        maxDist = 1.5f;
    }
    public override (UnityEngine.Object prefab, int rotation) LoadPrefab(Exits dirs){
        return this.LoadLandmark(dirs);
    }
}

public class ShieldRoom : Landmark{
    public ShieldRoom(){
        path = "ProcgenGreyboxesTextured/room-shield-";
        minDist = 1.2f;
        maxDist = 3f;
    }
    public override (UnityEngine.Object prefab, int rotation) LoadPrefab(Exits dirs){
        return this.LoadLandmark(dirs);
    }
}

// Trip wire, Camera, and Hiding Rooms
public class TripWire : Landmark{
    public TripWire(){
        path = "ProcgenGreyboxesTextured/room-trap-";
        minDist = 1f;
        maxDist = 4f;
    }
    public override (UnityEngine.Object prefab, int rotation) LoadPrefab(Exits dirs){
        return this.LoadLandmark(dirs);
    }
}

public class PVTMCamera : Landmark{
    public PVTMCamera(){
        path = "ProcgenGreyboxesTextured/room-";
        minDist = 1f;
        maxDist = 3f;
    }
    public override (UnityEngine.Object prefab, int rotation) LoadPrefab(Exits dirs){
        path += this.BuildExitString(dirs);
        path += "-cam";
        return (Resources.Load(path), 0);
    }
}

public class Hide : Landmark{
    public Hide(){
        path = "ProcgenGreyboxesTextured/room-";
        minDist = 1f;
        maxDist = 4f;
    }
    public override (UnityEngine.Object prefab, int rotation) LoadPrefab(Exits dirs){
        path += this.BuildExitString(dirs);
        path += "-hide";
        return (Resources.Load(path), 0);
    }
}