using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit{
    // printDirection() just prints a string based on the parameter dir
    public virtual void PrintExit(){
        // switch (dir){
        //     case typeof(Up):
        //         Debug.Log("UP");
        //         return;
        //     case typeof(Down):
        //         Debug.Log("DOWN");
        //         return;
        //     case typeof(Left):
        //         Debug.Log("LEFT");
        //         return;
        //     case typeof(Right):
        //         Debug.Log("RIGHT");
        //         return;
        // }
        // Debug.Log(type);
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

public class Right : Exit{
    public override void PrintExit(){
        Debug.Log(typeof(Right));
    }
    public System.Type OppositeDirection(){
        return typeof(Left);
    }
}

public class Left : Exit{
    public override void PrintExit(){
        Debug.Log(typeof(Left));
    }
    public System.Type OppositeDirection(){
        return typeof(Right);
    }
}

public class Up : Exit{
    public override void PrintExit(){
        Debug.Log(typeof(Up));
    }
    public System.Type OppositeDirection(){
        return typeof(Down);
    }
}

public class Down : Exit{
    public override void PrintExit(){
        Debug.Log(typeof(Down));
    }
    public System.Type OppositeDirection(){
        return typeof(Up);
    }
}