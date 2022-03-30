using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class CardSlot : Slot
{
    internal override void StartDropOnSlotEvent(PointerEventData eventData)
    {
        EventManager.current.StartDropOnCardSlot(this, eventData.pointerDrag);
    }
}
