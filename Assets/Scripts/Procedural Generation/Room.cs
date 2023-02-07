using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class primarily used for holding data about what this.exits are in each room
public class Room
{
    public enum Direction
    {
        LEFT,
        RIGHT,
        UP,
        DOWN,
        NONE
    }

    public List<Direction> exits = new List<Direction>();
    public List<List<Room>> warehouseData = null;
    public Landmark type = null;
    public GameObject prefab;
    public bool exitsMade = false;
    public int row;
    public int column;

    public LoadAndPlacePrefab(){
        Debug.Log("LOADING AND PLACING PREFAB");
    }

    
    /* MakeExitsToConsider() creates a list of exits that the
     * current room could possibly have with respect to bounds
     * checking as well as the exits the room already has. It
     * accepts a Room object and return a list with elements
     * of type Direction.
     */
    private List<Direction> MakeExitsToConsider (int flag){
        // creat a list of possible exits
        List<Direction> exitsToConsider = new List<Direction>(new Direction[] {
                        Direction.LEFT, Direction.RIGHT, Direction.UP, Direction.DOWN
                        });
        // remove all exits from exitsToConsider that the room already has
        for(int i = 0; i < this.exits.Count; i++){
            exitsToConsider.RemoveAt(exitsToConsider.IndexOf(this.exits[i]));
        }
        // remove exits with respect to bounds checking
        if(this.row == 0){
            exitsToConsider.RemoveAt(exitsToConsider.IndexOf(Direction.UP));
        }
        if(this.row == rows - 1){
            exitsToConsider.RemoveAt(exitsToConsider.IndexOf(Direction.DOWN));
        }
        if(this.column == 0){
            exitsToConsider.RemoveAt(exitsToConsider.IndexOf(Direction.LEFT));
        }
        if(this.column == columns - 1){
            exitsToConsider.RemoveAt(exitsToConsider.IndexOf(Direction.RIGHT));
        }
        return exitsToConsider;
    }

    /* FindLeastExits() an adjacent room that has the least amount of exits.
     * It accepts a room as well as a list of Directions that inform
     * the function of what rooms to examine. It first assumes that none of the
     * adjacent rooms exist. The for loop iterates through the exits list and refutes
     * this by checking the amount of exits described by each Direction. It
     * iterates through the exits list, finds the room based on that exit with respect
     * to room, checks if that room has less exits than leastExits, if so change some
     * return values and update leastExits. It returns a tuple with the room adjacent 
     * to the current room that has the least amount of exits, the direction towards that
     * room, and the opposing direction.
     */
    private (Room leastExitsRoom, Direction exitToMake, Direction opposingExitToMake) FindLeastExits(Room room, List<Direction> exits){
        int leastExits = 4;
        Room leastExitsRoom = null;
        Direction exitToMake = Direction.NONE;
        Direction opposingExitToMake = Direction.NONE;
        for(int i = 0; i < exits.Count; i++){
            if(room.DirectionToRoom(exits[i]).roomExits.Count != 0 && 
                room.DirectionToRoom(exits[i]).roomExits.Count < leastExits){
                opposingExitToMake = FindOpposingDirection(exits[i]);
                exitToMake     = exits[i];
                leastExitsRoom = room.DirectionToRoom(exits[i]);
                leastExits     = room.DirectionToRoom(exits[i]).roomExits.Count;
            }
        }
        return (leastExitsRoom, exitToMake, opposingExitToMake);
    }


    // comments for rotating prefab
    private int RotatePrefab(){
        if(this.exits.Count == 2){
            Direction opposite = FindOpposingDirection(this.exits[0]);
            if(this.exits.Contains(opposite)){
                if(this.exits.Contains(Direction.LEFT) && this.exits.Contains(Direction.RIGHT)){
                    return 90;
                }
            }
            else{
                if(this.exits.Contains(Direction.DOWN) && this.exits.Contains(Direction.RIGHT)){
                    return 90;
                }
                if(this.exits.Contains(Direction.DOWN) && this.exits.Contains(Direction.LEFT)){
                    return 180;
                }
                if(this.exits.Contains(Direction.LEFT) && this.exits.Contains(Direction.UP)){
                    return -90;
                }
            }
        }
        else if(this.exits.Count == 3){
            if(this.exits.Contains(Direction.DOWN) && 
               this.exits.Contains(Direction.RIGHT) && 
               this.exits.Contains(Direction.UP)){
                return 180;
            }
            if(this.exits.Contains(Direction.DOWN) && 
               this.exits.Contains(Direction.RIGHT) && 
               this.exits.Contains(Direction.LEFT)){
                return -90;
            }
            if(this.exits.Contains(Direction.LEFT) && 
               this.exits.Contains(Direction.RIGHT) && 
               this.exits.Contains(Direction.UP)){
                return 90;
            }
        }
        return 0;
    }

    // DirectionToRoom() returns a Room object based on the direction passed
    // This does not account for bounds checking
    public Room DirectionToRoom (Direction dir){
        switch (dir){
            case Direction.UP:
                return warehouseData[this.row - 1][ this.column];
            case Direction.DOWN:
                return warehouseData[this.row + 1][ this.column];
            case Direction.LEFT:
                return warehouseData[this.row][ this.column - 1];
            case Direction.RIGHT:
                return warehouseData[this.row][ this.column + 1];
        }
        return null;
    }

    // RoomToDirection() returns a direction based on a provided room
    // This does not account for bounds checking
    public Direction RoomToDirection(Room room){
        if(this.row + 1 == this.row && this.column == this.column){
            return Direction.DOWN;
        }
        else if(this.row - 1 == this.row && this.column == this.column){
            return Direction.UP;
        }
        else if(this.row == this.row && this.column + 1 == this.column){
            return Direction.LEFT;
        }
        else if(this.row == this.row && this.column - 1 == this.column){
            return Direction.RIGHT;
        }
        else{
            return Direction.NONE;
        }
    }
}