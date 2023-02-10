using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/* Warehouse class contains information for a 2D array filled
 * with "Rooms", how many rows and columns the 2D array has, its
 * starting room, the monster room, as well as the warehouse GameObject.
 * It contains functionality for procedurally generating exits for the
 * rooms inside the 2D array based off of a starting position. It will
 * also patch any dead ends within itself and will place prefabs and 
 * Landmark rooms appropriately.
 */
public class Warehouse{

    private GameObject warehouseEmpty;
    public List<List<Room>> data;
    public Room startRoom;
    public Room monsterRoom;
    public int startRow;
    public int startCol;
    public int columns;
    public int rows;

    // List containing landmark rooms to make, add appropriate landmark subclass to list
    private List<Landmark> toMake = new List<Landmark>(new Landmark[] {new Monster(), new PVTMCamera(), new Generator(), new AlphaTeam()});
    
    public Warehouse(int row, int columns, int startRow, int startCol, GameObject empty){
        this.warehouseEmpty = empty;
        this.startRow = startRow;
        this.startCol = startCol;
        this.columns  = columns;
        this.rows     = row;
    }

    ~Warehouse(){
        this.Teardown();
    }

    /* Generate() will fill data with empty "Rooms" and then call
     * GenerateExitsFor() to procedurally fill rooms with exits, fix
     * any dead ends it may have, and then place prefabs to build the warehouse.
     */
    public void Generate(){
        // Add empty "rooms" to the 2D Array based on the provided dimensions
        this.data = new List<List<Room>>();
        for(int i = 0; i < this.rows; i++){ 
            List<Room> rowOfEmptyRooms = new List<Room>();
            for(int j = 0; j < this.columns; j++){
                Room emptyRoom = new Room(i, j, this);
                rowOfEmptyRooms.Add(emptyRoom);
            }
            data.Add(rowOfEmptyRooms);
        }
        this.startRoom = data[this.startRow][this.startCol];
        GenerateExitsFor(data[this.startRow][this.startCol]);
        FixDeadEnds();
        PlaceRooms();
        if(toMake.Count != 0){
            Debug.Log("DIDNT FIND A SPOT FOR " + toMake.Count + " ROOMS");
        }
    }

    /* Regenerate() will teardown the warehouse and then generate it
     */
    public void Regenerate(){
        this.Teardown();
        this.Generate();
    }

    /* Teardown() will get rid of all exit and
     * room data inside the warehouse.
     */
    private void Teardown(){
        foreach(List<Room> row in data){
            for(int i = 0; i < row.Count; i++){
                row[i].exits = null;
                row[i] = null;
            }
        }
        this.data = null;
    }

    /* GenerateExitsFor() is the driving algorithm for procedurally 
     * filling a 2D array with "rooms". The algorithm will first determine
     * the possible exits this room could have. It then decides the amount of
     * potential exits the room could have. It then selects a random exit from
     * the list of possible ones, creates that exit in the current room, creates
     * the opposing exit in the opposing room. It will then checks its own exits
     * and if other rooms exits have been made and then calls RollForMoreExits()
     * to see if the other rooms will generate more exits.
     */
    private void GenerateExitsFor(Room current){
        Exits possible = current.MakeExitsToConsider(true);
        int potential = 0;
        if(current.IsStartRoom()){
            potential = 3;
        }
        else{
            // if distance of the current room to the start room is less or equal to 1 then we up the RNG of choosing more exits
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
        if(UnityEngine.Random.Range(0, 16) == 0){
            room.exitsMade = true;
            return;
        }
        GenerateExitsFor(room);
    }

    /* FixDeadEnds() iterates through data to find and fix dead ends.
     * It first determines if a room is a dead end. If so then it finds
     * an adjacent room with the least amount of exits. If a room exists
     * then it will connect the dead end room to that room. If not then
     * it examines all adjacent rooms, chooses one at random, connects the
     * dead end room to the room chosen, and then sets deadEnd as to the
     * room randomly chosen.
     */
    private void FixDeadEnds(){
        for(int i = 0; i < this.rows; i++){
            for(int j = 0; j < this.columns; j++){
                Room deadEnd = data[i][j];
                while(deadEnd.exits.NumberOf() == 1){
                    // get adjacent room with the least amount of exits
                    (Room leastExits, string exit, string opposing) = deadEnd.FindLeastExits();
                    if(leastExits != null){
                        deadEnd.exits.Add(exit);
                        leastExits.exits.Add(opposing);
                        break;
                    }
                    else{
                        Exits toConsider = deadEnd.MakeExitsToConsider(false);
                        string randomExit = toConsider.Random();
                        // make exit to a random adjacent room
                        // this room may have no exits at all or some
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

    /* PlaceRooms() will iterate through the 2D array of Rooms to
     * instantiate and place prefabs based on each rooms exits. It
     * will also loop through the list of Landmarks to make. It 
     * calculates the rooms distance to the start room, the calls
     * landmark.InRange() and landmark.PlaceLandmark() to determine
     * if whether or not we change the current rooms type to be a 
     * Landmark.
     */
    private void PlaceRooms(){
        foreach(List<Room> row in data){
            foreach(Room room in row){
                if(room.exits.NumberOf() == 0){
                    continue;
                }
                // iterate through list of landmarks to make
                foreach(Landmark landmark in toMake){
                    float dist = room.DistanceTo(data[this.startRow][this.startCol]);
                    if(landmark.InRange(dist)){
                        if(landmark.PlaceLandmark()){
                            if(landmark.GetType() == typeof(Monster)){
                                this.monsterRoom = room;
                            }
                            room.type = landmark;
                            room.Print();
                            toMake.RemoveAt(toMake.IndexOf(landmark));
                            break;
                        }
                    }
                }
                // get empty parent object with nested prefab and 
                // set its parent to the warehouse object
                GameObject prefabParent = room.LoadAndPlace();
                prefabParent.transform.SetParent(warehouseEmpty.transform);
            }
        }
    }

    /* PlacePlayerAndMonster() will place the player, camera, 
     * and monster GameObjects at their correct spawn location.
     */
    public void PlacePlayerAndMosnter(GameObject player, GameObject cam, GameObject monster){
        player.transform.position  = new Vector3(this.startRoom.obj.transform.position.x, 2f, this.startRoom.obj.transform.position.z);
        cam.transform.position     = new Vector3(this.startRoom.obj.transform.position.x, 2f, this.startRoom.obj.transform.position.z);
        monster.transform.position = this.monsterRoom.obj.transform.position;
    }



    /* PrintWarehouse() logs the exits of each index to the console
     * "L" "U" "D" "R" signify the direction of the exit at each index
     */
    private void PrintWarehouse(){
        Debug.Log("PRINTING WAREHOUSE DATA");
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