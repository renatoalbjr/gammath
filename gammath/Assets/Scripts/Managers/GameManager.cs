using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Responsible for managing the game flow. The GameUI will trigger events, to which this class will be subscribed to (UnityEvents).
/// So, when loading a scene it will be responsible for populating the data and controlling.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Variables
    public static GameManager Instance { get; private set; }

    // ---Serialized variables---
    [SerializeField] private Player _playerOne;
    [SerializeField] private Player _playerTwo;
    [SerializeField] private Field _field;
    [SerializeField] private Hand _playerOneHand;
    [SerializeField] private Hand _playerTwoHand;
    [SerializeField] private Deck _playerOneDeck;
    [SerializeField] private Deck _playerTwoDeck;
    [SerializeField] private int _turnCounterLimit;
    [SerializeField] private int _turnTimeLimit;

    // ---Internal use variables---
    private GameState _gameState;
    private TurnOwner _turnOwner;
    private TurnStage _turnStage;
    private int _turnCounter;
    private int _score;
    private Stopwatch _turnStopwatch;
    private bool hasSurrended;
    #endregion

    // ########################################################################################## //

    #region EventManager Subscriber/Unsubscriber
    // ########################################################################################## //

    ///<summary>Horrible piece of boilerplate code to get around unity's random awkening order</summary>
    private bool isSubscribed = false;

    ///<summary>Horrible piece of boilerplate code to get around unity's random awkening order</summary>
    private bool _tryToSubscribe(){
        if(isSubscribed) return false;
        if(EventManager.Instance == null) return false;
        isSubscribed = true;

        // ---Subscribe methods to events---
        EventManager.Instance.OnBeginDrag += _beginDragHandler;
        EventManager.Instance.OnDropOnSlot += _dropOnSlotHandler;

        EventManager.Instance.OnGameStateChange += _gameStateChangeHandler;
        EventManager.Instance.OnTurnOwnerChange += _turnOwnerChangeHandler;
        EventManager.Instance.OnTurnStageChange += _turnStageChangeHandler;
        return true;
    }
    ///<summary>Horrible piece of boilerplate code to get around unity's random awkening order</summary>
    private bool _tryToUnsubscribe(){
        if(EventManager.Instance == null) return false;
        isSubscribed = false;

        // ---Unsubscribe methods to events---
        EventManager.Instance.OnBeginDrag -= _beginDragHandler;
        EventManager.Instance.OnDropOnSlot -= _dropOnSlotHandler;

        EventManager.Instance.OnGameStateChange -= _gameStateChangeHandler;
        EventManager.Instance.OnTurnOwnerChange -= _turnOwnerChangeHandler;
        EventManager.Instance.OnTurnStageChange -= _turnStageChangeHandler;
        return true;
    }
    #endregion
    
    #region Unity Methods
    #region Awake
    // ########################################################################################## //

    void Awake()
    {
        UnityEngine.Debug.Log("Game Object: "+gameObject.name);
        
        isSubscribed = false;
        
        // ---Singleton Implementation---
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        // ---Initialize intenal use variables---
        _gameState = GameState.Loading; //StarMenu
        _turnOwner = TurnOwner.None;
        _turnStage = TurnStage.None;
        _turnCounter = 1;
        _score = 0;
        _turnStopwatch = new Stopwatch();
        hasSurrended = false;

        // ---Set Players---
        _field.SetPlayers(_playerOne, _playerTwo);
        _playerOneHand.BelongsTo(_playerOne);
        _playerTwoHand.BelongsTo(_playerTwo);/* 
        _playerOneDeck.BelongsTo(_playerOne);
        _playerTwoDeck.BelongsTo(_playerTwo); */

    }
    #endregion

    #region OnEnable
    // ########################################################################################## //

    void OnEnable()
    {
        _tryToSubscribe();
        
        UnityEngine.Debug.Log("Game Object: "+gameObject.name);
    }
    #endregion

    #region OnDisable
    // ########################################################################################## //

    void OnDisable()
    {
        _tryToUnsubscribe();
    }
    #endregion

    #region Start
    // ########################################################################################## //

    void Start()
    {
        _tryToSubscribe();
        UnityEngine.Debug.Log("Game Object: "+gameObject.name);
        UnityEngine.Debug.Log("GameManager :: Start() :: The game state is now "+_gameState.ToString());
    }
    #endregion

    #region Update
    // ########################################################################################## //

    void Update()
    {
        if(_gameState == GameState.Loading){
            EventManager.Instance.StartGameStateChange();
        }
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

            /// Client side
            /// Before starting the battle, initialize its variables with the players
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
                if(CheckGameOverConditions()){
                    UnityEngine.Debug.Log("GameOver: The winner is "+_turnOwner.ToString());
                    EventManager.Instance.StartGameStateChange();
                }
                else{
                    //Lets treat player A surrender and player B win as one and the same
                    _turnOwner = TurnOwner.PlayerTwo;
                    UnityEngine.Debug.Log("Now is "+_turnOwner.ToString()+" turn");
                    if(hasSurrended){
                        UnityEngine.Debug.Log("The player "+TurnOwner.PlayerOne+" has surrendered");
                        UnityEngine.Debug.Log("GameOver: The winner is "+_turnOwner.ToString());
                        EventManager.Instance.StartGameStateChange();
                    }
                    else{
                        EventManager.Instance.StartTurnStageChange();
                    }
                }
                break;

            //After PlayerTwo
            case TurnOwner.PlayerTwo:
                UnityEngine.Debug.Log("The turn "+_turnCounter.ToString()+" have finished");
                _turnCounter++;
                if(CheckGameOverConditions()){
                    UnityEngine.Debug.Log("GameOver: The winner is "+_turnOwner.ToString());
                    EventManager.Instance.StartGameStateChange();
                }
                else{
                    _turnOwner = TurnOwner.PlayerOne;
                    UnityEngine.Debug.Log("Now is "+_turnOwner.ToString()+" turn");
                    if(hasSurrended){
                        UnityEngine.Debug.Log("The player "+TurnOwner.PlayerTwo+" has surrendered");
                        UnityEngine.Debug.Log("GameOver: The winner is "+_turnOwner.ToString());
                        EventManager.Instance.StartGameStateChange();
                    }
                    else{
                        EventManager.Instance.StartTurnStageChange();
                    }
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
    private async void _turnStageUpdater(){
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
                await Attack(_turnOwner);
                _turnStageUpdater();
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

    // ---Checkers---

    #region Check Objects Ownership
    // ########################################################################################## //

    //Compares a object Ownership component value with players and return true if they are the same
    public bool CheckOwnership(Draggable dragComp, Player player){
        //Check is the player is the owner of dragComp
        return CheckOwnership(dragComp?.gameObject.GetComponent<Card>(), player); //Making one overload head to another one as long as the parameter is not null
    }
    public bool CheckOwnership(Card cardComp, Player player){
        Player owner = cardComp?.GetOwner();
        UnityEngine.Debug.Log(owner.gameObject.name);

        // ---The object has no owner---
        //if(owner == null) return false; If the object has no owner, it means anyone can control it (other object in scene)

        //return owner == player;
        return CheckTurnOwner(owner); //Until online, assumes that the actioner is the turn owner, then compares it with the card owner
    }
    public bool CheckOwnership(Slot slot, Player player){
        Player owner = slot?.GetOwner();

        /*  In the future, if custom validators are needed for each derived class from a base class
            the most apropriated approach will be a series of GetComponents, one for each custom derived class.
            This GetComponents can either lead to other validators overloads, or can be check in the base class overload.
         */

        // ---The object has no owner---
        //if(owner == null) return false; If the object has no owner, it means anyone can control it (other object in scene)

        //return owner == player;
        return CheckTurnOwner(owner); //Until online, assumes that the actioner is the turn owner, then compares it with the card owner
    }
    // ########################################################################################## //
    #endregion

    #region Checks the turnOwner
    // ########################################################################################## //

    //A player can only play on it's turn
    public bool CheckTurnOwner(Player player){
        //Locally, a action is always triggered by the current turn player
        //Since we are not supporting multiple inputs
        //In Online mode, every call to the server will have to be marked by the client with its player
        //Then this method will check if the caller is the same as the current turn
        
        Player p = null;
        if(_turnOwner == TurnOwner.PlayerOne){
            p = _playerOne;
        }
        else if(_turnOwner == TurnOwner.PlayerTwo){
            p = _playerTwo;
        }

        return p == player;
    }
    // ########################################################################################## //
    #endregion

    #region Check conditions for the GameOver
    // ########################################################################################## //
    public bool CheckGameOverConditions(){
        if(_turnCounter <= _turnCounterLimit) return false;
        //Check the score
        //Check if the surrender button was pressed (the this will only be called next turn, meaning the next player will be the winner);
        return true;
    }
    // ########################################################################################## //
    #endregion

    // ---Validators---
    // ---Validates player actions---

    #region Validates a card drag
    // ########################################################################################## //
    private void _beginDragHandler(Draggable dragComp){
        UnityEngine.Debug.Log("Game Manager :: _beginCardDragHandler");
        if(dragComp){
            if(!dragComp.canDrag) return;
            // ---Drag will be allowed only during Battle state AND by the objects owner---
            if(_gameState != GameState.Battle || !CheckOwnership(dragComp, null)) dragComp.canDrag = false;
        }
    }
    // ########################################################################################## //
    #endregion

    #region Validates a card drop
    // ########################################################################################## //
    private void _dropOnSlotHandler(Slot slot, Draggable dragComp){
        // ---Drop will only be allowed at Battle state---
        // ---And during the objects owner turn---
        // ---The objects owner turn breaks down to ownership and turn---
        if(slot != null){
            if(!slot.canPlace) return;
            if(_gameState != GameState.Battle
               || !CheckOwnership(dragComp, null)
               || !CheckOwnership(slot, null)
               || CheckTurnOwner(null)
               ) 
               slot.canPlace = false;
            //Do slot.canPlace = Card.CanPlace(field, slot, dragComp.GetComponent<Card>());
        }
    }
    // ########################################################################################## //
    #endregion

    #region Validates a card draw
    // ########################################################################################## //
    // ########################################################################################## //
    #endregion

    #region Validates a click on the end turn button
    // ########################################################################################## //
    public static void EndTurnButtonHandler(){
        GameManager instance = GameManager.Instance;
        if(instance._turnStage == TurnStage.PlacingStage && instance._gameState == GameState.Battle)
            instance._turnStageChangeHandler();
    }
    // ########################################################################################## //
    #endregion

    #region Validates a click on the surrender button
    // ########################################################################################## //
    public static void SurrenderButtonHandler(/* Player p */){
        GameManager instance = GameManager.Instance;
        if(instance._gameState == GameState.Battle){
            instance.hasSurrended = true;
            // if(instance.CheckTurnOwner(p) || instance.GetTurnOwner() == TurnOwner.None){
                instance._turnStopwatch.Stop();
                instance._turnStopwatch.Reset();
                instance._turnOwnerChangeHandler();
            // }
            // else
            //     EndTurnButtonHandler();
        }
    }
    // ########################################################################################## //
    #endregion

    #region Validates OnGameStateChange event
    // ########################################################################################## //
    private void _gameStateChangeHandler(){
        _gameStateUpdater();
    }
    // ########################################################################################## //
    #endregion
    
    #region Validates OnTurnOwnerChange event
    // ########################################################################################## //
    private void _turnOwnerChangeHandler(){
        _turnOwnerUpdater();
    }
    // ########################################################################################## //
    #endregion

    #region Validates OnTurnStageChange event
    // ########################################################################################## //
    private void _turnStageChangeHandler(){
        _turnStageUpdater();
    }
    // ########################################################################################## //
    #endregion

    // --- Public Utilities---

    #region Returns the Turn Elapsed Time
    // ########################################################################################## //
    public System.TimeSpan GetTurnStageET(){
        return GameManager.Instance._turnStopwatch.Elapsed;
    }
    // ########################################################################################## //
    #endregion

    #region Return the Turn Time Left
    // ########################################################################################## //
    public System.TimeSpan GetTurnStageTL(){
        return System.TimeSpan.FromSeconds(_turnTimeLimit) - _turnStopwatch.Elapsed;
    }
    // ########################################################################################## //
    #endregion

    #region Return the Turn Owner
    // ########################################################################################## //
    public TurnOwner GetTurnOwner(){
        return GameManager.Instance._turnOwner;
    }
    // ########################################################################################## //
    #endregion

    #region Return the turn Time Limit
    // ########################################################################################## //
    public System.TimeSpan GetTurnTimeLimit(){
        return System.TimeSpan.FromSeconds(_turnTimeLimit);
    }
    // ########################################################################################## //
    #endregion

    #region Return the current turn
    // ########################################################################################## //
    public int GetTurnCounter(){
        return _turnCounter;
    }
    // ########################################################################################## //
    #endregion

    #region Attack
    // ########################################################################################## //
    /// <summary>
    /// Call the Card.Attack() on every card in the field in the proper order. This allows special placement attacks.
    /// </summary>
    private async Task Attack(TurnOwner attackingPlayer)
    {
        List<Card> attackers = new List<Card>();

        // ---Make the front column attack---
        await Attack(SlotType.FrontColumn, attackingPlayer);

        // ---Make the back column attack---
        await Attack(SlotType.BackColumn, attackingPlayer);

        // ---Make the Preview column attack---
        await Attack(SlotType.Preview, attackingPlayer);

        // ---Make the Placement column attack---
        await Attack(SlotType.Placement, attackingPlayer);

    }

    private async Task Attack(SlotType slotType, TurnOwner turnOwner)
    {
        Card[] attackers = _field.GetPlacedCards(slotType, turnOwner);
        for (int i = 0; i < attackers.Length; i++)
        {
            if(attackers[i] == null) continue;
            await attackers[i].Attack(_field, turnOwner, slotType, i);
        }

    }
    // ########################################################################################## //
    #endregion

    #region Score
    public void ScoreAgainst(float score, TurnOwner player){
        float newScore = player == TurnOwner.PlayerOne ? -score : score;
        _score += Mathf.FloorToInt(newScore);
        _score = Mathf.Clamp(_score, -6, 6);
    }
    public int GetScore(){
        return _score;
    }
    #endregion

}

#region GameState enum (Loading, Battle, GameOver)
// ########################################################################################## //

public enum GameState{
    Loading,
    Battle,
    GameOver,
    Enviroment,
    StarMenu
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