using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Warehouse{
    public int rows;
    public int columns;
    public int startRow;
    public int startCol;
    public List<List<Room>> data;
    public Room startRoom;
    private GameObject warehouseEmpty;

    public Warehouse(int row, int columns, int startRow, int startCol, GameObject empty){
        this.rows = row;
        this.columns = columns;
        this.startRow = startRow;
        this.startCol = startCol;
        this.warehouseEmpty = empty;
    }

    ~Warehouse(){
        this.Teardown();
    }

    public void Generate(){
        // Add empty "rooms" to the 2D Array based on the provided dimensions
        this.data = new List<List<Room>>();
        for(int i = 0; i < this.rows; i++){ 
            List<Room> rowOfEmptyRooms = new List<Room>();
            for(int j = 0; j < this.columns; j++){
                Room emptyRoom = new Room(i, j, this, warehouseEmpty);
                rowOfEmptyRooms.Add(emptyRoom);
            }
            data.Add(rowOfEmptyRooms);
        }
        Debug.Log("START ROOM");
        GenerateExitsFor(data[this.startRow][this.startCol]);
        FixDeadEnds();
        PlaceRooms();
    }

    public void Regenerate(){
        this.Teardown();
        this.Generate();
    }

    private void Teardown(){
        foreach(List<Room> row in data){
            for(int i = 0; i < row.Count; i++){
                row[i].exits = null;
                row[i] = null;
            }
        }
        this.data = null;
    }

    /* GenerateExitsFor() is the driving algorithm for procedurally filling a 2D array with
     * "rooms". The algorithm will first generate a random number that decides how many 
     * potential exits the current room will have. It then chooses a random index from the 
     * array possible. If the current room doesn't have an exit at the value of that
     * index, it creates that exit then creates another opposing exit at the room adjacent. 
     * So if we make a left exit at the current room we make a right exit at the room to the 
     * left of the current one. The algorithm will then looks at all exits the current room 
     * has and then pick another random number that decides if the next room will have more 
     * exits which is handled by RollForMoreExits().
     */
    private void GenerateExitsFor(Room current){
        Exits possible = current.MakeExitsToConsider(true);
        // if distance of the current room to the start room is less or equal to 1 then we up the RNG of choosing more exits
        int potential = 0;
        if(current.IsStartRoom()){
            potential = 3;
        }
        else{
            potential = current.DistanceTo(this.startRow, this.startCol) <= 1 ? possible.NumberOf() : current.RandomNum(1, possible.NumberOf());
        }
        for(; potential > 0; potential--){
            string randomExit = possible.Random();
            switch(randomExit){
                case "Left":
                    current.exits.Add("Left");
                    current.RoomToLeft().exits.Add("Right");
                    break;
                case "Right":
                    current.exits.Add("Right");
                    current.RoomToRight().exits.Add("Left");
                    break;
                case "Up":
                    current.exits.Add("Up");
                    current.RoomAbove().exits.Add("Down");
                    break;
                case "Down":
                    current.exits.Add("Down");
                    current.RoomUnderneath().exits.Add("Up");
                    break;
            }
            // then remove that random direction so it doesn't get chosen again
            possible.Remove(randomExit);
        }
        current.exitsMade = true;
        if(current.exits.Has("Left") && !current.RoomToLeft().exitsMade){
            RollForMoreExits(current.RoomToLeft());
        }
        if(current.exits.Has("Right") && !current.RoomToRight().exitsMade){
            RollForMoreExits(current.RoomToRight());
        }
        if(current.exits.Has("Up") && !current.RoomAbove().exitsMade){
            RollForMoreExits(current.RoomAbove());
        }
        if(current.exits.Has("Down") && !current.RoomUnderneath().exitsMade){
            RollForMoreExits(current.RoomUnderneath());
        }
        return;
    }

    /* RollForMoreExits() basically rolls a random number for the current room and
     * has a 1 in 15 chance to not create adittional exits for that room. If we
     * don't hit that 1 in 15 we generate more exits for the room at that index.
     */
    private void RollForMoreExits(Room room){
        if(UnityEngine.Random.Range(0, 14) == 0){
            room.exitsMade = true;
            return;
        }
        GenerateExitsFor(room);
    }

    // /* PlaceRooms() will iterate through the 2D array of Room and
    //  * will instantiate rooms based the current rooms exits and places them
    //  * appropriately. It first build a string based off of its exits which is
    //  * then used to load a prefab from the folder "Assets/Resources/ProcgenGreyboxes".
    //  * It places an empty game object at the correct location within world space. This
    //  * was done to ensure that the original rotation of the prefab doesn't get altered
    //  * due to the Instantiate() method. EmptyGameObject's parent is set to the Warehouse
    //  * GameObject for organization within the editor. It then loads the prefab and
    //  * sets its position to the previously mentioned EmptyGameObject.
    //  */
    private void PlaceRooms(){
        foreach(List<Room> row in data){
            foreach(Room room in row){
                if(room.exits.NumberOf() == 0){
                    continue;
                }
                room.LoadAndPlace();
            }
        }
    }

    // /* FixDeadEnds() iterates through the warehouseData array and finds
    //  * dead ends. It calls MakeExitsToConsider() which creates a List of
    //  * surrounding exits besides the exit the current room already has.
    //  * It then calls FindLeastExits() which finds the closest adjacent
    //  * room with the least amount of exits. This was done to up the exits
    //  * a room has rather than favoring rooms that already have many exits.
    //  * If a room exits then we connect that room with the current room, 
    //  * fixing the dead end. If no adjacent rooms exist (excluding the one
    //  * this room connects to) then we chose one of those nonexistent, fix
    //  * the dead end, then we move to the new dead end we created and then
    //  * attempt to fix it again.
    //  */
    private void FixDeadEnds(){
        for(int i = 0; i < this.rows; i++){
            for(int j = 0; j < this.columns; j++){
                Room deadEnd = data[i][j];
                while(deadEnd.exits.NumberOf() == 1){
                    Exits toConsider = deadEnd.MakeExitsToConsider(false);
                    (Room leastExits, string exit, string opposing) = deadEnd.FindLeastExits();
                    if(leastExits != null){
                        deadEnd.exits.Add(exit);
                        leastExits.exits.Add(opposing);
                        break;
                    }
                    else{
                        string randomExit = toConsider.Random();
                        // fix the dead end with the adjacent room
                        deadEnd.exits.Add(randomExit);
                        Room newDeadEnd = deadEnd.DirectionToRoom(randomExit);
                        newDeadEnd.exits.Add(toConsider.OppositeDirection(randomExit));
                        // set current dead end room to the new dead room we just made
                        deadEnd = newDeadEnd;
                    }
                }
            }
        }
    }

    /* PrintWarehouse() logs the exits of each index to the console
     * "L" "U" "D" "R" signify the direction of the exit at each index
     */
    private void PrintWarehouse(){
        Debug.Log("PRINTING WAREHOUSE ROOM DATA");
        foreach(List<Room> row in data){
            string rowData = System.String.Empty;
            foreach(Room room in row){
                rowData += room.exits.Has("Left")  ? "L" : "_";
                rowData += room.exits.Has("Up")    ? "U" : "_";
                rowData += room.exits.Has("Down")  ? "D" : "_";
                rowData += room.exits.Has("Right") ? "R" : "_";
                rowData += " ";
            }
            rowData += "\n";
            Debug.Log(rowData);
        }
    }
}