using UnityEngine;
using System;

/// <summary>
/// Holds declarations of events and the methods to invoke them
/// </summary>
/// <remarks>
/// <para>Events syntax: On[Event]</para>
/// <para>Invoke events syntax: Start[Event]()</para>
/// <para>All events are C# Actions</para>
/// </remarks>
public class EventManager : MonoBehaviour 
{
    #region Variables
    public static EventManager Instance;

    #region Player input events
    public event Action<Card> OnBeginCardDrag;
    public event Action<Draggable> OnEndDrag;
    public event Action<CardSlot, Draggable> OnDropOnCardSlot;
    public event Action<Hand, Draggable> OnDropOnHand;
    #endregion

    #region Game state events
    public event Action OnGameStateChange;
    public event Action OnTurnOwnerChange;
    public event Action OnTurnStageChange;
    public event Action OnGameOver;
    public event Action OnSceneUnload;
    public event Action OnAttack;
    #endregion
    #endregion

    // ########################################################################################## //

    #region Unity Methods
    #region Awake
    // ########################################################################################## //

    void Awake()
    {
        // ---Singleton Implementation---
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
    #endregion
    #endregion

    // ########################################################################################## //

    #region Invoke Methods
    #region Player events
    public void StartBeginCardDrag(Card dragComp){
        OnBeginCardDrag?.Invoke(dragComp);
    }
    public void StartEndDrag(Draggable dragComp){
        OnEndDrag?.Invoke(dragComp);
    }
    public void StartDropOnCardSlot(CardSlot cardSlot, Draggable draggableObject){
        OnDropOnCardSlot?.Invoke(cardSlot, draggableObject);
    }

    public void StartDropOnHand(Hand hand, Draggable draggableObject){
        OnDropOnHand?.Invoke(hand, draggableObject);
    }
    #endregion

    #region Game state events
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
    #endregion
    #endregion
}
