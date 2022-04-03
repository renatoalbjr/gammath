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
    internal static EventManager Instance { get; private set; }

    #region Player input events
    public event Action<Draggable> OnBeginDrag;
    public event Action<Slot, Draggable> OnDropOnSlot;
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
        Debug.Log("Game Object: "+gameObject.name);
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
    public void StartBeginDrag(Draggable dragComp){
        OnBeginDrag?.Invoke(dragComp);
    }
    public void StartDropOnSlot(Slot slot, Draggable draggable){
        OnDropOnSlot?.Invoke(slot, draggable);
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
