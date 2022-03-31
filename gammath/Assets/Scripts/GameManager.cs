using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
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

    private void CardBeginDragHandler(DraggableCard dragComp){
        if(dragComp){
            dragComp.canDrag = true;
        }
    }

    private void DropOnCardSlotHandler(CardSlot cardSlot, Draggable dragComp){
        if(cardSlot != null){
            if(cardSlot.filledCapacity+1 <= cardSlot.maxCapacity)
                cardSlot.canPlace = true;
        }
    }
}
