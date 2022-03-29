using UnityEngine;
using System;

public class EventManager : MonoBehaviour 
{
    public static EventManager current;
    public event Action<CardDragAndDrop> OnCardBeginDrag;
    public event Action<CardSlot, GameObject> OnDropOnSlot;

    private void Awake()
    {
        if(current == null)
            current = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void StartCardBeginDrag(CardDragAndDrop cardDrag){
        OnCardBeginDrag?.Invoke(cardDrag);
    }
    public void StartDropOnSlot(CardSlot cardSlot, GameObject card){
        OnDropOnSlot?.Invoke(cardSlot, card);
    }
}