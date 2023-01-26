using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private int startingRoomRow;
    [SerializeField] private int startingRoomColumn;
    [SerializeField] private GameObject Warehouse;

    public List<List<roomData>> warehouseData = new List<List<roomData>>();

    void Start()
    {
        // error checking to make sure that the provided starting row and column
        // is a valid index in the provided dimensions of the 2D arrray
        if(startingRoomRow >= rows || startingRoomColumn >= columns 
            || startingRoomRow < 0 || startingRoomColumn < 0){
            startingRoomRow = UnityEngine.Random.Range(0, rows - 1);
            startingRoomColumn = UnityEngine.Random.Range(0, columns - 1);
        }
        for(int i = 0; i < rows; i++){ // Add empty "rooms" to the 2D Array based on the provided dimensions
            List<roomData> rowOfEmptyRooms = new List<roomData>();
            for(int j = 0; j < columns; j++){
                roomData emptyRoom = new roomData();
                emptyRoom.rowIndex = i;
                emptyRoom.columnIndex = j;
                rowOfEmptyRooms.Add(emptyRoom);
            }
            warehouseData.Add(rowOfEmptyRooms);
        }
        warehouseData[startingRoomRow][startingRoomColumn].roomType = "STARTING ROOM";
        GenerateRoomExits(startingRoomRow, startingRoomColumn);
        // PrintWarehouseData();
        FixDeadEnds();
        PlaceRooms();
    }

    /* GenerateRoomExits() is the driving algorithm for procedurally filling a 2D array with
     * "rooms". The algorithm will first generate a random number that decides how many 
     * potential exits the current room will have. It then chooses a random index from the 
     * array potentialExits. If the current room doesn't have an exit at the value of that
     * index, it creates that exit then createsanother opposing exit at the room adjacent. 
     * So if we make a left exit at the current room we make a right exit at the room to the 
     * left of the current one. The algorithm will then looks at all exits the current room 
     * has and then pick another random number that decides if the next room will have more 
     * exits which is handled by RollForMoreExits().
     */
    void GenerateRoomExits(int currentRow, int currentColumn){
        if(warehouseData[currentRow][currentColumn].exitsMade){
            return;
        }
        // fill an array with all the exit directions
        List<exitDirection> potentialExits = new List<exitDirection>(new exitDirection[] {
                              exitDirection.LEFT, exitDirection.RIGHT, exitDirection.UP, exitDirection.DOWN
                              });
        float distToStartingRoom = Mathf.Sqrt((Mathf.Pow((currentRow - startingRoomRow), 2f)) + Mathf.Pow((currentColumn - startingRoomColumn), 2f));
        // if distance of the current room to the start room is less or equal to 1 then we up the RNG of choosing more exits
        int numOfPotentialExits = distToStartingRoom <= 1 ? UnityEngine.Random.Range(3, 4) : UnityEngine.Random.Range(2, 3);
        for(; numOfPotentialExits > 0; numOfPotentialExits--){
            // get random index from potentialExits which gives random a room direction
            int indexOfExitDirection = UnityEngine.Random.Range(0, potentialExits.Count);
            // if current room doesn't already have a room equal to the value at the random index
            //  then make the exit in the current room of the value at the random index
            if(!warehouseData[currentRow][currentColumn].roomExits.Contains(potentialExits[indexOfExitDirection])){
                switch(potentialExits[indexOfExitDirection]){
                    case exitDirection.LEFT:
                        if(currentColumn != 0){                                                                     // bounds checking to not add a room exit to an index that doesn't exit
                            warehouseData[currentRow][currentColumn].roomExits.Add(exitDirection.LEFT);      // update roomExits array to have the desired room
                            warehouseData[currentRow][currentColumn - 1].roomExits.Add(exitDirection.RIGHT); // update adjacent room to have a corresponding exit as well
                        }
                        break;
                    case exitDirection.RIGHT:
                        if(currentColumn != columns - 1){
                            warehouseData[currentRow][currentColumn].roomExits.Add(exitDirection.RIGHT);
                            warehouseData[currentRow][currentColumn + 1].roomExits.Add(exitDirection.LEFT);
                        }
                        break;
                    case exitDirection.UP:
                        if(currentRow != 0){
                            warehouseData[currentRow][currentColumn].roomExits.Add(exitDirection.UP);
                            warehouseData[currentRow - 1][currentColumn].roomExits.Add(exitDirection.DOWN);
                        }
                        break;
                    case exitDirection.DOWN:
                        if(currentRow != rows - 1){
                            warehouseData[currentRow][currentColumn].roomExits.Add(exitDirection.DOWN);
                            warehouseData[currentRow + 1][currentColumn].roomExits.Add(exitDirection.UP);
                        }
                        break;
                }
            }
            potentialExits.RemoveAt(indexOfExitDirection);  // then remove that random index so it doesn't get chosen again
        }
        warehouseData[currentRow][currentColumn].exitsMade = true;
        // for all of the exits we just made in the current room roll a random number to see if we make more exits in that respective room
        if(warehouseData[currentRow][currentColumn].roomExits.Contains(exitDirection.LEFT)
           && !warehouseData[currentRow][currentColumn - 1].exitsMade){
            RollForMoreExits(currentRow, currentColumn - 1);
        }
        if(warehouseData[currentRow][currentColumn].roomExits.Contains(exitDirection.RIGHT)
           && !warehouseData[currentRow][currentColumn + 1].exitsMade){
            RollForMoreExits(currentRow, currentColumn + 1);
        }
        if(warehouseData[currentRow][currentColumn].roomExits.Contains(exitDirection.UP)
           && !warehouseData[currentRow - 1][currentColumn].exitsMade){
            RollForMoreExits(currentRow - 1, currentColumn);
        }
        if(warehouseData[currentRow][currentColumn].roomExits.Contains(exitDirection.DOWN)
           && !warehouseData[currentRow + 1][currentColumn].exitsMade){
            RollForMoreExits(currentRow + 1, currentColumn);
        }
        return;
    }

    /* RollForMoreExits() basically rolls a random number for the current room and
     * has a 1 in 15 chance to not create adittional exits for that room. If we
     * don't hit that 1 in 15 we generate more exits for the room at that index.
     */
    void RollForMoreExits(int row, int column){
        if(UnityEngine.Random.Range(0, 14) == 0){
            warehouseData[row][column].exitsMade = true;
            return;
        }
        GenerateRoomExits(row, column);
    }

    void FixDeadEnds(){
        for(int i = 0; i < warehouseData.Count; i++){
            for(int j = 0; j < warehouseData[i].Count; j++){
                if(warehouseData[i][j].roomExits.Count == 1){
                    Debug.Log("DEAD END AT ROW " + i + " COLUMN " + j);
                    List<exitDirection> exitsToConsider = MakeExitsToConsider(i, j);
                    Debug.Log(exitsToConsider.Count);
                    (roomData leastExitRoom, exitDirection exitToMake, exitDirection opposingExitToMake) = FindLeastExits(i, j, exitsToConsider);
                    if(leastExitRoom != null){
                        warehouseData[i][j].roomExits.Add(exitToMake);
                        leastExitRoom.roomExits.Add(opposingExitToMake);
                    }
                    else{
                        // pick random room from exitToConsider and then start generating rooms from there
                        Debug.Log("CANT PATCH DEAD END MAKING MORE RANDOM ROOMS");
                        (int newRow, int newColumn) = warehouseData[i][j].getIndexFromDirection(exitsToConsider[0]);
                        Debug.Log("MAKING MORE ROOMS AT ROW " + newRow + " COLUMN " + newColumn);
                        GenerateRoomExits(newRow, newColumn);
                        exitToMake = warehouseData[i][j].getDirectionFromIndex(newRow, newColumn);
                        opposingExitToMake = warehouseData[newRow][newColumn].getDirectionFromIndex(i, j);
                        Debug.Log("PATCHIN DEAD END BETWEEN ROW " + i + " COLUMN " + j + " AND ROW " + newRow + " COLUMN " + newColumn);
                        warehouseData[i][j].roomExits.Add(exitToMake);
                        warehouseData[newRow][newColumn].roomExits.Add(opposingExitToMake);
                        // consider the case in which we make a try to fix a dead end room by making another room
                        // but that new room creates another dead end at an index that has already been skipped over
                        // in the nested for loops above
                    }
                }
            }
        }
    }

    List<exitDirection> MakeExitsToConsider (int row, int column){
        List<exitDirection> exitsToConsider = new List<exitDirection>(new exitDirection[] {
                        exitDirection.LEFT, exitDirection.RIGHT, exitDirection.UP, exitDirection.DOWN
                        });
        exitsToConsider.RemoveAt(exitsToConsider.IndexOf(warehouseData[row][column].roomExits[0]));
        if(row == 0){
            exitsToConsider.RemoveAt(exitsToConsider.IndexOf(exitDirection.UP));
        }
        if(row == rows - 1){
            exitsToConsider.RemoveAt(exitsToConsider.IndexOf(exitDirection.DOWN));
        }
        if(column == 0){
            exitsToConsider.RemoveAt(exitsToConsider.IndexOf(exitDirection.LEFT));
        }
        if(column == columns - 1){
            exitsToConsider.RemoveAt(exitsToConsider.IndexOf(exitDirection.RIGHT));
        }
        return exitsToConsider;
    }

    (roomData leastExitsRoom, exitDirection exitToMake, exitDirection opposingExitToMake) FindLeastExits(int row, int column, List<exitDirection> exits){
        int leastExits = 4;
        roomData leastExitsRoom = null;
        exitDirection exitToMake = exitDirection.NONE;
        exitDirection opposingExitToMake = exitDirection.NONE;
        for(int i = 0; i < exits.Count; i++){
            switch (exits[i]){
                case exitDirection.UP:
                    if(warehouseData[row - 1][column].roomExits.Count != 0 && 
                       warehouseData[row - 1][column].roomExits.Count < leastExits){
                        opposingExitToMake = exitDirection.DOWN;
                        exitToMake     = exitDirection.UP;
                        leastExitsRoom = warehouseData[row - 1][column];
                        leastExits     = warehouseData[row - 1][column].roomExits.Count;
                    }
                    break;
                case exitDirection.DOWN:
                    if(warehouseData[row + 1][column].roomExits.Count != 0 && 
                       warehouseData[row + 1][column].roomExits.Count < leastExits){
                        opposingExitToMake = exitDirection.UP;
                        exitToMake     = exitDirection.DOWN;
                        leastExitsRoom = warehouseData[row + 1][column];
                        leastExits     = warehouseData[row + 1][column].roomExits.Count;
                    }
                    break;
                case exitDirection.RIGHT:
                    if(warehouseData[row][column + 1].roomExits.Count != 0 && 
                       warehouseData[row][column + 1].roomExits.Count < leastExits){
                        opposingExitToMake = exitDirection.LEFT;
                        exitToMake     = exitDirection.RIGHT;
                        leastExitsRoom = warehouseData[row][column + 1];
                        leastExits     = warehouseData[row][column + 1].roomExits.Count;
                    }
                    break;
                case exitDirection.LEFT:
                    if(warehouseData[row][column - 1].roomExits.Count != 0 && 
                       warehouseData[row][column - 1].roomExits.Count < leastExits){
                        opposingExitToMake = exitDirection.RIGHT;
                        exitToMake     = exitDirection.LEFT;
                        leastExitsRoom = warehouseData[row][column - 1];
                        leastExits     = warehouseData[row][column - 1].roomExits.Count;
                    }
                    break;
            }
        }
        return (leastExitsRoom, exitToMake, opposingExitToMake);
    }

    /* PlaceRooms() will iterate through the 2D array of roomData and
     * will instantiate rooms based the current rooms exits and places them
     * appropriately. It first build a string based off of its exits which is
     * then used to load a prefab from the folder "Assets/Resources/ProcgenGreyboxes".
     * It places an empty game object at the correct location within world space. This
     * was done to ensure that the original rotation of the prefab doesn't get altered
     * due to the Instantiate() method. EmptyGameObject's parent is set to the Warehouse
     * GameObject for organization within the editor. It then loads the prefab and
     * sets its position to the previously mentioned EmptyGameObject.
     */
    void PlaceRooms(){
        for(int i = 0; i < warehouseData.Count; i++){
            for(int j = 0; j < warehouseData[i].Count; j++){
                if(warehouseData[i][j].roomExits.Count == 0){
                    continue;
                }
                string roomExits = System.String.Empty;
                roomExits += warehouseData[i][j].roomExits.Contains(exitDirection.LEFT)  ? "L" : "_";
                roomExits += warehouseData[i][j].roomExits.Contains(exitDirection.UP)    ? "U" : "_";
                roomExits += warehouseData[i][j].roomExits.Contains(exitDirection.DOWN)  ? "D" : "_";
                roomExits += warehouseData[i][j].roomExits.Contains(exitDirection.RIGHT) ? "R" : "_";
                GameObject EmptyParentObject = new GameObject("Room " + i + " " + j);
                EmptyParentObject.transform.position = new Vector3((float)j * 20.0f, 0.0f, (float)i * -20.0f);
                EmptyParentObject.transform.SetParent(Warehouse.transform);
                string roomWidth = UnityEngine.Random.Range(0, 2) == 0 ? "-thin" : "-wide";
                UnityEngine.Object roomPrefab = Resources.Load("ProcgenGreyboxes/room-" + roomExits + roomWidth); // note: not .prefab!
                GameObject placedRoomObject = (GameObject)Instantiate(roomPrefab, EmptyParentObject.transform);
                placedRoomObject.transform.position = EmptyParentObject.transform.position;
            }
        }
    }

    /* PrintWarehouseData() logs the exits of each index to the console
     * "L" "U" "D" "R" signify the direction of the exit at each index
     */
    void PrintWarehouseData(){
        Debug.Log("PRINTING WAREHOUSE ROOM DATA");
        for(int i = 0; i < warehouseData.Count; i++){
            string rowData = System.String.Empty;
            for(int j = 0; j < warehouseData[i].Count; j++){
                rowData += warehouseData[i][j].roomExits.Contains(exitDirection.LEFT)  ? "L" : "_";
                rowData += warehouseData[i][j].roomExits.Contains(exitDirection.UP)    ? "U" : "_";
                rowData += warehouseData[i][j].roomExits.Contains(exitDirection.DOWN)  ? "D" : "_";
                rowData += warehouseData[i][j].roomExits.Contains(exitDirection.RIGHT) ? "R" : "_";
                rowData += " ";
            }
            rowData += "\n";
            Debug.Log(rowData);
        }
    }
}

// class primarily used for holding data about what exits are in each room
public class roomData
{
    public List<exitDirection> roomExits = new List<exitDirection>();
    public string roomType = "NONE";
    public bool exitsMade = false;
    public int rowIndex;
    public int columnIndex;

    public (int row, int column) getIndexFromDirection(exitDirection dir){
        if(dir == exitDirection.UP){
            return (rowIndex - 1, columnIndex);
        }
        else if(dir == exitDirection.DOWN){
            return (rowIndex + 1, columnIndex);
        }
        else if(dir == exitDirection.LEFT){
            return (rowIndex, columnIndex - 1);
        }
        else{
            return (rowIndex, columnIndex + 1);
        }
    }

    public exitDirection getDirectionFromIndex(int row, int column){
        if(row == rowIndex && column + 1 == columnIndex){
            return exitDirection.LEFT;
        }
        else if(row == rowIndex && column - 1 == columnIndex){
            return exitDirection.RIGHT;
        }
        else if(row + 1 == rowIndex && column == columnIndex){
            return exitDirection.UP;
        }
        else{
            return exitDirection.DOWN;
        }
    }
}
