using UnityEngine;
using System;
using System.Collections.Generic;

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
    public event Action<Deck> OnClickOnDeck;
    #endregion

    #region Game state events
    public event Action OnGameStateChange;
    public event Action OnTurnOwnerChange;
    public event Action OnTurnStageChange;
    public event Action OnGameOver;
    public event Action OnSceneUnload;
    #endregion

    #region Battle events
    public event Action<Card> OnAttackValidation;
    public event Action<Card, List<float>> OnGetAttackValue;
    public event Action<Card, Card, List<float>> OnDMGDealt;
    public event Action<Card, List<Card>> OnGetEnemiesToAttack;
    public event Action<Card, List<TurnOwner>> OnAttackPlayer;
    public event Action OnPlace;
    public event Action OnMove;
    public event Action OnDie;
    public event Action OnDraw;
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
    public void StartBeginDrag(Draggable dragComp) => OnBeginDrag?.Invoke(dragComp);
    public void StartDropOnSlot(Slot slot, Draggable draggable) => OnDropOnSlot?.Invoke(slot, draggable);
    public void StartClickOnDeck(Deck deck) => OnClickOnDeck?.Invoke(deck);
    #endregion

    #region Game state events
    public void StartGameStateChange() => OnGameStateChange?.Invoke();
    public void StartTurnOwnerChange() => OnTurnOwnerChange?.Invoke();
    public void StartTurnStageChange() => OnTurnStageChange?.Invoke();
    public void StartGameOver() => OnGameOver?.Invoke();
    public void StartSceneUnload() => OnSceneUnload?.Invoke();

    #region Battle events
    public void StartAttackValidation(Card attacker) => OnAttackValidation?.Invoke(attacker);
    public void StartGetAttackValue(Card attacker, List<float> attackValue) => OnGetAttackValue?.Invoke(attacker, attackValue);
    public void StartDMGDealt(Card defender, Card attacker, List<float> dmgDealt) => OnDMGDealt?.Invoke(defender, attacker, dmgDealt);
    public void StartGetEnemiesToAttack(Card attacker, List<Card> enemies) => OnGetEnemiesToAttack?.Invoke(attacker, enemies);
    public void StartOnAttackPlayer(Card attacker, List<TurnOwner> attackedPlayer) => OnAttackPlayer?.Invoke(attacker, attackedPlayer);
    #endregion

    #endregion
    #endregion
}
