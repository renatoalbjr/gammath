using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Variables
    public static GameManager Instance { get; private set; }

    // ---Serialized variables---
    [SerializeField] private Player _playerOne;
    [SerializeField] private Player _playerTwo;
    [SerializeField] private int _turnCounterLimit;
    [SerializeField] private int _turnTimeLimit;

    // ---Internal use variables---
    private GameState _gameState;
    private TurnOwner _turnOwner;
    private TurnStage _turnStage;
    private int _turnCounter;
    private Stopwatch _turnStopwatch;
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

        // ---Initialize intenal use variables---
        _gameState = GameState.Loading;
        _turnOwner = TurnOwner.None;
        _turnStage = TurnStage.None;
        _turnCounter = 1;
        _turnStopwatch = new Stopwatch();
    }
    #endregion

    #region OnEnable
    // ########################################################################################## //

    void OnEnable()
    {
        // ---Subscribe methods to events---
        EventManager.Instance.OnBeginCardDrag += _cardBeginDragHandler;
        EventManager.Instance.OnDropOnCardSlot += _dropOnCardSlotHandler;

        EventManager.Instance.OnGameStateChange += _gameStateChangeHandler;
        EventManager.Instance.OnTurnOwnerChange += _turnOwnerChangeHandler;
        EventManager.Instance.OnTurnStageChange += _turnStageChangeHandler;
    }
    #endregion

    #region OnDisable
    // ########################################################################################## //

    void OnDisable()
    {
        // ---Unsubscribe methods to events---
        EventManager.Instance.OnBeginCardDrag -= _cardBeginDragHandler;
        EventManager.Instance.OnDropOnCardSlot -= _dropOnCardSlotHandler;

        EventManager.Instance.OnGameStateChange -= _gameStateUpdater;
        EventManager.Instance.OnTurnOwnerChange -= _turnOwnerUpdater;
        EventManager.Instance.OnTurnStageChange -= _turnStageUpdater;
    }
    #endregion

    #region Start
    // ########################################################################################## //

    void Start()
    {
        UnityEngine.Debug.Log("GameManager :: Start() :: The game state is now "+_gameState.ToString());
    }
    #endregion

    #region Update
    // ########################################################################################## //

    void Update()
    {
        if(_turnStopwatch.Elapsed.Seconds >= _turnTimeLimit)
            EventManager.Instance.StartTurnStageChange();
    }
    #endregion
    #endregion

    #region GameStateUpdater
    // ########################################################################################## //

    /// <summary>
    /// Subscribed to OnGameStateChange.
    /// Decides the next gameState based on the current one.
    /// </summary>
    /// <remarks>
    /// If it's Loading, updates to Battle.
    /// If it's Battle updates to End.
    /// Loading is the default state, and should end after SceneManager triggers OnGameStateChange for the first time.
    /// The Battle state starts just after Loading, and should end after TurnUpdater triggers OnGameStateChange for the first time.
    /// After End starts, it should trigger OnEndGame that should handle the control back to SceneManager
    /// </remarks>
    private void _gameStateUpdater(){

        switch (_gameState)
        {
            case GameState.Loading:
                _gameState = GameState.Battle;
                UnityEngine.Debug.Log("The game state is now "+_gameState.ToString());
                EventManager.Instance.StartTurnOwnerChange();
                break;

            case GameState.Battle:
                _gameState = GameState.GameOver;
                UnityEngine.Debug.Log("The game state is now "+_gameState.ToString());
                EventManager.Instance.StartGameOver();
                break;

            case GameState.GameOver:
                UnityEngine.Debug.Log("GameManager :: Starting scene unloading...");
                EventManager.Instance.StartSceneUnload();
                break;
        }
    }
    #endregion

    #region TurnUpdater
    // ########################################################################################## //

    /// <summary>
    /// Subscribed to OnTurnChange.
    /// Decides the next turn based on the current one.
    /// </summary>
    /// <remarks>
    /// If it's None, updates to PlayerOne.
    /// If it's PlayerOne, updates to PlayerTwo and vice versa.
    /// Triggers OnTurnStageChange whenever updated to PlayerOne or PlayerTwo.
    /// On every call, calls CheckEndGameConditions
    /// If it returns true, then trigger OnGameStateChange, and updates to None.
	/// </remarks>
    private void _turnOwnerUpdater(){
        switch (_turnOwner)
        {
            //After Initializing
            case TurnOwner.None:
                _turnOwner = TurnOwner.PlayerOne;
                UnityEngine.Debug.Log("Now is "+_turnOwner.ToString()+" turn");
                EventManager.Instance.StartTurnStageChange();
                break;

            //After PlayerOne
            case TurnOwner.PlayerOne:
                if(_checkGameOverConditions()){
                    UnityEngine.Debug.Log("GameOver: The winner is "+_turnOwner.ToString());
                    EventManager.Instance.StartGameStateChange();
                }
                else{
                    _turnOwner = TurnOwner.PlayerTwo;
                    UnityEngine.Debug.Log("Now is "+_turnOwner.ToString()+" turn");
                    EventManager.Instance.StartTurnStageChange();
                }
                break;

            //After PlayerTwo
            case TurnOwner.PlayerTwo:
                UnityEngine.Debug.Log("The turn "+_turnCounter.ToString()+" have finished");
                _turnCounter++;
                if(_checkGameOverConditions()){
                    UnityEngine.Debug.Log("GameOver: The winner is "+_turnOwner.ToString());
                    EventManager.Instance.StartGameStateChange();
                }
                else{
                    _turnOwner = TurnOwner.PlayerOne;
                    UnityEngine.Debug.Log("Now is "+_turnOwner.ToString()+" turn");
                    EventManager.Instance.StartTurnStageChange();
                }
                break;
        }
    }
    #endregion

    #region TurnStageUpdater
    // ########################################################################################## //

    /// <summary>
    /// Subscribed to OnTurnStageChange.
	/// Decides the next turnState based on the current one.
	/// </summary>
	/// <remarks>
    /// If it's None, updates to PlacingStage
    /// If it's PlacingStage, updates to AttackStage and triggers OnAttack
    /// If it's AttackStage, triggers OnTurnChange and updates to None
    /// Triggered by TurnUpdater, when a player turn starts.
    /// Also triggered by a counter that checks for time limits on every Update (During PlacingStage)
    /// And triggered by the End Turn button on UI (During PlacingStage)
    /// Those triggers on PlacingStage are enabled with a validator inside the PlacingStage case
	/// </remarks>
    private void _turnStageUpdater(){
        //~And also triggered by itself~ (Wrong, it depends on other triggers that happens mostly during PlacingStage)

        switch (_turnStage)
        {
            //After TurnChange
            case TurnStage.None:
                _turnStage = TurnStage.PlacingStage;
                _turnStopwatch.Start();
                UnityEngine.Debug.Log("The "+_turnOwner.ToString()+" turn stage is now "+_turnStage.ToString());
                break;

            //After PlacingStage
            case TurnStage.PlacingStage:
                //Validates the OnTurnStageChange trigger
                _turnStage = TurnStage.AtackStage;
                _turnStopwatch.Stop();
                _turnStopwatch.Reset();
                UnityEngine.Debug.Log("The "+_turnOwner.ToString()+" turn stage is now "+_turnStage.ToString());
                EventManager.Instance.StartAtack();
                break;

            //After AttackStage
            case TurnStage.AtackStage:
                _turnStage = TurnStage.None;
                UnityEngine.Debug.Log("The "+_turnOwner.ToString()+" turn have finished");
                EventManager.Instance.StartTurnOwnerChange();
                break;
        }
    }
    #endregion

    #region Check Objects Ownership
    // ########################################################################################## //

    //A player can only take action on it's cards
    private bool _checkOwnership(){
        return true;
    }
    #endregion

    #region Checks the turnOwner
    // ########################################################################################## //

    //A player can only play on it's turn
    private bool _checkTurn(){
        //Locally, a action is always triggered by the current turn player
        //Since we are not supporting multiple inputs
        //In Online mode, every call to the server will have to be marked by the client with its player
        //Then this method will check if the caller is the same as the current turn
        return true;
    }
    #endregion

    #region Check conditions for the GameOver
    // ########################################################################################## //

    private bool _checkGameOverConditions(){
        if(_turnCounter > _turnCounterLimit) return true;
        return false;
    }
    #endregion

    // ---Validators---

    #region Validates a card drag
    // ########################################################################################## //

    private void _cardBeginDragHandler(Card dragComp){
        if(dragComp){
            dragComp.canDrag = true;
        }
    }
    #endregion

    #region Validates a card drop
    // ########################################################################################## //

    private void _dropOnCardSlotHandler(CardSlot cardSlot, Draggable dragComp){
        if(cardSlot != null){
            if(cardSlot.filledCapacity+1 <= cardSlot.maxCapacity)
                cardSlot.canPlace = true;
        }
    }
    #endregion

    #region Validates OnGameStateChange event
    // ########################################################################################## //

    private void _gameStateChangeHandler(){
        _gameStateUpdater();
    }
    #endregion
    
    #region Validates OnTurnOwnerChange event
    // ########################################################################################## //

    private void _turnOwnerChangeHandler(){
        _turnOwnerUpdater();
    }
    #endregion

    #region Validates OnturnStageChange event
    // ########################################################################################## //

    private void _turnStageChangeHandler(){
        _turnStageUpdater();
    }
    #endregion

}

#region GameState enum (Loading, Battle, GameOver)
// ########################################################################################## //

public enum GameState{
    Loading,
    Battle,
    GameOver
}
#endregion

#region TurnOwner enum (None, PlayerOne, PlayerTwo)
// ########################################################################################## //

public enum TurnOwner{
    None,
    PlayerOne,
    PlayerTwo
}
#endregion

#region TurnStage enum (None, PlacingStage, AttackStage)
// ########################################################################################## //

public enum TurnStage{
    None,
    PlacingStage,
    AtackStage
}
#endregion