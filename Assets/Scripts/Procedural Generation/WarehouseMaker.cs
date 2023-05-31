using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.AI;

public class WarehouseMaker : MonoBehaviour
{
    [SerializeField] private GameObject warehouseEmpty;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject monster;
    [SerializeField] private GameObject player;
    [SerializeField] private int startRow;
    [SerializeField] private int startCol;
    [SerializeField] private int columns;
    [SerializeField] private int rows;

    public List<RandomSFXPopulator> lightSFXGenerators;

    public Warehouse warehouse;

    public NavMeshSurface surface;

    void Start()
    {
        warehouse = new Warehouse(this.rows, this.columns, this.startRow, this.startCol, this.warehouseEmpty);
        warehouse.Generate();
        warehouse.PlacePlayerAndMosnter(player, playerCamera, monster);
        surface.BuildNavMesh();
        warehouse.TurnOffAllLights();
    }

    void Update(){
        if(GameStateManager.debug && Input.GetKeyDown(KeyCode.Minus)){
            foreach(Transform child in warehouseEmpty.transform){
                Destroy(child.gameObject);
            }
            warehouse.Regenerate();
            warehouse.PlacePlayerAndMosnter(player, playerCamera, monster);
            surface.BuildNavMesh();
            warehouse.TurnOffAllLights();
        }
    }
}
