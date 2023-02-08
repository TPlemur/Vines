using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class primarily used for holding data about what this.exits are in each room
public class Room
{
    private Warehouse warehouse;
    private GameObject warehouseEmpty;
    public  Exits exits = new Exits(false);
    public  Landmark type;
    public  bool exitsMade = false;
    public  int row;
    public  int column;

    public Room(int row, int column, Warehouse warehouse, GameObject warehouseEmpty){
        this.row = row;
        this.column = column;
        this.warehouse = warehouse;
        this.warehouseEmpty = warehouseEmpty;
        this.type = this.IsStartRoom() ? new Start(warehouseEmpty) : new Generic(warehouseEmpty);
    }

    ~Room(){
        exitsMade = false;
        warehouse = null;
        exits     = null;
        type      = null;
    }

    public void LoadAndPlace(){
        this.type.PlacePrefab(this.row, this.column, this.exits);
    }
    
    /* MakeExitsToConsider() creates a list of exits that the
     * current room could possibly have with respect to bounds
     * checking as well as the exits the room already has. It
     * accepts a Room object and return a list with elements
     * of type Direction.
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
        if(this.IsOnRightEdge()) { toConsider.Remove("Right");}
        if(this.IsOnLeftEdge())  { toConsider.Remove("Left"); }
        if(this.IsOnBottomEdge()){ toConsider.Remove("Down"); }
        if(this.IsOnTopEdge())   { toConsider.Remove("Up");   }
        return toConsider;
    }

    // Find Adjacent Landmarks
    public List<Room> FindAdjacentLandmarks(){
        List<Room> adjacents = new List<Room>();
        // add above room to list if its a landmark
        if(!this.IsOnTopEdge()){ 
            if(this.RoomAbove().type.GetType() != typeof(Generic)){
                adjacents.Add(this.RoomAbove());
            }
        }
        // add room below if its a landmark
        if(!this.IsOnBottomEdge()){ 
            if(this.RoomUnderneath().type.GetType() != typeof(Generic)){
                adjacents.Add(this.RoomUnderneath());
            }
        }
        // add room to the left if its a landmark
        if(!this.IsOnLeftEdge()){ 
            if(this.RoomToLeft().type.GetType() != typeof(Generic)){
                adjacents.Add(this.RoomToLeft());
            }
        }
        // add room to the right if its a landmark
        if(!this.IsOnRightEdge()){ 
            if(this.RoomToRight().type.GetType() != typeof(Generic)){
                adjacents.Add(this.RoomToRight());
            }
        }
        return adjacents;
    }

    // /* FindLeastExits() an adjacent room that has the least amount of exits.
    //  * It accepts a room as well as a list of Directions that inform
    //  * the function of what rooms to examine. It first assumes that none of the
    //  * adjacent rooms exist. The for loop iterates through the exits list and refutes
    //  * this by checking the amount of exits described by each Direction. It
    //  * iterates through the exits list, finds the room based on that exit with respect
    //  * to room, checks if that room has less exits than leastExits, if so change some
    //  * return values and update leastExits. It returns a tuple with the room adjacent 
    //  * to the current room that has the least amount of exits, the direction towards that
    //  * room, and the opposing direction.
    //  */
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

    // DirectionToRoom() returns a Room object based on the direction passed
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
        // if this is the room above room, then the direction to room is down
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

    // Functions used to get rooms based on direction
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
        Debug.Log("THIS ROOM IS AT ROW " + this.row + " COLUMN " + this.column);
    }
}