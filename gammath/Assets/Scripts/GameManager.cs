using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Variables
    public GameManager Instance;

    // ---Serialized variables---
    [SerializeField] private Player playerOne;
    [SerializeField] private Player playerTwo;
    [SerializeField] private int turnCounterLimit;
    [SerializeField] private int turnTimeLimit;

    // ---Internal use variables---
    private GameState gameState;
    private TurnOwner turnOwner;
    private TurnStage turnStage;
    private int turnCounter;
    private Stopwatch turnStopwatch;
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
        gameState = GameState.Loading;
        turnOwner = TurnOwner.None;
        turnStage = TurnStage.None;
        turnCounter = 1;
        turnStopwatch = new Stopwatch();
    }
    #endregion

    #region OnEnable
    // ########################################################################################## //

    void OnEnable()
    {
        // ---Subscribe methods to events---
        EventManager.Instance.OnBeginCardDrag += CardBeginDragHandler;
        EventManager.Instance.OnDropOnCardSlot += DropOnCardSlotHandler;

        EventManager.Instance.OnGameStateChange += GameStateUpdater;
        EventManager.Instance.OnTurnOwnerChange += TurnUpdater;
        EventManager.Instance.OnTurnStageChange += TurnStageUpdater;
    }
    #endregion

    #region OnDisable
    // ########################################################################################## //

    private void OnDisable()
    {
        // ---Unsubscribe methods to events---
        EventManager.Instance.OnBeginCardDrag -= CardBeginDragHandler;
        EventManager.Instance.OnDropOnCardSlot -= DropOnCardSlotHandler;

        EventManager.Instance.OnGameStateChange -= GameStateUpdater;
        EventManager.Instance.OnTurnOwnerChange -= TurnUpdater;
        EventManager.Instance.OnTurnStageChange -= TurnStageUpdater;
    }
    #endregion

    #region Start
    // ########################################################################################## //

    void Start()
    {
        UnityEngine.Debug.Log("GameManager :: Start() :: The game state is now "+gameState.ToString());
    }
    #endregion

    #region Update
    // ########################################################################################## //

    void Update()
    {
        if(turnStopwatch.Elapsed.Seconds >= turnTimeLimit)
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
    private void GameStateUpdater(){

        switch (gameState)
        {
            case GameState.Loading:
                gameState = GameState.Battle;
                UnityEngine.Debug.Log("The game state is now "+gameState.ToString());
                EventManager.Instance.StartTurnOwnerChange();
                break;

            case GameState.Battle:
                gameState = GameState.GameOver;
                UnityEngine.Debug.Log("The game state is now "+gameState.ToString());
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
    private void TurnUpdater(){
        switch (turnOwner)
        {
            //After Initializing
            case TurnOwner.None:
                turnOwner = TurnOwner.PlayerOne;
                UnityEngine.Debug.Log("Now is "+turnOwner.ToString()+" turn");
                EventManager.Instance.StartTurnStageChange();
                break;

            //After PlayerOne
            case TurnOwner.PlayerOne:
                if(checkGameOverConditions()){
                    UnityEngine.Debug.Log("GameOver: The winner is "+turnOwner.ToString());
                    EventManager.Instance.StartGameStateChange();
                }
                else{
                    turnOwner = TurnOwner.PlayerTwo;
                    UnityEngine.Debug.Log("Now is "+turnOwner.ToString()+" turn");
                    EventManager.Instance.StartTurnStageChange();
                }
                break;

            //After PlayerTwo
            case TurnOwner.PlayerTwo:
                UnityEngine.Debug.Log("The turn "+turnCounter.ToString()+" have finished");
                turnCounter++;
                if(checkGameOverConditions()){
                    UnityEngine.Debug.Log("GameOver: The winner is "+turnOwner.ToString());
                    EventManager.Instance.StartGameStateChange();
                }
                else{
                    turnOwner = TurnOwner.PlayerOne;
                    UnityEngine.Debug.Log("Now is "+turnOwner.ToString()+" turn");
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
    private void TurnStageUpdater(){
        //~And also triggered by itself~ (Wrong, it depends on other triggers that happens mostly during PlacingStage)

        switch (turnStage)
        {
            //After TurnChange
            case TurnStage.None:
                turnStage = TurnStage.PlacingStage;
                turnStopwatch.Start();
                UnityEngine.Debug.Log("The "+turnOwner.ToString()+" turn stage is now "+turnStage.ToString());
                break;

            //After PlacingStage
            case TurnStage.PlacingStage:
                //Validates the OnTurnStageChange trigger
                turnStage = TurnStage.AtackStage;
                turnStopwatch.Stop();
                turnStopwatch.Reset();
                UnityEngine.Debug.Log("The "+turnOwner.ToString()+" turn stage is now "+turnStage.ToString());
                EventManager.Instance.StartAtack();
                break;

            //After AttackStage
            case TurnStage.AtackStage:
                turnStage = TurnStage.None;
                UnityEngine.Debug.Log("The "+turnOwner.ToString()+" turn have finished");
                EventManager.Instance.StartTurnOwnerChange();
                break;
        }
    }
    #endregion

    #region Check Objects Ownership
    // ########################################################################################## //

    //A player can only take action on it's cards
    private bool checkOwnership(){
        return true;
    }
    #endregion

    #region Checks the turnOwner
    // ########################################################################################## //

    //A player can only play on it's turn
    private bool checkTurn(){
        //Locally, a action is always triggered by the current turn player
        //Since we are not supporting multiple inputs
        //In Online mode, every call to the server will have to be marked by the client with its player
        //Then this method will check if the caller is the same as the current turn
        return true;
    }
    #endregion

    #region Check conditions for the GameOver
    // ########################################################################################## //

    private bool checkGameOverConditions(){
        if(turnCounter > turnCounterLimit) return true;
        return false;
    }
    #endregion

    // ---Validators---

    #region Validates a card drag
    // ########################################################################################## //

    private void CardBeginDragHandler(DraggableCard dragComp){
        if(dragComp){
            dragComp.canDrag = true;
        }
    }
    #endregion

    #region Validates a card drop
    // ########################################################################################## //

    private void DropOnCardSlotHandler(CardSlot cardSlot, Draggable dragComp){
        if(cardSlot != null){
            if(cardSlot.filledCapacity+1 <= cardSlot.maxCapacity)
                cardSlot.canPlace = true;
        }
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