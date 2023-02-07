using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Warehouse{
    public int rows;
    public int columns;
    public int startRow;
    public int startCol;
    //public List<List<Room>
    public Warehouse(){
        Debug.Log("MAKING WAREHOUSE");
    }

    /* Generateexits() is the driving algorithm for procedurally filling a 2D array with
     * "rooms". The algorithm will first generate a random number that decides how many 
     * potential exits the current room will have. It then chooses a random index from the 
     * array potentialExits. If the current room doesn't have an exit at the value of that
     * index, it creates that exit then creates another opposing exit at the room adjacent. 
     * So if we make a left exit at the current room we make a right exit at the room to the 
     * left of the current one. The algorithm will then looks at all exits the current room 
     * has and then pick another random number that decides if the next room will have more 
     * exits which is handled by RollForMoreExits().
     */
    void GenerateRoomExits(Room currentRoom){
        if(currentRoom.exitsMade){
            return;
        }
        // create list with possible exits the room could have excluding the exit the room already has if it has any
        List<Direction> possibleExits = MakeExitsToConsider(currentRoom);
        // if distance of the current room to the start room is less or equal to 1 then we up the RNG of choosing more exits
        int potentialExits = 0;
        if(currentRoom.row == startRoomRow && currentRoom.column == startRoomColumn){
            potentialExits = 4;
        }
        else{
            float distToStart = Mathf.Sqrt((Mathf.Pow((currentRoom.row - startRoomRow), 2f)) + Mathf.Pow((currentRoom.column - startRoomColumn), 2f));
            potentialExits = distToStart <= 1 ? UnityEngine.Random.Range(possibleExits.Count - 1, possibleExits.Count) : UnityEngine.Random.Range(1, possibleExits.Count - 1);
        }
        for(; potentialExits > 0; potentialExits--){
            Direction randomExit = RandomExitFromList(possibleExits);
            Room room = null;
            switch(randomExit){
                case Direction.LEFT:
                    currentRoom.exits.Add(Direction.LEFT);
                    room = currentRoom.DirectionToRoom(Direction.LEFT);
                    room.exits.Add(Direction.RIGHT);
                    break;
                case Direction.RIGHT:
                    currentRoom.exits.Add(Direction.RIGHT);
                    room = currentRoom.DirectionToRoom(Direction.RIGHT);
                    room.exits.Add(Direction.LEFT);
                    break;
                case Direction.UP:
                    currentRoom.exits.Add(Direction.UP);
                    room = currentRoom.DirectionToRoom(Direction.UP);
                    room.exits.Add(Direction.DOWN);
                    break;
                case Direction.DOWN:
                    currentRoom.exits.Add(Direction.DOWN);
                    room = currentRoom.DirectionToRoom(Direction.DOWN);
                    room.exits.Add(Direction.UP);
                    break;
            }
            // then remove that random direction so it doesn't get chosen again
            possibleExits.RemoveAt(possibleExits.IndexOf(randomExit));
        }
        currentRoom.exitsMade = true;
        if(currentRoom.exits.Contains(Direction.LEFT)
           && !currentRoom.DirectionToRoom(Direction.LEFT).exitsMade){
            RollForMoreExits(currentRoom.DirectionToRoom(Direction.LEFT));
        }
        if(currentRoom.exits.Contains(Direction.RIGHT)
           && !currentRoom.DirectionToRoom(Direction.RIGHT).exitsMade){
            RollForMoreExits(currentRoom.DirectionToRoom(Direction.RIGHT));
        }
        if(currentRoom.exits.Contains(Direction.UP)
           && !currentRoom.DirectionToRoom(Direction.UP).exitsMade){
            RollForMoreExits(currentRoom.DirectionToRoom(Direction.UP));
        }
        if(currentRoom.exits.Contains(Direction.DOWN)
           && !currentRoom.DirectionToRoom(Direction.DOWN).exitsMade){
            RollForMoreExits(currentRoom.DirectionToRoom(Direction.DOWN));
        }
        return;
    }

    /* RollForMoreExits() basically rolls a random number for the current room and
     * has a 1 in 15 chance to not create adittional exits for that room. If we
     * don't hit that 1 in 15 we generate more exits for the room at that index.
     */
    void RollForMoreExits(Room room){
        if(UnityEngine.Random.Range(0, 14) == 0){
            room.exitsMade = true;
            return;
        }
        GenerateRoomExits(room);
    }
}