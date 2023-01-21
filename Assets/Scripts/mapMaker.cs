using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum directionOfExit
{
    LEFT,
    RIGHT,
    UP,
    DOWN
}

public class MapMaker : MonoBehaviour
{

    [SerializeField] private int rows;
    [SerializeField] private int columns;
    [SerializeField] private int startingRoomRow;
    [SerializeField] private int startingRoomColumn;
    [SerializeField] private GameObject Warehouse;

    public List<List<roomData>> warehouseDataArray = new List<List<roomData>>();

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
                rowOfEmptyRooms.Add(emptyRoom);
            }
            warehouseDataArray.Add(rowOfEmptyRooms);
        }
        warehouseDataArray[startingRoomRow][startingRoomColumn].roomType = "STARTING ROOM";
        GenerateRoomExits(startingRoomRow, startingRoomColumn);
        // PrintWarehouseData();
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
        if(warehouseDataArray[currentRow][currentColumn].exitsMade){
            return;
        }
        // fill an array with all the exit directions
        List<directionOfExit> potentialExits = new List<directionOfExit>(new directionOfExit[] {
                              directionOfExit.LEFT, directionOfExit.RIGHT, directionOfExit.UP, directionOfExit.DOWN
                              });
        float distToStartingRoom = Mathf.Sqrt((Mathf.Pow((currentRow - startingRoomRow), 2f)) + Mathf.Pow((currentColumn - startingRoomColumn), 2f));
        // if distance of the current room to the start room is greater less than 2 then we up the RNG of choosing more exits
        int numOfPotentialExits = distToStartingRoom < 2 ? UnityEngine.Random.Range(3, 4) : UnityEngine.Random.Range(2, 3);
        for(; numOfPotentialExits > 0; numOfPotentialExits--){
            // get random index from potentialExits which gives random a room direction
            int indexOfExitDirection = UnityEngine.Random.Range(0, potentialExits.Count);
            // if current room doesn't already have a room equal to the value at the random index
            //  then make the exit in the current room of the value at the random index
            if(!warehouseDataArray[currentRow][currentColumn].roomExits.Contains(potentialExits[indexOfExitDirection])){
                switch(potentialExits[indexOfExitDirection]){
                    case directionOfExit.LEFT:
                        if(currentColumn != 0){                                                                     // bounds checking to not add a room exit to an index that doesn't exit
                            warehouseDataArray[currentRow][currentColumn].roomExits.Add(directionOfExit.LEFT);      // update roomExits array to have the desired room
                            warehouseDataArray[currentRow][currentColumn - 1].roomExits.Add(directionOfExit.RIGHT); // update adjacent room to have a corresponding exit as well
                        }
                        break;
                    case directionOfExit.RIGHT:
                        if(currentColumn != columns - 1){
                            warehouseDataArray[currentRow][currentColumn].roomExits.Add(directionOfExit.RIGHT);
                            warehouseDataArray[currentRow][currentColumn + 1].roomExits.Add(directionOfExit.LEFT);
                        }
                        break;
                    case directionOfExit.UP:
                        if(currentRow != 0){
                            warehouseDataArray[currentRow][currentColumn].roomExits.Add(directionOfExit.UP);
                            warehouseDataArray[currentRow - 1][currentColumn].roomExits.Add(directionOfExit.DOWN);
                        }
                        break;
                    case directionOfExit.DOWN:
                        if(currentRow != rows - 1){
                            warehouseDataArray[currentRow][currentColumn].roomExits.Add(directionOfExit.DOWN);
                            warehouseDataArray[currentRow + 1][currentColumn].roomExits.Add(directionOfExit.UP);
                        }
                        break;
                }
            }
            potentialExits.RemoveAt(indexOfExitDirection);  // then remove that random index so it doesn't get chosen again
        }
        warehouseDataArray[currentRow][currentColumn].exitsMade = true;
        // for all of the exits we just made in the current room roll a random number to see if we make more exits in that respective room
        if(warehouseDataArray[currentRow][currentColumn].roomExits.Contains(directionOfExit.LEFT)
           && !warehouseDataArray[currentRow][currentColumn - 1].exitsMade){
            RollForMoreExits(currentRow, currentColumn - 1);
        }
        if(warehouseDataArray[currentRow][currentColumn].roomExits.Contains(directionOfExit.RIGHT)
           && !warehouseDataArray[currentRow][currentColumn + 1].exitsMade){
            RollForMoreExits(currentRow, currentColumn + 1);
        }
        if(warehouseDataArray[currentRow][currentColumn].roomExits.Contains(directionOfExit.UP)
           && !warehouseDataArray[currentRow - 1][currentColumn].exitsMade){
            RollForMoreExits(currentRow - 1, currentColumn);
        }
        if(warehouseDataArray[currentRow][currentColumn].roomExits.Contains(directionOfExit.DOWN)
           && !warehouseDataArray[currentRow + 1][currentColumn].exitsMade){
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
            warehouseDataArray[row][column].exitsMade = true;
            return;
        }
        GenerateRoomExits(row, column);
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
        for(int i = 0; i < warehouseDataArray.Count; i++){
            for(int j = 0; j < warehouseDataArray[i].Count; j++){
                if(warehouseDataArray[i][j].roomExits.Count == 0){
                    continue;
                }
                string roomExits = System.String.Empty;
                roomExits += warehouseDataArray[i][j].roomExits.Contains(directionOfExit.LEFT)  ? "L" : "_";
                roomExits += warehouseDataArray[i][j].roomExits.Contains(directionOfExit.UP)    ? "U" : "_";
                roomExits += warehouseDataArray[i][j].roomExits.Contains(directionOfExit.DOWN)  ? "D" : "_";
                roomExits += warehouseDataArray[i][j].roomExits.Contains(directionOfExit.RIGHT) ? "R" : "_";
                GameObject EmptyParentObject = new GameObject("Room " + i + " " + j);
                EmptyParentObject.transform.position = new Vector3((float)j * 20.0f, 0.0f, (float)i * -20.0f);
                EmptyParentObject.transform.SetParent(Warehouse.transform);
                UnityEngine.Object roomPrefab = Resources.Load("ProcgenGreyboxes/room-" + roomExits + "-thin"); // note: not .prefab!
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
        for(int i = 0; i < warehouseDataArray.Count; i++){
            string rowData = System.String.Empty;
            for(int j = 0; j < warehouseDataArray[i].Count; j++){
                rowData += warehouseDataArray[i][j].roomExits.Contains(directionOfExit.LEFT)  ? "L" : "_";
                rowData += warehouseDataArray[i][j].roomExits.Contains(directionOfExit.UP)    ? "U" : "_";
                rowData += warehouseDataArray[i][j].roomExits.Contains(directionOfExit.DOWN)  ? "D" : "_";
                rowData += warehouseDataArray[i][j].roomExits.Contains(directionOfExit.RIGHT) ? "R" : "_";
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
    public List<directionOfExit> roomExits = new List<directionOfExit>();
    public string roomType = "NONE";
    public bool exitsMade = false;
}
