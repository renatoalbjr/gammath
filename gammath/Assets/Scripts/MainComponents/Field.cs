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
        slots = new CardSlot[fieldSize.x, fieldSize.y];
        for(int x = 0; x < fieldSize.x; x++)
            for(int y = 0; y < fieldSize.y; y++)
                slots[x, y] = GenerateSingleSlot(x, y);
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

    internal Card[] GetPlacedCards(SlotType slotType, TurnOwner turnOwner)
    {
        Card[] cards = new Card[fieldSize.y];
        int x = turnOwner == TurnOwner.PlayerOne ? (int) slotType : 7-(int) slotType;
        for(int y = 0; y < fieldSize.y; y++)
            cards[y] = (slots[x, y].GetContent(0));
        return cards;
    }

    internal Card GetPlacedCard(SlotType slotType, TurnOwner turnOwner, int columnIndex)
    {
        int x = turnOwner == TurnOwner.PlayerOne ? (int) slotType : 7-(int) slotType;
        return slots[x, columnIndex].GetContent(0);
    }

    internal void AdvanceCards(TurnOwner turnOwner, SlotType slotType)
    {
        if(slotType == SlotType.FrontColumn){
            Debug.Log("Cantt advance card in "+slotType.ToString());
            return;
        }

        // ---Get the index of the columns---
        SlotType destinationType = slotType + 1;
        int originX = turnOwner == TurnOwner.PlayerOne ? (int) slotType : 7-(int) slotType;
        int destinationX = turnOwner == TurnOwner.PlayerOne ? (int) destinationType : 7-(int) destinationType;

        // ---Move cards---
        for(int y = 0; y < fieldSize.y; y++){
            // ---Continues if theres a card at destination---
            if(slots[destinationX, y].GetContent(0) != null){
                Debug.Log("Can't advance "+turnOwner.ToString()+"'s"+"card in "+slotType.ToString()+"_"+y+" because there's a card in front");
                continue;
            }

            // ---Get the card in the origin---
            Card c = slots[originX, y].GetContent(0);

            // ---Continues if theres no card at origin---
            if(c == null){
                Debug.Log("Can't advance "+turnOwner.ToString()+"'s"+"card in "+slotType.ToString()+"_"+y+" because the slot is empty");
                continue;
            }

            //slots[originX, y].Remove(c);
            slots[originX, y].RemoveWithouCost(c);
            slots[destinationX, y].PlaceUnsafe(c);
            Debug.Log(turnOwner.ToString()+"'s"+"card in "+slotType.ToString()+"_"+y+" moved forward");
        }
    }
}