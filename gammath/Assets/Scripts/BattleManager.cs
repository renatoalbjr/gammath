using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    void Start()
    {
        EventManager.current.OnCardBeginDrag += CardBeginDragHandler;
        EventManager.current.OnDropOnSlot += DropOnSlotHandler;
    }

    private void OnDisable(){
        EventManager.current.OnCardBeginDrag -= CardBeginDragHandler;
        EventManager.current.OnDropOnSlot -= DropOnSlotHandler;
    }

    private void CardBeginDragHandler(CardDragAndDrop cardDrag){
        if(cardDrag){
            Transform cardParent = cardDrag.transform.parent;
            cardDrag.dragging = true;
            cardDrag.transform.SetParent(null);
        }
    }

    private void DropOnSlotHandler(CardSlot cardSlot, GameObject card){
        if(cardSlot != null){
            if(cardSlot.transform.childCount == 0)
                cardSlot.canDrop = true;
        }
    }
}
