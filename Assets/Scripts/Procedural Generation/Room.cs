using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Room class contains information about its type, what exits it
 * has, its prefab GameObject, and where it lies within the warehouse.
 * Contains functionality for bounds checking, returning another Room
 * based on a direction, creating possible exits, finding adjacent Landmark
 * rooms, finding an adjacent room with the least amount of exits, and
 * instantiating and placing a prefab based on its type, position, and exits.
 */
public class Room : MonoBehaviour
{
    private Warehouse warehouse;
    public  GameObject obj;
    public  Landmark type;
    public  Exits exits = new Exits(false);
    public  bool  exitsMade = false;
    public  int   column;
    public  int   row;

    public Room(int row, int column, Warehouse warehouse){
        this.row = row;
        this.column = column;
        this.warehouse = warehouse;
        this.type = this.IsStartRoom() ? new Start() : new Generic();
    }

    ~Room(){
        exitsMade = false;
        warehouse = null;
        exits     = null;
        type      = null;
    }

    /* LoadAndPlace() will load a prefab based on its type then create and
     * position an empty parent object, instantiate the prefab, then move
     * the prefab to the location of the empty parent object. It returns
     * an empty parent object that contains a nested prefab object.
     */
    public GameObject LoadAndPlace(){
        (UnityEngine.Object prefab, int rotation) = this.type.LoadPrefab(this.exits);
        GameObject emptyParent = new GameObject("Room " + this.row + " " + this.column);
        emptyParent.transform.position = new Vector3((float)this.column * 20.0f, 0.0f, (float)this.row * -20.0f);
        GameObject prefabObj = (GameObject)Instantiate(prefab, emptyParent.transform);
        prefabObj.transform.position = emptyParent.transform.position;
        prefabObj.transform.Rotate(0, rotation, 0);
        obj = emptyParent;
        return emptyParent;
    }
    
    /* MakeExitsToConsider() creates a list of exits that the
     * current room could possibly have with respect to bounds
     * checking as well as the exits the room already has. It
     * returns an Exits object that contains exits the room could have.
     */
    public Exits MakeExitsToConsider (bool flag){
        Exits toConsider = new Exits(true);
        // remove exits the room already has for consideration
        foreach(string dir in this.exits.types){ 
            toConsider.Remove(dir);
        }
        // dont consider exits to that lead to landmark rooms
        if(flag){
            List<Room> adjacentLandmarks = this.FindAdjacentLandmarks();
            foreach(Room landmark in adjacentLandmarks){
                toConsider.Remove(this.RoomToDirection(landmark));
            }
        }
        // remove exits with respect to bounds checking
        if(this.IsOnRightEdge()) { toConsider.Remove("Right");}
        if(this.IsOnLeftEdge())  { toConsider.Remove("Left"); }
        if(this.IsOnBottomEdge()){ toConsider.Remove("Down"); }
        if(this.IsOnTopEdge())   { toConsider.Remove("Up");   }
        return toConsider;
    }

    /* FindAdjacentLandmarks() will check if anu surrounding rooms are
     * not Generic rooms. If any are it will add them to that list. It
     * then returns that list.
     */
    public List<Room> FindAdjacentLandmarks(){
        List<Room> adjacents = new List<Room>();
        // add above room to list if its a landmark
        if(!this.IsOnTopEdge()){ 
            if(this.RoomAbove().type.GetType() != typeof(Generic)){
                adjacents.Add(this.RoomAbove());
            }
        }
        if(!this.IsOnBottomEdge()){ 
            if(this.RoomUnderneath().type.GetType() != typeof(Generic)){
                adjacents.Add(this.RoomUnderneath());
            }
        }
        if(!this.IsOnLeftEdge()){ 
            if(this.RoomToLeft().type.GetType() != typeof(Generic)){
                adjacents.Add(this.RoomToLeft());
            }
        }
        if(!this.IsOnRightEdge()){ 
            if(this.RoomToRight().type.GetType() != typeof(Generic)){
                adjacents.Add(this.RoomToRight());
            }
        }
        return adjacents;
    }

    /* FindLeastExits() will examine all adjacent rooms an return the
     * room with the least amount of exits. It first creates possible
     * exits the room could have, checks if the room has exits, if the
     * number of exits it has is greater than leastNum, and then return
     * that room.
     */
    public (Room leastExits, string exit, string opposing) FindLeastExits(){
        int leastNum = 4;
        Room leastExits = null;
        string exit = System.String.Empty;
        string opposing = System.String.Empty;
        Exits possible = this.MakeExitsToConsider(false);
        foreach(string dir in possible.types){
            if(this.DirectionToRoom(dir).exits.NumberOf() != 0 &&
               this.DirectionToRoom(dir).exits.NumberOf() < leastNum){
                opposing   = possible.OppositeDirection(dir);
                exit       = dir;
                leastExits = this.DirectionToRoom(dir);
                leastNum   = this.DirectionToRoom(dir).exits.NumberOf();
            }
        }
        return (leastExits, exit, opposing);
    }

    /* These functions are mainly helper functions that aid in
     * determining a rooms position within a 2D array as well as
     * finding other rooms in that array. There are a few functions
     * that perform bounds checking as well as a distance function
     * from this room to another or this room to another room's
     * coordinates.
     */
    
    // DirectionToRoom() returns a Room object based on a given direction
    public Room DirectionToRoom (string dir){
        if(dir == "Up"    && !this.IsOnTopEdge()){
            return this.RoomAbove();
        }
        if(dir == "Down"  && !this.IsOnBottomEdge()){
            return this.RoomUnderneath();
        }
        if(dir == "Left"  && !this.IsOnLeftEdge()){
            return this.RoomToLeft();
        }
        if(dir == "Right" && !this.IsOnRightEdge()){
            return this.RoomToRight();
        }
        return null;
    }

    // RoomToDirection() returns a direction based on a provided room
    public string RoomToDirection(Room room){
        // if thi is above room, then the direction to room is down
        if(this == room.RoomAbove()){
            return "Down";
        }
        if(this == room.RoomUnderneath()){
            return "Up";
        }
        if(this == room.RoomToRight()){
            return "Left";
        }
        if(this == room.RoomToLeft()){
            return "Right";
        }
        return "None";
    }

    // DistanceTo() return the distance from this room to the argument passed, based on position in 2D array
    public float DistanceTo(int row, int col){
        return Mathf.Sqrt((Mathf.Pow((this.row - row), 2f)) + Mathf.Pow((this.column - col), 2f));
    }

    // DistanceTo() return the distance from this room to the argument passed, based on position in 2D array
    public float DistanceTo(Room room){
        return Mathf.Sqrt((Mathf.Pow((this.row - room.row), 2f)) + Mathf.Pow((this.column - room.column), 2f));
    }

    // Functions used for accessing rooms based on direction
    // takes into account bounds checking
    public Room RoomToLeft(){
        if(!this.IsOnLeftEdge()){
            return warehouse.data[this.row][this.column - 1];
        }
        return null;
    }

    public Room RoomToRight(){
        if(!this.IsOnRightEdge()){
            return warehouse.data[this.row][this.column + 1];
        }
        return null;
    }

    public Room RoomAbove(){
        if(!this.IsOnTopEdge()){
            return warehouse.data[this.row - 1][this.column];
        }
        return null;
    }

    public Room RoomUnderneath(){
        if(!this.IsOnBottomEdge()){
            return warehouse.data[this.row + 1][this.column];
        }
        return null;
    }

    // Check if current room is the start room
    public bool IsStartRoom(){
        return this.row == warehouse.startRow && this.column == warehouse.startCol;
    }

    // Bounds checking related functions
    public bool IsOnLeftEdge(){
        return this.column == 0;
    }

    public bool IsOnRightEdge(){
        return this.column == warehouse.columns - 1;
    }

    public bool IsOnTopEdge(){
        return this.row == 0;
    }

    public bool IsOnBottomEdge(){
        return this.row == warehouse.rows - 1;
    }

    // Random number between two values
    public int RandomNum(int lower, int upper){
        return UnityEngine.Random.Range(lower, upper);
    }

    public void Print(){
        Debug.Log(this.type.GetType() + " ROOM AT " + this.row + " " + this.column);
    }
}