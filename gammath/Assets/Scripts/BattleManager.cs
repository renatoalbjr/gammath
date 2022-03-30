using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    void Start()
    {
        EventManager.current.OnBeginCardDrag += CardBeginDragHandler;
        EventManager.current.OnDropOnCardSlot += DropOnCardSlotHandler;
    }

    private void OnDisable(){
        EventManager.current.OnBeginCardDrag -= CardBeginDragHandler;
        EventManager.current.OnDropOnCardSlot -= DropOnCardSlotHandler;
    }

    private void CardBeginDragHandler(DraggableCard cardDrag){
        if(cardDrag){
            cardDrag.canDrag = true;
        }
    }

    private void DropOnCardSlotHandler(CardSlot cardSlot, GameObject card){
        if(cardSlot != null){
            if(cardSlot.filledCapacity+1 <= cardSlot.maxCapacity)
                cardSlot.canDrop = true;
        }
    }
}
