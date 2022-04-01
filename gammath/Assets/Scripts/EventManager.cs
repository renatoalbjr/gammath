using UnityEngine;
using System;

public class EventManager : MonoBehaviour 
{
    public static EventManager Instance;
    public event Action<DraggableCard> OnBeginCardDrag;
    public event Action<CardSlot, Draggable> OnDropOnCardSlot;
    public event Action<HandManager, Draggable> OnDropOnHand;

    public event Action OnGameStateChange;
    public event Action OnTurnOwnerChange;
    public event Action OnTurnStageChange;
    public event Action OnGameOver;
    public event Action OnSceneUnload;
    public event Action OnAttack;

    //Undestanding how those actions are invoked
    //public event Action LoopTester;

    void Awake()
    {
        if(Instance == null)
            Instance = this;
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

    public void StartGameStateChange(){
        OnGameStateChange?.Invoke();
    }

    public void StartTurnOwnerChange(){
        OnTurnOwnerChange?.Invoke();
    }

    public void StartTurnStageChange(){
        OnTurnStageChange?.Invoke();
    }

    public void StartGameOver(){
        OnGameOver?.Invoke();
    }

    public void StartSceneUnload(){
        OnSceneUnload?.Invoke();
    }

    public void StartAtack(){
        OnAttack?.Invoke();
    }
}
