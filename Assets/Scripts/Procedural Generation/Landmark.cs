using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Landmark : MonoBehaviour{
    public GameObject warehouseEmpty;
    public string path;
    
    // comments for rotating prefab
    public (string type, int rot) RotatePrefab(Exits dirs){
        if(dirs.NumberOf() == 2){
            string opposite = dirs.OppositeDirection(dirs.types[0]);
            if(dirs.Has(opposite)){
                if(dirs.Has("Left") && dirs.Has("Right")){
                    return ("straight", 90);
                }
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
        }
        return ("quad", 0);
    }

    // actually loading and placing prefab and attaching it to empty and parent objects
    public void PlacePrefab(int i, int j, Exits dirs){
        GameObject emptyParent = new GameObject("Room " + i + " " + j);
        emptyParent.transform.position = new Vector3((float)j * 20.0f, 0.0f, (float)i * -20.0f);
        emptyParent.transform.SetParent(warehouseEmpty.transform);
        UnityEngine.Object roomPrefab = null;
        GameObject prefabObj = null;
        if(this.GetType() != typeof(Generic)){
            (string type, int rotation) = this.RotatePrefab(dirs);
            roomPrefab = Resources.Load(path + type);
            prefabObj = (GameObject)Instantiate(roomPrefab, emptyParent.transform);
            prefabObj.transform.position = emptyParent.transform.position;
            prefabObj.transform.Rotate(0, rotation, 0);
        }
        else{
            string roomExits = System.String.Empty;
            roomExits += dirs.Has("Left")  ? "L" : "_";
            roomExits += dirs.Has("Up")    ? "U" : "_";
            roomExits += dirs.Has("Down")  ? "D" : "_";
            roomExits += dirs.Has("Right") ? "R" : "_";
            // choose random room width then load and instantiate prefab then move the prefab to the empty gameobject above
            roomExits += UnityEngine.Random.Range(0, 2) == 0 ? "-thin" : "-wide";
            roomPrefab = Resources.Load(path + roomExits); // note: not .prefab!
            prefabObj = (GameObject)Instantiate(roomPrefab, emptyParent.transform);
            prefabObj.transform.position = emptyParent.transform.position;
        }
    }
}

public class Generic : Landmark{
    public Generic(GameObject empty){
        warehouseEmpty = empty;
        path = "ProcgenGreyboxes/room-";
    }
}

public class Start : Landmark{
    public Start(GameObject empty){
        warehouseEmpty = empty;
        path = "ProcgenGreyboxes/room-elevator-";
    }
}

public class Monster : Landmark{
    public Monster(GameObject empty){
        warehouseEmpty = empty;
        path = "ProcgenGreyboxes/room-";
    }
}

public class AlphaTeam : Landmark{
    public AlphaTeam(GameObject empty){
        warehouseEmpty = empty;
        path = "ProcgenGreyboxes/room-pvtm-";
    }
}

public class PVTMCamera : Landmark{
    public PVTMCamera(GameObject empty){
        warehouseEmpty = empty;
        path = "ProcgenGreyboxes/room-";
    }
}

public class Generator : Landmark{
    public Generator(GameObject empty){
        warehouseEmpty = empty;
        path = "ProcgenGreyboxes/room-";
    }
}

