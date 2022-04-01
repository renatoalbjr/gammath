using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class CardSlot : Slot
{
    internal override void ValidateCanPlace<T>(T tObj)
    {
        EventManager.Instance.StartDropOnCardSlot(this, tObj.GetComponent<Draggable>());
    }
}