using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// class primarily used for holding data about what this.exits are in each room
public class Room
{

    private Warehouse warehouse;
    public List<Exit> exits = new List<Exit>();
    public Landmark type;
    public GameObject prefab;
    public bool exitsMade = false;
    public int row;
    public int column;

    public Room(int row, int column, Warehouse warehouse){
        this.row = row;
        this.column = column;
        this.warehouse = warehouse;
        this.type = new GenericRoom();
    }

    // public void LoadAndPlacePrefab(){
    //     Debug.Log("LOADING AND PLACING PREFAB");
    // }

    
    /* MakeExitsToConsider() creates a list of exits that the
     * current room could possibly have with respect to bounds
     * checking as well as the exits the room already has. It
     * accepts a Room object and return a list with elements
     * of type Direction.
     */
    public List<Exit> MakeExitsToConsider (bool flag){
        // creat a list of possible exits
        List<Exit> exitsToConsider = new List<Exit>(new Exit[] {
                        new Exit("Left"), new Exit("Right"), new Exit("Up"), new Exit("Down")
                        });
        // remove all exits from exitsToConsider that the room already has
        foreach(Exit direction in this.exits){
            Exit temp = exitsToConsider.Find(x => x.type == direction.type);
            exitsToConsider.RemoveAt(exitsToConsider.IndexOf(temp));
        }
        //////////////////////////////////////////////// for not adding exits existing landmark rooms
        // if(flag){
        //     List<Room> adjacentLandmarks = this.FindAdjacentLandmarks();
        //     foreach(Room adjacent in adjacentLandmarks){
        //         string dir = this.RoomToDirection(adjacent);
        //         Exit temp = exitsToConsider.Find(x => x.type == dir);
        //         exitsToConsider.RemoveAt(exitsToConsider.IndexOf(temp));
        //     }
        // }
        // List<Room> adjacentLandmarks = this.FindAdjacentLandmarks();
        // foreach(Room adjacent in adjacentLandmarks){
        //     string dir = this.RoomToDirection(adjacent);
        //     Exit temp = exitsToConsider.Find(x => x.type == dir);
        //     exitsToConsider.RemoveAt(exitsToConsider.IndexOf(temp));
        // }
        ////////////////////////////////////////////////
        // remove exits with respect to bounds checking
        if(this.row == 0){
            var upExit = exitsToConsider.Find(x => x.type == "Up");
            if(upExit != null){
                exitsToConsider.RemoveAt(exitsToConsider.IndexOf(upExit));
                Debug.Log("REMOVING UP EXIT");
            }
            // exitsToConsider.RemoveAt(exitsToConsider.IndexOf(upExit));
        }
        if(this.row == warehouse.rows - 1){
            // exitsToConsider.RemoveAt(exitsToConsider.IndexOf(Direction.DOWN));
            var downExit = exitsToConsider.Find(x => x.type == "Down");
            if(downExit != null){
                exitsToConsider.RemoveAt(exitsToConsider.IndexOf(downExit));
                Debug.Log("REMOVING DOWN EXIT");
            }
            
        }
        if(this.column == 0){
            var leftExit = exitsToConsider.Find(x => x.type == "Left");
            if(leftExit != null){
                exitsToConsider.RemoveAt(exitsToConsider.IndexOf(leftExit));
                Debug.Log("REMOVING LEFT EXIT");
            }
            
        }
        if(this.column == warehouse.columns - 1){
            var rightExit = exitsToConsider.Find(x => x.type == "Right");
            if(rightExit != null){
                exitsToConsider.RemoveAt(exitsToConsider.IndexOf(rightExit));
                Debug.Log("REMOVING RIGHT EXIT");
            }
            
        }
        Debug.Log("FINAL EXITS TO CONSIDER " + exitsToConsider.Count);
        return exitsToConsider;
    }

    // Find Adjacent Landmarks
    private List<Room> FindAdjacentLandmarks(){
        List<Room> adjacents = new List<Room>();
        if(this.row != 0){ // add above room to list if its a landmark
            if(warehouse.room2DArray[this.row - 1][this.column].type.GetType() != typeof(GenericRoom)){
                adjacents.Add(warehouse.room2DArray[this.row - 1][this.column]);
            }
        }
        if(this.row != warehouse.rows - 1){ // add room below if its a landmark
            if(warehouse.room2DArray[this.row + 1][this.column].type.GetType() != typeof(GenericRoom)){
                adjacents.Add(warehouse.room2DArray[this.row + 1][this.column]);
            }
        }
        if(this.column != 0){ // add room to the left if its a landmark
            if(warehouse.room2DArray[this.row][this.column - 1].type.GetType() != typeof(GenericRoom)){
                adjacents.Add(warehouse.room2DArray[this.row][this.column - 1]);
            }
        }
        if(this.column != warehouse.columns - 1){ // add room to the right if its a landmark
            if(warehouse.room2DArray[this.row][this.column + 1].type.GetType() != typeof(GenericRoom)){
                adjacents.Add(warehouse.room2DArray[this.row][this.column + 1]);
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
    // private (Room leastExitsRoom, Direction exitToMake, Direction opposingExitToMake) FindLeastExits(Room room, List<Direction> exits){
    //     int leastExits = 4;
    //     Room leastExitsRoom = null;
    //     Direction exitToMake = Direction.NONE;
    //     Direction opposingExitToMake = Direction.NONE;
    //     for(int i = 0; i < exits.Count; i++){
    //         if(room.DirectionToRoom(exits[i]).roomExits.Count != 0 && 
    //             room.DirectionToRoom(exits[i]).roomExits.Count < leastExits){
    //             opposingExitToMake = FindOpposingDirection(exits[i]);
    //             exitToMake     = exits[i];
    //             leastExitsRoom = room.DirectionToRoom(exits[i]);
    //             leastExits     = room.DirectionToRoom(exits[i]).roomExits.Count;
    //         }
    //     }
    //     return (leastExitsRoom, exitToMake, opposingExitToMake);
    // }

    // DirectionToRoom() returns a Room object based on the direction passed
    // This does not account for bounds checking
    public Room DirectionToRoom (Exit dir){
        switch (dir.type){
            case "Up":
                if(this.row != 0){
                    return warehouse.room2DArray[this.row - 1][ this.column];
                }
                break;
            case "Down":
                if(this.row != warehouse.rows - 1){
                    return warehouse.room2DArray[this.row + 1][ this.column];
                }
                break;
            case "Left":
                if(this.column != 0){
                    return warehouse.room2DArray[this.row][this.column - 1];
                }
                break;
            case "Right":
                if(this.column != warehouse.columns - 1){
                    return warehouse.room2DArray[this.row][this.column + 1];
                }
                break;
        }
        return null;
    }

    // RoomToDirection() returns a direction based on a provided room
    // This does not account for bounds checking
    public string RoomToDirection(Room room){
        if(this.row + 1 == room.row && this.column == room.column){
            return "Down";
        }
        else if(this.row - 1 == room.row && this.column == room.column){
            return "Up";
        }
        else if(this.row == room.row && this.column + 1 == room.column){
            return "Right";
        }
        else if(this.row == room.row && this.column - 1 == room.column){
            return "Right";
        }
        else{
            return "None";
        }
    }
}