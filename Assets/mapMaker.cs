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

public class mapMaker : MonoBehaviour
{

    [SerializeField] private int rows;
    [SerializeField] private int columns;
    [SerializeField] private int startingRoomRow;
    [SerializeField] private int startingRoomColumn;

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

        // Add empty "rooms" to the 2D Array based on the provided dimensions
        for(int i = 0; i < rows; i++){
            List<roomData> rowOfEmptyRooms = new List<roomData>();
            for(int j = 0; j < columns; j++){
                roomData emptyRoom = new roomData();
                rowOfEmptyRooms.Add(emptyRoom);
            }
            warehouseDataArray.Add(rowOfEmptyRooms);
        }
        warehouseDataArray[startingRoomRow][startingRoomColumn].roomType = "STARTING ROOM";
        generateRoomExits(startingRoomRow, startingRoomColumn);
        printWarehouseData();
    }

    /* generateRoomExits() is the driving algorithm for procedurally filling a 2D array with
     * "rooms". The algorithm will first generate a random number that decides how many 
     * potential exits the current room will have. It then chooses a random index from the 
     * array potentialExits. If the current room doesn't have an exit at the value of that
     * index, it creates that exit then createsanother opposing exit at the room adjacent. 
     * So if we make a left exit at the current room we make a right exit at the room to the 
     * left of the current one. The algorithm will then looks at all exits the current room 
     * has and then pick another random number that decides if the next room will have more 
     * exits which is handled by rollForMoreExits().
     */
    void generateRoomExits(int currentRow, int currentColumn){
        if(warehouseDataArray[currentRow][currentColumn].exitsMade){
            return;
        }
        // fill an array with all the exit directions
        List<directionOfExit> potentialExits = new List<directionOfExit>(new directionOfExit[] {
                              directionOfExit.LEFT, directionOfExit.RIGHT, directionOfExit.UP, directionOfExit.DOWN
                              });
        // if we are at the "starting room" then we up the probabilty of having
        // many exits, if not then we lower it, rooms generally have about 2 exits
        // however its not perfect and occassionally we get rooms with 1 exit
        int numOfPotentialExits = (currentRow == startingRoomRow && currentColumn == startingRoomColumn) ? UnityEngine.Random.Range(3, 4) : UnityEngine.Random.Range(2, 4);
        for(; numOfPotentialExits > 0; numOfPotentialExits--){
            // get random index from potentialExits which gives random a room direction
            int indexOfExitDirection = UnityEngine.Random.Range(0, potentialExits.Count);
            // if current room doesn't already have a room equal to the value at the random index
            //  then make the exit in the current room of the value at the random index
            if(!warehouseDataArray[currentRow][currentColumn].roomExits.Contains(potentialExits[indexOfExitDirection])){
                switch(potentialExits[indexOfExitDirection]){
                    case directionOfExit.LEFT:
                        // bounds checking to not add a room exit to an index that doesn't exit
                        if(currentColumn != 0){
                            // add exit to the current room
                            warehouseDataArray[currentRow][currentColumn].roomExits.Add(directionOfExit.LEFT);
                            // update adjacent room to have a corresponding exit as well
                            warehouseDataArray[currentRow][currentColumn - 1].roomExits.Add(directionOfExit.RIGHT);
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
            // then remove that random index so it doesn't get chosen again
            potentialExits.RemoveAt(indexOfExitDirection);
        }
        warehouseDataArray[currentRow][currentColumn].exitsMade = true;
        // for each room corresponding to exits we just made in the current room 
        // roll a random number to see if we make more exits in that respective room
        if(warehouseDataArray[currentRow][currentColumn].roomExits.Contains(directionOfExit.LEFT)
           && !warehouseDataArray[currentRow][currentColumn - 1].exitsMade){
            rollForMoreExits(currentRow, currentColumn - 1);
        }
        if(warehouseDataArray[currentRow][currentColumn].roomExits.Contains(directionOfExit.RIGHT)
           && !warehouseDataArray[currentRow][currentColumn + 1].exitsMade){
            rollForMoreExits(currentRow, currentColumn + 1);
        }
        if(warehouseDataArray[currentRow][currentColumn].roomExits.Contains(directionOfExit.UP)
           && !warehouseDataArray[currentRow - 1][currentColumn].exitsMade){
            rollForMoreExits(currentRow - 1, currentColumn);
        }
        if(warehouseDataArray[currentRow][currentColumn].roomExits.Contains(directionOfExit.DOWN)
           && !warehouseDataArray[currentRow + 1][currentColumn].exitsMade){
            rollForMoreExits(currentRow + 1, currentColumn);
        }
        return;
    }

    void rollForMoreExits(int row, int column){
        // 1/15 chance we do not make more exits for the current room
        // UnityEngine.Random.Range() is inclusive
        if(UnityEngine.Random.Range(0, 14) == 0){
            warehouseDataArray[row][column].exitsMade = true;
            return;
        }
        generateRoomExits(row, column);
    }

    /* printWarehouseData() logs the exits of each index to the console
     * "L" "U" "D" "R" signify the direction of the exit at each index
     */
    void printWarehouseData(){
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
