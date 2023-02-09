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
            List<string> temp = new List<string>(new string[] {"Left", "Right", "Down", "Up"});
            // add exits to types in a random order
            for(int i = 0; i < 4; i++){
                int index = UnityEngine.Random.Range(0, temp.Count - 1);
                this.types.Add(temp[index]);
                temp.RemoveAt(index);
            }
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

    public bool Has(string dir){
        return this.types.Contains(dir);
    }

    public void Remove(string dir){
        if(this.types.Contains(dir)){
            this.types.RemoveAt(this.types.IndexOf(dir));
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

    public string Random(){
        if(this.types.Count > 0){
            return this.types[UnityEngine.Random.Range(0, this.types.Count - 1)];
        }
        return "None";
    }
}
