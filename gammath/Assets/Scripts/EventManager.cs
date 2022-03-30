using UnityEngine;
using System;

public class EventManager : MonoBehaviour 
{
    public static EventManager current;
    public event Action<DraggableCard> OnBeginCardDrag;
    public event Action<CardSlot, GameObject> OnDropOnCardSlot;
    public event Action<HandManager, GameObject> OnDropOnHand;

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
    public void StartDropOnCardSlot(CardSlot cardSlot, GameObject draggableObject){
        OnDropOnCardSlot?.Invoke(cardSlot, draggableObject);
    }

    public void StartDropOnHand(HandManager hand, GameObject draggableObject){
        OnDropOnHand?.Invoke(hand, draggableObject);
    }
}
