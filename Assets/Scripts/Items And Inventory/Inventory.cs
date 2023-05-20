using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public List<Item> items;
    public Item equipped;
    private int current;

    public Inventory(){
        items = new List<Item>();
        equipped = null;
        current = -1;
    }

    // Add items to the inventory
    public void Add(Item newItem){
        // check if player doesnt have the item
        if (!Has(newItem)){
            UnEquipCurrent();
            items.Add(newItem);
            current  = items.Count - 1;
            equipped = items[current];
            EquipCurrent();
;        }
    }

    public bool Has(Item item){
        foreach(Item has in items){
            if(has.GetType() == item.GetType()){
                return true;
            }
        }
        return false;
    }

    public bool Has(System.Type type)
    {
        foreach (Item has in items)
        {
            if (has.GetType() == type)
            {
                return true;
            }
        }
        return false;
    }

    // Equip and UnEquip current items
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

    // Equipped item primary and secondary buttons
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

    // Cycle left and right between items
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

    // return equipped item gameobject
    public GameObject GetEquippedGameObject(){
        return equipped.itemObj;
    }

    // Helper function for determining what item is equipped and if its toggled
    public bool EquippedIsPVTM(){
        return equipped.GetType() == typeof(PVTM);
    }

    public bool EquippedIsFlashlight(){
        return equipped.GetType() == typeof(Flashlight);
    }

    public bool EquippedIsElectricalEquipment(){
        return equipped.GetType() == typeof(ElectricalEquipment);
    }

    public bool EquippedIsShield(){
        return equipped.GetType() == typeof(Shield);
    }
    
    public bool EquippedIsToggled(){
        return equipped.IsToggled();
    }

    public int getCurrentIndex()
    {
        return current;
    }

    public void setToZeroth()
    {
        UnEquipCurrent();
        current = 0;
        equipped = items[current];
        equipped.Equip();
    }

    public int getIndexOfType(System.Type item)
    {
        for(int i = 0; i < items.Count;i++)
        {
            if(items[i].GetType() == item) { return i; }
        }
        Debug.Log("getIndexOfType() Failed to find item");
        return -1;
    }

    public void setCurrent(int newCurrent)
    {
        equipped.UnEquip();
        current = newCurrent;
        equipped = items[current];
        equipped.Equip();
    }
}
