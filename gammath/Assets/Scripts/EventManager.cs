using UnityEngine;
using System;

public class EventManager : MonoBehaviour 
{
    public static EventManager current;
    public event Action<DraggableCard> OnBeginCardDrag;
    public event Action<CardSlot, Draggable> OnDropOnCardSlot;
    public event Action<HandManager, Draggable> OnDropOnHand;

    private void Awake()
    {
        if(current == null)
            current = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void StartBeginCardDrag(DraggableCard dragComp){
        OnBeginCardDrag?.Invoke(dragComp);
    }
    public void StartDropOnCardSlot(CardSlot cardSlot, Draggable draggableObject){
        OnDropOnCardSlot?.Invoke(cardSlot, draggableObject);
    }

    public void StartDropOnHand(HandManager hand, Draggable draggableObject){
        OnDropOnHand?.Invoke(hand, draggableObject);
    }
}
