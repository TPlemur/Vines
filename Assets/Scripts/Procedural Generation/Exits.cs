using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Exits class contains information about the types of
 * exits a room has. It essentially functions as a List
 * but provides functionality for giving the opposite
 * direction for a type of exits.
 */
public class Exits{
    
    public List<string> types;

    public Exits(bool allExits){
        this.types = new List<string>();
        if(allExits){
            this.AddRandom(4);
        }
    }

    public Exits(List<string> provided){
        foreach(string dir in provided){
            this.types.Add(dir);
        }
    }

    ~Exits(){
        types = null;
    }

    public void Add(string dir){
        this.types.Add(dir);
    }

    public void Add(List<string> dirs){
        foreach(string dir in dirs){
            types.Add(dir);
        }
    }

    public bool Has(string dir){
        var index = this.types.Find(x => x == dir);
        if(index != null){
            return true;
        }
        return false;
    }

    public void Remove(string dir){
        var index = this.types.Find(x => x == dir);
        if(index != null){
            this.types.RemoveAt(this.types.IndexOf(dir));
        }
    }

    public void Remove(List<string> dirs){
        foreach(string dir in dirs){
            this.Remove(dir);
        }
    }

    public int NumberOf(){
        return this.types.Count;
    }

    public void Print(){
        foreach(string dir in this.types){
            Debug.Log(dir);
        }
    }

    public string OppositeDirection(string dir){
        switch(dir){
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

    public void AddOpposite(string dir){
        switch(dir){
            case "Right":
                types.Add("Left");
                break;
            case "Left":
                types.Add("Right");
                break;
            case "Up":
                types.Add("Down");
                break;
            case "Down":
                types.Add("Up");
                break;
        }
    }

    public string Random(){
        if(this.types.Count > 0){
            return this.types[UnityEngine.Random.Range(0, this.types.Count - 1)];
        }
        return "None";
    }

    public void AddRandom(int count){
        List<string> temp = new List<string>(new string[] {"Left", "Right", "Down", "Up"});
        // add exits to types in a random order
        for(int i = 0; i < count; i++){
            int index = UnityEngine.Random.Range(0, temp.Count);
            this.types.Add(temp[index]);
            temp.RemoveAt(index);
        }
    }
}
