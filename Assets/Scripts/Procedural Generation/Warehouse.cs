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
    private List<Landmark> special  = new List<Landmark>(new Landmark[] {new AlphaTeam(),  new Generator(), new ShieldRoom()});
    private List<Landmark> cameras  = new List<Landmark>(new Landmark[] {new PVTMCamera(), new PVTMCamera(), new PVTMCamera(), new PVTMCamera()});
    private List<Landmark> hiding   = new List<Landmark>(new Landmark[] {new Hide(), new Hide(), new Hide(), new Hide()});
    private List<Landmark> tripwire = new List<Landmark>(new Landmark[] {new TripWire(), new TripWire(), new TripWire(), new TripWire()});

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
        this.startRoom.ConnectToRandom();
        PlaceMonsterSpawn();
        PlaceLandmarks();
        FixDeadEnds();
        PlaceRooms();
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

    /* FixDeadEnds() iterates through data to find and fix dead ends.
     * It first determines if a room is a dead end. If so then it finds
     * an adjacent room with the least amount of exits. If a room exists
     * then it will connect the dead end room to that room. If not then
     * it examines all adjacent rooms, chooses one at random, connects the
     * dead end room to the room chosen, and then sets deadEnd to the
     * room randomly chosen.
     */
    private void FixDeadEnds(){
        for(int i = 0; i < this.rows; i++){
            for(int j = 0; j < this.columns; j++){
                Room deadEnd = data[i][j];
                while(deadEnd.exits.NumberOf() == 1){
                    int index = -1;
                    int leastNum = 4;
                    List<Landmark> temp = new List<Landmark>(new Landmark[] {new Monster("1", 0), new Start()});
                    List<Room> possible = deadEnd.RoomsToNotOfType(temp);
                    for(int k = 0; k < possible.Count; k++){
                        if(possible[k].exits.NumberOf() != 0 && possible[k].exits.NumberOf() < leastNum){
                            leastNum = possible[k].exits.NumberOf();
                            index = k;
                        }
                    }
                    // some room with the least amount of exits was found
                    if(index != -1){
                        deadEnd.ConnectTo(possible[index]);
                    }
                    else{
                        // no room was found and this dead end has no adjacent room so make a
                        // new one and then check if that room has adjacent rooms to connect to
                        Room newDeadEnd = possible[UnityEngine.Random.Range(0, possible.Count)];
                        deadEnd.ConnectTo(newDeadEnd);
                        deadEnd = newDeadEnd;
                    }
                }
            }
        }
    }

    /* PlaceRooms() will iterate through the 2D array of Rooms to
     * instantiate and place prefabs based on each rooms exits.
     */
    private void PlaceRooms(){
        foreach(List<Room> row in data){
            foreach(Room room in row){
                if(room.exits.NumberOf() == 0){
                    continue;
                }
                // get empty parent object with nested prefab and 
                // set its parent to the warehouse object
                GameObject prefabParent = room.LoadAndPlace();
                prefabParent.transform.SetParent(warehouseEmpty.transform);
            }
        }
    }

    /* PlaceMonsterSpawn() will first choose a random corner of the 2D
     * array for the location of the spawn room. It then determines what
     * corner it is and then determines the appropriate quadrants
     * for adjacent rooms and one diagonal room. It also calculates a
     * rotation to apply to each quadrant which is used when placing each prefab.
     */
    private void PlaceMonsterSpawn(){
        Room corner = GetRandomCorner();
        Room q2 = null;
        Room q3 = null;
        Room q4 = null;
        Room opposite = null;
        int rotation = 0;
        if(corner.IsTopLeftCorner()){
            q2 = corner.RoomToRight();
            q3 = q2.RoomUnderneath();
            q4 = corner.RoomUnderneath();
            // get room opposite of q4 aka room leading into the spawn room
            opposite = q4.RoomUnderneath();
        }
        else if(corner.IsTopRightCorner()){
            q2 = corner.RoomUnderneath();
            q3 = q2.RoomToLeft();
            q4 = corner.RoomToLeft();
            rotation = 90;
            opposite = q4.RoomToLeft();
        }
        else if(corner.IsBottomLeftCorner()){
            q2 = corner.RoomAbove();
            q3 = q2.RoomToRight();
            q4 = corner.RoomToRight();
            rotation = -90;
            opposite = q4.RoomToRight();
        }
        else if(corner.IsBottomRightCorner()){
            q2 = corner.RoomToLeft();
            q3 = q2.RoomAbove();
            q4 = corner.RoomAbove();
            rotation = 180;
            opposite = q4.RoomAbove();
        }
        // set new rooms to respective quadrant types
        corner.type = new Monster("1", rotation);
        q2.type = new Monster("2", rotation);
        q3.type = new Monster("3", rotation);
        q4.type = new Monster("4", rotation);
        corner.exitsMade = true;
        q2.exitsMade = true;
        q3.exitsMade = true;
        q4.exitsMade = true;
        // connect q4 to opposite and create a dead end in opposite
        q4.ConnectTo(opposite);
        corner.ConnectTo(q2);
        corner.ConnectTo(q4);
        q3.ConnectTo(q2);
        q3.ConnectTo(q4);
        this.monsterRoom = q4;
    }

    // Place All Landmark rooms
    private void PlaceLandmarks(){
        this.PlaceLandmarksFrom(this.special);
        this.PlaceLandmarksFrom(this.cameras);
        this.PlaceLandmarksFrom(this.hiding);
        this.PlaceLandmarksFrom(this.tripwire);
    }

    /* PlaceLandmarksFrom() will place landmark rooms found in a list.
     * It first check if the list has any rooms to place. It then gets
     * a random Generic room and determines if this room is within the
     * Landmarks minimum and maximum spawn distance form the start room.
     * If so then we place the room at that location and choose random
     * exits for that room and then remove the Landmark from the list of
     * Landmarks to place.
     */
    private void PlaceLandmarksFrom(List<Landmark> list){
        while(list.Count != 0){
            Room random = GetRandomGeneric();
            foreach(Landmark landmark in list){
                float dist = random.DistanceTo(this.startRoom);
                if(landmark.InRange(dist)){
                    random.type = landmark;
                    random.ConnectToRandom();
                    list.RemoveAt(list.IndexOf(landmark));
                    break;
                }
            }
        }
    }

    /* GetRandomCorner() will choose a random corner of the
     * 2D array and return the room at that position
     */
    private Room GetRandomCorner(){
        int row = UnityEngine.Random.Range(0, 2) == 0 ? 0 : this.rows - 1;
        int column = UnityEngine.Random.Range(0, 2) == 0 ? 0 : this.columns - 1;
        return data[row][column];
    }

    /* GetRandomGeneric() will choose a random genric room
     */
    private Room GetRandomGeneric(){
        while(true){
            int row = UnityEngine.Random.Range(0, this.rows - 1);
            int column = UnityEngine.Random.Range(0, this.columns -1);
            if(data[row][column].type.GetType() == typeof(Generic)){
                return data[row][column];
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

    /* TurnOnLights() turn on lights
     *
     */
    public void TurnOnLights(){
        foreach(List<Room> row in data){
            foreach(Room room in row){
                if(room.exits.NumberOf() != 0){
                    Transform lights = room.obj.transform.GetChild(0).transform.Find("Lights");
                    if(lights != null){
                        lights.gameObject.SetActive(true);
                    }
                    Transform emergency_lights = room.obj.transform.GetChild(0).transform.Find("EmergencyLights");
                    if(emergency_lights != null){
                        emergency_lights.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    /* PrintWarehouse() logs the exits of each index to the console
     * "L" "U" "D" "R" signify the direction of the exit at each index
     */
    public void PrintWarehouse(){
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