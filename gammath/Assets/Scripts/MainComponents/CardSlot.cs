using System;
using UnityEngine;

public class CardSlot : Slot
{
    internal SlotType type;

    internal Card GetContent(int index)
    {
        if(filledCapacity <= index) return null;
        return transform.GetChild(index).GetComponent<Card>();
    }
}
