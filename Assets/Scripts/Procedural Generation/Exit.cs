using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public enum Direction
// {
//     LEFT,
//     RIGHT,
//     UP,
//     DOWN,
//     NONE
// }

// // FindOpposingDirection() returns the direction opposite from the one provided
// public string OppositeDirection(Direction exit){
//     switch (exit){
//         case Direction.UP:
//             return Direction.DOWN;
//         case Direction.DOWN:
//             return Direction.UP;
//         case Direction.LEFT:
//             return Direction.RIGHT;
//         case Direction.RIGHT:
//             return Direction.LEFT;
//     }
//     return Direction.NONE;
// }

public class Exit{
    public string type;

    public Exit(string type){
        this.type = type;
    }

    // printExit() just prints a string based on the parameter dir
    public void PrintExit(){
        Debug.Log(this.type);
    }

    public string OppositeDirection(){
        switch(this.type){
            case "Right":
                return "Left";
            case "Left":
                return "Right";
            case "Up":
                return "Down";
            case "Down":
                return "Up";
        }
        return "None";
    }

    // // RandomExitFromList() returns a random Exit from a list
    // Exit RandomExitFromList(List<Exit> list){
    //     return list[UnityEngine.Random.Range(0, list.Count)];
    // }

    // FindOpposingDirection() returns the direction opposite from the one provided
    // public string OppositeDirection(){
    //     switch (type){
    //         case "Up":
    //             return "Down";
    //         case "Down":
    //             return :"Up";
    //         case "Left":
    //             return "Right";
    //         case "Right":
    //             return "Left";
    //     }
    //     return "None";
    // }
}
