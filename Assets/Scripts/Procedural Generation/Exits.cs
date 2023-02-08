using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exits{
    
    public List<string> types;

    public Exits(bool allExits){
        if(allExits){
            this.types = new List<string>();
            List<string> temp = new List<string>(new string[] {"Left", "Right", "Down", "Up"});
            for(int i = 0; i < 4; i++){
                int index = UnityEngine.Random.Range(0, temp.Count - 1);
                this.types.Add(temp[index]);
                temp.RemoveAt(index);
            }
        }
        else{
            this.types = new List<string>();
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

    // printExit() just prints a string based on the parameter dir
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
        return this.types[UnityEngine.Random.Range(0, this.types.Count - 1)];
    }
}
