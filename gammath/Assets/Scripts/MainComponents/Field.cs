using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    #region  Variables
    [SerializeField] private Vector2Int fieldSize;
    [SerializeField] private CardSlot slotPrefab;
    private Vector2 slotSize;
    private Vector2 offset;
    private GameObject[,] slots; 

    #endregion

    void Awake()
    {
        slotSize = slotPrefab.GetComponent<BoxCollider2D>().size;
        CalculateOffset();
        GenerateAllSlots();
    }

    private void GenerateAllSlots()
    {
        slots = new GameObject[fieldSize.y, fieldSize.x];
        for(int x = 0; x < fieldSize.x; x++)
            for(int y = 0; y < fieldSize.y; y++)
                slots[y, x] = GenerateSingleSlot(x, y);
    }

    private GameObject GenerateSingleSlot(int x, int y){
        GameObject newSlot = Instantiate(slotPrefab.gameObject,new Vector3((x+0.5f)*slotSize.x, (y+0.5f)*slotSize.y, -1), Quaternion.identity);
        newSlot.transform.position = newSlot.transform.position+(Vector3) offset;
        newSlot.transform.SetParent(transform);
        if(x < fieldSize.x/2){
            newSlot.name = string.Format("{0}_{1}", (SlotType)x, y);
        }
        else{
            newSlot.name = string.Format("{0}_{1}", (SlotType)(fieldSize.x-x-1), y);
        }
        return newSlot;
    }

    private void CalculateOffset(){
        offset = Vector2.Scale(Vector2.Scale((Vector2) fieldSize, slotSize), new Vector2(-0.5f, -0.5f));
    }
}

public enum SlotType{
    Placement = 0,
    Preview = 1,
    BackColumn = 2,
    FrontColumn = 3
}