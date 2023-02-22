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
    public GameObject obj;
    public Landmark type;
    public Exits exits = new Exits(false);
    public bool exitsMade = false;
    public bool placed = false;
    public int column;
    public int row;

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

    /* ConnectToRandom() will connect a random to random exits. It
     * first creates a list of possible rooms to connect to that
     * aren't of type Start or Monster. It then determines the number
     * of rooms to connect to and then chooses random rooms to connect to.
     */
    public void ConnectToRandom(){
        List<Landmark> temp = new List<Landmark>(new Landmark[] {new Monster("1", 0), new Start()});
        List<Room> possible = this.RoomsToNotOfType(temp);
        int numExits = 0;
        if(this.IsStartRoom()){
            numExits = 3;
        }
        else if(possible.Count == 1){
            numExits = 1;
        }
        else if(possible.Count > 1){
            numExits = UnityEngine.Random.Range(2, possible.Count - 2);
        }
        for(;numExits > 0; numExits--){
            int index = UnityEngine.Random.Range(0, possible.Count);
            this.ConnectTo(possible[index]);
            possible.RemoveAt(index);
        }
    }

    /* RoomsToConnect() creates a list of possible rooms that
     * could be connedted to with respect to bounds checking.
     */
    public List<Room> RoomsToConnect(){
        List<Room> possible = new List<Room>();
        if(!this.IsOnRightEdge()) { possible.Add(this.RoomToRight());}
        if(!this.IsOnLeftEdge())  { possible.Add(this.RoomToLeft());}
        if(!this.IsOnBottomEdge()){ possible.Add(this.RoomUnderneath());}
        if(!this.IsOnTopEdge())   { possible.Add(this.RoomAbove());}
        return possible;
    }

    /* NotAlreadyLinkedRooms() creates a list of possible rooms that
     * could be connedted to with respect to bounds checking as well as
     * what exits the room already has. So if a room has a left exit and
     * it is to the left of a room with the possible list. It removes that
     * room as it is alread connected to that room.
     */
    public List<Room> NotAlreadyLinkedRooms(){
        List<Room> possible = this.RoomsToConnect();
        foreach(string dir in this.exits.types){
            for(int i = 0; i < possible.Count; i++){
                if(dir == "Left" && this.IsToRightOf(possible[i])){
                    possible.RemoveAt(i);
                }
                if(dir == "Right" && this.IsToLeftOf(possible[i])){
                    possible.RemoveAt(i);
                }
                if(dir == "Down" && this.IsAbove(possible[i])){
                    possible.RemoveAt(i);
                }
                if(dir == "Up" && this.IsUnderneath(possible[i])){
                    possible.RemoveAt(i);
                }
            }
        }
        return possible;
    }

    /* RoomsToNotOfType() will return a list of rooms that a room
     * could connect to with respect to bounds checking, rooms it is
     * already connected to, and adjacent rooms of a certain type.
     * The types of rooms it excludes are determined by the types list
     * which contains Landmark rooms of a certain type.
     */
    public List<Room> RoomsToNotOfType(List<Landmark> types){
        List<Room> possible = this.NotAlreadyLinkedRooms();
        foreach(Landmark landmark in types){
            for(int i = 0; i < possible.Count; i++){
                if(possible[i].type.GetType() == landmark.GetType()){
                    possible.RemoveAt(i);
                    break;
                }
            }
        }
        return possible;
    }

    /* These functions are mainly helper functions that aid in
     * determining a rooms position within a 2D array as well as
     * finding other rooms in that array. There are a few functions
     * that perform bounds checking as well as a distance function
     * from this room to another or this room to another room's
     * coordinates.
     */

    // RoomToDirection() returns a direction based on a provided room
    public string RoomToDirection(Room room){
        // if this is above room, then the direction to room is down
        if(this.row - 1 == room.row && this.column == room.column){
            return "Up";
        }
        if(this.row + 1 == room.row && this.column == room.column){
            return "Down";
        }
        if(this.row == room.row && this.column - 1 == room.column){
            return "Left";
        }
        if(this.row == room.row && this.column + 1 == room.column){
            return "Right";
        }
        Debug.Log("RETURNING NONE ROOM");
        return "None";
    }

    /* ConnectTo will connect a room to another if the room
     * it is trying to connect to doesn't already have an exit
     * to the original room.
     */ 
    public void ConnectTo(Room room){
        string dir = this.RoomToDirection(room);
        if(!this.exits.Has(dir)){
            this.exits.Add(dir);
        }
        this.ConnectTo(dir);
    }

    /* A different variation of ConnectTo() above. It accepts a
     * string as an input.
     */
    public void ConnectTo(string dir){
        Room toConnect = null;
        if(dir == "Up"){
            toConnect = this.RoomAbove();
        }
        if(dir == "Down"){
            toConnect = this.RoomUnderneath();
        }
        if(dir == "Right"){
            toConnect = this.RoomToRight();
        }
        if(dir == "Left"){
            toConnect = this.RoomToLeft();
        }
        if(!toConnect.HasOpposite(dir)){
            toConnect.AddOpposite(dir);
        }
    }

    // HasOpposite() has determines if a room has an opposite direction
    public bool HasOpposite(string dir){
        string opposite = this.exits.OppositeDirection(dir);
        return this.exits.Has(opposite);
    }

    // AddOpposite() adds an opposing exit to itself
    public void AddOpposite(string dir){
        string opposite = this.exits.OppositeDirection(dir);
        this.exits.Add(opposite);
    }

    // DistanceTo() return the distance from this room to the argument passed, based on position in 2D array
    public float DistanceTo(int row, int col){
        return Mathf.Sqrt((Mathf.Pow((this.row - row), 2f)) + Mathf.Pow((this.column - col), 2f));
    }

    // DistanceTo() return the distance from this room to the argument passed, based on position in 2D array
    public float DistanceTo(Room room){
        return Mathf.Sqrt((Mathf.Pow((this.row - room.row), 2f)) + Mathf.Pow((this.column - room.column), 2f));
    }

    // Accessing rooms with respect to itself and bounds checking
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

    // Check if a room is in a particular corner of the warehouse
    public bool IsTopLeftCorner(){
        if(this.row == 0 && this.column == 0){
            return true;
        }
        return false;
    }

    public bool IsTopRightCorner(){
        if(this.row == 0 && this.column == warehouse.columns - 1){
            return true;
        }
        return false;
    }

    public bool IsBottomLeftCorner(){
        if(this.row == warehouse.rows - 1 && this.column == 0){
            return true;
        }
        return false;
    }

    public bool IsBottomRightCorner(){
        if(this.row == warehouse.rows - 1 && this.column == warehouse.columns - 1){
            return true;
        }
        return false;
    }

    // Positional checking to see a room is adjacent to another
    public bool IsToLeftOf(Room room){
        return this.row == room.row && this.column + 1 == room.column;
    }

    public bool IsToRightOf(Room room){
        return this.row == room.row && this.column - 1 == room.column;
    }

    public bool IsUnderneath(Room room){
        return this.row - 1 == room.row && this.column == room.column;
    }

    public bool IsAbove(Room room){
        return this.row + 1 == room.row && this.column == room.column;
    }

    // Bounds checking related
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

    // Check if current room is the start room
    public bool IsStartRoom(){
        return this.row == warehouse.startRow && this.column == warehouse.startCol;
    }

    // Random number between two values
    public int RandomNum(int lower, int upper){
        return UnityEngine.Random.Range(lower, upper);
    }

    public void Print(){
        Debug.Log(this.type.GetType() + " ROOM AT " + this.row + " " + this.column + " THIS ROOM HAS EXITS ");
        this.exits.Print();
    }
}