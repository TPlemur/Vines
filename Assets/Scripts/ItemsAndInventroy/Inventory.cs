using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public List<Item> items = new List<Item>();
    public Item equipped = null;
    private int current = -1;

    public Inventory(){
        Debug.Log("NEW INVENTORY");
    }

    public void Add(Item newItem){
        if(equipped != null){
            equipped.UnEquip();
        }
        items.Add(newItem);
        current  = items.Count - 1;
        equipped = items[current];
        EquipCurrent();
    }

    public void EquipCurrent(){
        if(items.Count != 0){
            equipped.Equip();
        }
    }

    public void UnEquipCurrent(){
        if(items.Count != 0){
            equipped.UnEquip();
        }
    }

    public void EquippedPrimary(){
        if(items.Count != 0){
            equipped.Primary();
        }
    }

    public void EquippedSecondary(){
        if(items.Count != 0){
            equipped.Secondary();
        }
    }

    public void CycleRight(){
        if(items.Count > 0){
            equipped.UnEquip();
            current += current == items.Count - 1 ? -1 * (items.Count - 1) : 1;
            equipped = items[current];
            equipped.Equip();
        }
    }

    public void CycleLeft(){
        if(items.Count > 0){
            equipped.UnEquip();
            current -= current == 0 ? -1 * (items.Count - 1) : 1;
            equipped = items[current];
            equipped.Equip();

        }
    }

    public GameObject GetEquippedGameObject(){
        Debug.Log("GETTING EQUIPPED GAMEOBJECT");
        return null;
    }

}
