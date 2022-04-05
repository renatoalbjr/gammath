using System;
using UnityEngine;

public class CardSlot : Slot
{

    internal override bool Place<T>(T tObj)
    {
        if(base.Place(tObj)){
            Card c = tObj.GetComponent<Card>();
            GameManager.Instance.ModifyCurrentMana(c.GetOwner(), -c.manaCost);
        }
        return false;
    }

    internal override void Remove<T>(T tObj)
    {
        int previousCapacity = filledCapacity;
        Card c = tObj?.GetComponent<Card>();
        base.Remove(tObj);
        if(filledCapacity < previousCapacity){
            if(c == null){
                Debug.Log("The object removed from "+name+" is not a card");
                return;
            }
            GameManager.Instance.ModifyCurrentMana(c.GetOwner(), 0+c.manaCost);
        }
    }

    /* public override void OnDrop(PointerEventData eventData)
    {
        int previousCapacity = filledCapacity;
        Card c = eventData.pointerDrag?.GetComponent<Card>();
        base.OnDrop(eventData);
        if(filledCapacity > previousCapacity){
            if(c == null){
                Debug.Log("The draggable object placed on "+name+" is not a card");
                return;
            }
            GameManager.Instance.ModifyCurrentMana(c.GetOwner(), 0+c.manaCost);
        }
    } */

    internal Card GetContent(int index)
    {
        if(filledCapacity <= index) return null;
        return transform.GetChild(index).GetComponent<Card>();
    }

    internal void RemoveWithouCost(Card c)
    {
        base.Remove(c);
    }
}
