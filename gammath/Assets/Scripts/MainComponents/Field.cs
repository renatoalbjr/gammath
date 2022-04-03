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
    private CardSlot[,] slots;
    private Player[] players = new Player[2];
    private bool arePlayersSet = false;

    #endregion

    void Start(){
        slotSize = slotPrefab.GetComponent<BoxCollider2D>().size;
        CalculateOffset();
        GenerateAllSlots();
    }

    private void GenerateAllSlots()
    {
        slots = new CardSlot[fieldSize.y, fieldSize.x];
        for(int x = 0; x < fieldSize.x; x++)
            for(int y = 0; y < fieldSize.y; y++)
                slots[y, x] = GenerateSingleSlot(x, y);
    }

    private CardSlot GenerateSingleSlot(int x, int y){
        CardSlot newSlot = Instantiate(slotPrefab,new Vector3((x+0.5f)*slotSize.x, (y+0.5f)*slotSize.y, -1), Quaternion.identity);
        newSlot.transform.position = newSlot.transform.position+(Vector3) offset;
        newSlot.transform.SetParent(transform);
        if(x < fieldSize.x/2){
            newSlot.name = string.Format("{0}_{1}", (SlotType)x, y);
            newSlot.type = (SlotType)x;
        }
        else{
            newSlot.name = string.Format("{0}_{1}", (SlotType)(fieldSize.x-x-1), y);
            newSlot.type = (SlotType)(fieldSize.x-x-1);
        }

        newSlot.BelongsTo(players[Mathf.FloorToInt(2*x/fieldSize.x)]);

        return newSlot;
    }

    private void CalculateOffset(){
        offset = Vector2.Scale(Vector2.Scale((Vector2) fieldSize, slotSize), new Vector2(-0.5f, -0.5f));
    }

    public void SetPlayers(Player one, Player two){
        if(arePlayersSet) return;
        players[0] = one;
        players[1] = two;
        arePlayersSet = true;
    }
}