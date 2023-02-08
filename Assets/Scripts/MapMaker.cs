using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum exitDirection
{
    LEFT,
    RIGHT,
    UP,
    DOWN,
    NONE
}

public class MapMaker : MonoBehaviour
{

    [SerializeField] private int rows;
    [SerializeField] private int columns;
    [SerializeField] private int startRow;
    [SerializeField] private int startCol;
    [SerializeField] private GameObject Warehouse;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject monster;

    public List<List<Room>> warehouseData = new List<List<Room>>();
    public Warehouse warehouse;

    public NavMeshSurface surface;

    void Start()
    {
        warehouse = new Warehouse(this.rows, this.columns, this.startRow, this.startCol);
        warehouse.Generate();
        // // error checking to make sure that the provided starting row and column
        // // is a valid index in the provided dimensions of the 2D arrray
        // if(startRoomRow >= rows || startRoomColumn >= columns 
        //     || startRoomRow < 0 || startRoomColumn < 0){
        //     startRoomRow = UnityEngine.Random.Range(0, rows - 1);
        //     startRoomColumn = UnityEngine.Random.Range(0, columns - 1);
        // }
        // for(int i = 0; i < rows; i++){ // Add empty "rooms" to the 2D Array based on the provided dimensions
        //     List<Room> rowOfEmptyRooms = new List<Room>();
        //     for(int j = 0; j < columns; j++){
        //         Room emptyRoom = new Room();
        //         emptyRoom.row = i;
        //         emptyRoom.column = j;
        //         emptyRoom.warehouseData =  warehouseData;
        //         rowOfEmptyRooms.Add(emptyRoom);
        //     }
        //     warehouseData.Add(rowOfEmptyRooms);
        // }
        // warehouseData[startRoomRow][startRoomColumn].type = "STARTING ROOM";
        // GenerateRoomExits(warehouseData[startRoomRow][startRoomColumn]);
        // FixDeadEnds();
        // PlaceRooms();

        // surface.BuildNavMesh();
    }

    // /* GenerateRoomExits() is the driving algorithm for procedurally filling a 2D array with
    //  * "rooms". The algorithm will first generate a random number that decides how many 
    //  * potential exits the current room will have. It then chooses a random index from the 
    //  * array potentialExits. If the current room doesn't have an exit at the value of that
    //  * index, it creates that exit then creates another opposing exit at the room adjacent. 
    //  * So if we make a left exit at the current room we make a right exit at the room to the 
    //  * left of the current one. The algorithm will then looks at all exits the current room 
    //  * has and then pick another random number that decides if the next room will have more 
    //  * exits which is handled by RollForMoreExits().
    //  */
    // void GenerateRoomExits(Room currentRoom){
    //     if(currentRoom.exitsMade){
    //         return;
    //     }
    //     // create list with possible exits the room could have excluding the exit the room already has if it has any
    //     List<exitDirection> possibleExits = MakeExitsToConsider(currentRoom);
    //     // if distance of the current room to the start room is less or equal to 1 then we up the RNG of choosing more exits
    //     int potentialExits = 0;
    //     if(currentRoom.row == startRoomRow && currentRoom.column == startRoomColumn){
    //         potentialExits = 4;
    //     }
    //     else{
    //         float distToStart = Mathf.Sqrt((Mathf.Pow((currentRoom.row - startRoomRow), 2f)) + Mathf.Pow((currentRoom.column - startRoomColumn), 2f));
    //         potentialExits = distToStart <= 1 ? UnityEngine.Random.Range(possibleExits.Count - 1, possibleExits.Count) : UnityEngine.Random.Range(1, possibleExits.Count - 1);
    //     }
    //     for(; potentialExits > 0; potentialExits--){
    //         exitDirection randomExit = RandomExitFromList(possibleExits);
    //         Room room = null;
    //         switch(randomExit){
    //             case exitDirection.LEFT:
    //                 currentRoom.roomExits.Add(exitDirection.LEFT);
    //                 room = currentRoom.DirectionToRoom(exitDirection.LEFT);
    //                 room.roomExits.Add(exitDirection.RIGHT);
    //                 break;
    //             case exitDirection.RIGHT:
    //                 currentRoom.roomExits.Add(exitDirection.RIGHT);
    //                 room = currentRoom.DirectionToRoom(exitDirection.RIGHT);
    //                 room.roomExits.Add(exitDirection.LEFT);
    //                 break;
    //             case exitDirection.UP:
    //                 currentRoom.roomExits.Add(exitDirection.UP);
    //                 room = currentRoom.DirectionToRoom(exitDirection.UP);
    //                 room.roomExits.Add(exitDirection.DOWN);
    //                 break;
    //             case exitDirection.DOWN:
    //                 currentRoom.roomExits.Add(exitDirection.DOWN);
    //                 room = currentRoom.DirectionToRoom(exitDirection.DOWN);
    //                 room.roomExits.Add(exitDirection.UP);
    //                 break;
    //         }
    //         // then remove that random direction so it doesn't get chosen again
    //         possibleExits.RemoveAt(possibleExits.IndexOf(randomExit));
    //     }
    //     currentRoom.exitsMade = true;
    //     if(currentRoom.roomExits.Contains(exitDirection.LEFT)
    //        && !currentRoom.DirectionToRoom(exitDirection.LEFT).exitsMade){
    //         RollForMoreExits(currentRoom.DirectionToRoom(exitDirection.LEFT));
    //     }
    //     if(currentRoom.roomExits.Contains(exitDirection.RIGHT)
    //        && !currentRoom.DirectionToRoom(exitDirection.RIGHT).exitsMade){
    //         RollForMoreExits(currentRoom.DirectionToRoom(exitDirection.RIGHT));
    //     }
    //     if(currentRoom.roomExits.Contains(exitDirection.UP)
    //        && !currentRoom.DirectionToRoom(exitDirection.UP).exitsMade){
    //         RollForMoreExits(currentRoom.DirectionToRoom(exitDirection.UP));
    //     }
    //     if(currentRoom.roomExits.Contains(exitDirection.DOWN)
    //        && !currentRoom.DirectionToRoom(exitDirection.DOWN).exitsMade){
    //         RollForMoreExits(currentRoom.DirectionToRoom(exitDirection.DOWN));
    //     }
    //     return;
    // }

    // /* RollForMoreExits() basically rolls a random number for the current room and
    //  * has a 1 in 15 chance to not create adittional exits for that room. If we
    //  * don't hit that 1 in 15 we generate more exits for the room at that index.
    //  */
    // void RollForMoreExits(Room room){
    //     if(UnityEngine.Random.Range(0, 14) == 0){
    //         room.exitsMade = true;
    //         return;
    //     }
    //     GenerateRoomExits(room);
    // }

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
    // void FixDeadEnds(){
    //     for(int i = 0; i < warehouseData.Count; i++){
    //         for(int j = 0; j < warehouseData[i].Count; j++){
    //             Room deadEnd = warehouseData[i][j];
    //             while(deadEnd.roomExits.Count == 1){
    //                 List<exitDirection> exitsToConsider = MakeExitsToConsider(deadEnd);
    //                 (Room leastExitRoom, exitDirection exitToMake, exitDirection opposingExitToMake) = FindLeastExits(deadEnd, exitsToConsider);
    //                 if(leastExitRoom != null){
    //                     deadEnd.roomExits.Add(exitToMake);
    //                     leastExitRoom.roomExits.Add(opposingExitToMake);
    //                     break;
    //                 }
    //                 else{
    //                     // choose random exit from the that leads to a nonexistent room
    //                     exitDirection randomExit = RandomExitFromList(exitsToConsider);
    //                     // fix the dead end with the adjacent room
    //                     deadEnd.roomExits.Add(randomExit);
    //                     Room newDeadEndRoom = deadEnd.DirectionToRoom(randomExit);
    //                     newDeadEndRoom.roomExits.Add(FindOpposingDirection(randomExit));
    //                     // set current dead end room to the new dead room we just made
    //                     deadEnd = newDeadEndRoom;
    //                 }
    //             }
    //         }
    //     }
    // }

    // /* MakeExitsToConsider() creates a list of exits that the
    //  * current room could possibly have with respect to bounds
    //  * checking as well as the exits the room already has. It
    //  * accepts a Room object and return a list with elements
    //  * of type exitDirection.
    //  */
    // List<exitDirection> MakeExitsToConsider (Room room){
    //     // creat a list of possible exits
    //     List<exitDirection> exitsToConsider = new List<exitDirection>(new exitDirection[] {
    //                     exitDirection.LEFT, exitDirection.RIGHT, exitDirection.UP, exitDirection.DOWN
    //                     });
    //     // remove all exits from exitsToConsider that the room already has
    //     for(int i = 0; i < room.roomExits.Count; i++){
    //         exitsToConsider.RemoveAt(exitsToConsider.IndexOf(room.roomExits[i]));
    //     }
    //     // remove exits with respect to bounds checking
    //     if(room.row == 0){
    //         exitsToConsider.RemoveAt(exitsToConsider.IndexOf(exitDirection.UP));
    //     }
    //     if(room.row == rows - 1){
    //         exitsToConsider.RemoveAt(exitsToConsider.IndexOf(exitDirection.DOWN));
    //     }
    //     if(room.column == 0){
    //         exitsToConsider.RemoveAt(exitsToConsider.IndexOf(exitDirection.LEFT));
    //     }
    //     if(room.column == columns - 1){
    //         exitsToConsider.RemoveAt(exitsToConsider.IndexOf(exitDirection.RIGHT));
    //     }
    //     return exitsToConsider;
    // }

    // /* FindLeastExits() an adjacent room that has the least amount of exits.
    //  * It accepts a room as well as a list of exitDirections that inform
    //  * the function of what rooms to examine. It first assumes that none of the
    //  * adjacent rooms exist. The for loop iterates through the exits list and refutes
    //  * this by checking the amount of exits described by each exitDirection. It
    //  * iterates through the exits list, finds the room based on that exit with respect
    //  * to room, checks if that room has less exits than leastExits, if so change some
    //  * return values and update leastExits. It returns a tuple with the room adjacent 
    //  * to the current room that has the least amount of exits, the direction towards that
    //  * room, and the opposing direction.
    //  */
    // (Room leastExitsRoom, exitDirection exitToMake, exitDirection opposingExitToMake) FindLeastExits(Room room, List<exitDirection> exits){
    //     int leastExits = 4;
    //     Room leastExitsRoom = null;
    //     exitDirection exitToMake = exitDirection.NONE;
    //     exitDirection opposingExitToMake = exitDirection.NONE;
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
    // void PlaceRooms(){
    //     int upper = 2;
    //     bool monsterPlaced = false;
    //     float minSpawnDist = Mathf.Pow((rows * columns), 0.25f);
    //     for(int i = 0; i < warehouseData.Count; i++){
    //         for(int j = 0; j < warehouseData[i].Count; j++){
    //             if(warehouseData[i][j].roomExits.Count == 0){
    //                 continue;
    //             }
    //             // create empty GameObject, move it to correct position and set its parent
    //             GameObject EmptyParentObject = new GameObject("Room " + i + " " + j);
    //             EmptyParentObject.transform.position = new Vector3((float)j * 20.0f, 0.0f, (float)i * -20.0f);
    //             EmptyParentObject.transform.SetParent(Warehouse.transform);
    //             UnityEngine.Object roomPrefab = null;
    //             GameObject roomObj = null;
    //             // check if room is far enough away to be a PVTM room
    //             float distToStart = Mathf.Sqrt((Mathf.Pow((warehouseData[i][j].row - startRoomRow), 2f)) + Mathf.Pow((warehouseData[i][j].column - startRoomColumn), 2f));
    //             if(i == startRoomRow && j == startRoomColumn){
    //                 Debug.Log("LOADING START ROOM");
    //                 roomPrefab = Resources.Load("ProcgenGreyboxes/room-elevator-quad");
    //                 roomObj = (GameObject)Instantiate(roomPrefab, EmptyParentObject.transform);
    //                 roomObj.transform.position = EmptyParentObject.transform.position;
    //                 warehouseData[i][j].prefab = roomObj;
    //                 player.transform.position = new Vector3(roomObj.transform.position.x, 2f, roomObj.transform.position.z);
    //                 playerCamera.transform.position = new Vector3(roomObj.transform.position.x, 2f, roomObj.transform.position.z);
    //             }
    //             else if(distToStart >= minSpawnDist && !monsterPlaced){
    //                 if(UnityEngine.Random.Range(0, upper) == 0){
    //                     Debug.Log("PLACING MONSTER AT " + i + " " + j);
    //                     if(warehouseData[i][j].roomExits.Count == 2){
    //                         exitDirection opposing = FindOpposingDirection(warehouseData[i][j].roomExits[0]);
    //                         if(warehouseData[i][j].roomExits.Contains(opposing)){
    //                             roomPrefab = Resources.Load("ProcgenGreyboxes/room-pvtm-straight");
    //                         }
    //                         else{
    //                             roomPrefab = Resources.Load("ProcgenGreyboxes/room-pvtm-elbow");
    //                         }
    //                     }
    //                     else if(warehouseData[i][j].roomExits.Count == 3){
    //                         roomPrefab = Resources.Load("ProcgenGreyboxes/room-pvtm-tri");
    //                     }
    //                     else{
    //                         roomPrefab = Resources.Load("ProcgenGreyboxes/room-pvtm-quad");
    //                     }
    //                     roomObj = (GameObject)Instantiate(roomPrefab, EmptyParentObject.transform);
    //                     roomObj.transform.position = EmptyParentObject.transform.position;
    //                     int rotation = RotatePrefab(warehouseData[i][j].roomExits);
    //                     roomObj.transform.Rotate(0 ,rotation ,0);
    //                     warehouseData[i][j].prefab = roomObj;
    //                     monster.transform.position = new Vector3(roomObj.transform.position.x, 2f, roomObj.transform.position.z);
    //                     monsterPlaced = true;
    //                 }
    //                 else{
    //                     upper -= 1;
    //                     string roomExits = System.String.Empty;
    //                     roomExits += warehouseData[i][j].roomExits.Contains(exitDirection.LEFT)  ? "L" : "_";
    //                     roomExits += warehouseData[i][j].roomExits.Contains(exitDirection.UP)    ? "U" : "_";
    //                     roomExits += warehouseData[i][j].roomExits.Contains(exitDirection.DOWN)  ? "D" : "_";
    //                     roomExits += warehouseData[i][j].roomExits.Contains(exitDirection.RIGHT) ? "R" : "_";
    //                     // choose random room width then load and instantiate prefab then move the prefab to the empty gameobject above
    //                     string roomWidth = UnityEngine.Random.Range(0, 2) == 0 ? "-thin" : "-wide";
    //                     roomPrefab = Resources.Load("ProcgenGreyboxes/room-" + roomExits + roomWidth); // note: not .prefab!
    //                     roomObj = (GameObject)Instantiate(roomPrefab, EmptyParentObject.transform);
    //                     roomObj.transform.position = EmptyParentObject.transform.position;
    //                     warehouseData[i][j].prefab = roomObj;
    //                 }
    //             }
    //             else{
    //                 string roomExits = System.String.Empty;
    //                 roomExits += warehouseData[i][j].roomExits.Contains(exitDirection.LEFT)  ? "L" : "_";
    //                 roomExits += warehouseData[i][j].roomExits.Contains(exitDirection.UP)    ? "U" : "_";
    //                 roomExits += warehouseData[i][j].roomExits.Contains(exitDirection.DOWN)  ? "D" : "_";
    //                 roomExits += warehouseData[i][j].roomExits.Contains(exitDirection.RIGHT) ? "R" : "_";
    //                 // choose random room width then load and instantiate prefab then move the prefab to the empty gameobject above
    //                 string roomWidth = UnityEngine.Random.Range(0, 2) == 0 ? "-thin" : "-wide";
    //                 roomPrefab = Resources.Load("ProcgenGreyboxes/room-" + roomExits + roomWidth); // note: not .prefab!
    //                 roomObj = (GameObject)Instantiate(roomPrefab, EmptyParentObject.transform);
    //                 roomObj.transform.position = EmptyParentObject.transform.position;
    //                 warehouseData[i][j].prefab = roomObj;
    //             }
    //         }
    //     }
    // }

    // int RotatePrefab(List<exitDirection> exits){
    //     if(exits.Count == 2){
    //         exitDirection opposite = FindOpposingDirection(exits[0]);
    //         if(exits.Contains(opposite)){
    //             if(exits.Contains(exitDirection.LEFT) && exits.Contains(exitDirection.RIGHT)){
    //                 return 90;
    //             }
    //         }
    //         else{
    //             if(exits.Contains(exitDirection.DOWN) && exits.Contains(exitDirection.RIGHT)){
    //                 return 90;
    //             }
    //             if(exits.Contains(exitDirection.DOWN) && exits.Contains(exitDirection.LEFT)){
    //                 return 180;
    //             }
    //             if(exits.Contains(exitDirection.LEFT) && exits.Contains(exitDirection.UP)){
    //                 return -90;
    //             }
    //         }
    //     }
    //     else if(exits.Count == 3){
    //         if(exits.Contains(exitDirection.DOWN) && 
    //            exits.Contains(exitDirection.RIGHT) && 
    //            exits.Contains(exitDirection.UP)){
    //             return 180;
    //         }
    //         if(exits.Contains(exitDirection.DOWN) && 
    //            exits.Contains(exitDirection.RIGHT) && 
    //            exits.Contains(exitDirection.LEFT)){
    //             return -90;
    //         }
    //         if(exits.Contains(exitDirection.LEFT) && 
    //            exits.Contains(exitDirection.RIGHT) && 
    //            exits.Contains(exitDirection.UP)){
    //             return 90;
    //         }
    //     }
    //     return 0;
    // }

    // /* PrintWarehouseData() logs the exits of each index to the console
    //  * "L" "U" "D" "R" signify the direction of the exit at each index
    //  */
    // void PrintWarehouseData(){
    //     Debug.Log("PRINTING WAREHOUSE ROOM DATA");
    //     for(int i = 0; i < warehouseData.Count; i++){
    //         string rowData = System.String.Empty;
    //         for(int j = 0; j < warehouseData[i].Count; j++){
    //             rowData += warehouseData[i][j].roomExits.Contains(exitDirection.LEFT)  ? "L" : "_";
    //             rowData += warehouseData[i][j].roomExits.Contains(exitDirection.UP)    ? "U" : "_";
    //             rowData += warehouseData[i][j].roomExits.Contains(exitDirection.DOWN)  ? "D" : "_";
    //             rowData += warehouseData[i][j].roomExits.Contains(exitDirection.RIGHT) ? "R" : "_";
    //             rowData += " ";
    //         }
    //         rowData += "\n";
    //         Debug.Log(rowData);
    //     }
    // }

    // // printDirection() just prints a string based on the parameter dir
    // void PrintExitDirection(exitDirection  dir){
    //     switch (dir){
    //         case exitDirection.UP:
    //             Debug.Log("UP");
    //             return;
    //         case exitDirection.DOWN:
    //             Debug.Log("DOWN");
    //             return;
    //         case exitDirection.LEFT:
    //             Debug.Log("LEFT");
    //             return;
    //         case exitDirection.RIGHT:
    //             Debug.Log("RIGHT");
    //             return;
    //     }
    // }

    // // RandomExitFromList() returns a random exitDirection from a list
    // exitDirection RandomExitFromList(List<exitDirection> list){
    //     return list[UnityEngine.Random.Range(0, list.Count)];
    // }

    // // FindOpposingDirection() returns the direction opposite from the one provided
    // exitDirection FindOpposingDirection(exitDirection dir){
    //     switch (dir){
    //         case exitDirection.UP:
    //             return exitDirection.DOWN;
    //         case exitDirection.DOWN:
    //             return exitDirection.UP;
    //         case exitDirection.LEFT:
    //             return exitDirection.RIGHT;
    //         case exitDirection.RIGHT:
    //             return exitDirection.LEFT;
    //     }
    //     return exitDirection.NONE;
    // }
}

// // class primarily used for holding data about what exits are in each room
// public class Room
// {
//     public List<exitDirection> roomExits = new List<exitDirection>();
//     public string type = "NONE";
//     public bool exitsMade = false;
//     public List<List<Room>> warehouseData = null;
//     public int row;
//     public int column;
//     public GameObject prefab;

//     // DirectionToRoom() returns a Room object based on the direction passed
//     // This does not account for bounds checking
//     public Room DirectionToRoom (exitDirection dir){
//         switch (dir){
//             case exitDirection.UP:
//                 return warehouseData[this.row - 1][ this.column];
//             case exitDirection.DOWN:
//                 return warehouseData[this.row + 1][ this.column];
//             case exitDirection.LEFT:
//                 return warehouseData[this.row][ this.column - 1];
//             case exitDirection.RIGHT:
//                 return warehouseData[this.row][ this.column + 1];
//         }
//         return null;
//     }

//     // RoomToDirection() returns a direction based on a provided room
//     // This does not account for bounds checking
//     public exitDirection RoomToDirection(Room room){
//         if(room.row + 1 == this.row && room.column == this.column){
//             return exitDirection.DOWN;
//         }
//         else if(room.row - 1 == this.row && room.column == this.column){
//             return exitDirection.UP;
//         }
//         else if(room.row == this.row && room.column + 1 == this.column){
//             return exitDirection.LEFT;
//         }
//         else if(room.row == this.row && room.column - 1 == this.column){
//             return exitDirection.RIGHT;
//         }
//         else{
//             return exitDirection.NONE;
//         }
//     }
// }
