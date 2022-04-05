using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

///This class is meant to be used inside prefabs which will combine it with a behavior
[RequireComponent(typeof(SpriteRenderer))]
public class Card : MonoBehaviour, IBelong
{
    private Player owner;
    /// Reference a scriptable object which contains the card data (baseHealth, baseAttack, description, etc...)
    [SerializeField] private CardData cardData;
    //This behavior must be attached to this GameObject (This prefab) //Will set to automatically gather later
    [SerializeField] private CardBehavior behavior;
    [SerializeField] private SpriteRenderer sr;

    public float currentAttack { get; internal set; }
    public float currentHealth { get; internal set; }
    public bool isBlocked { get; internal set; }
    private bool _isVisible;
    public bool isVisible

     {

             get { return _isVisible;}

             set { 
                 _isVisible = value;
                 if(_isVisible) sr.sprite = front;
                 else sr.sprite = back;
                 }  

     }  
    public int manaCost { get; internal set; }
    private Sprite back;
    private Sprite front;

    //Must override StartBeginDragEvent to trigger a OnCardDrag event


    ///<summary>Horrible piece of boilerplate code to get around unity's random awkening order</summary>
    private bool isSubscribed = false;

    ///<summary>Horrible piece of boilerplate code to get around unity's random awkening order</summary>
    private bool _tryToSubscribe(){
        if(isSubscribed) return false;
        if(EventManager.Instance == null) return false;
        isSubscribed = true;

        // ---Subscribe methods to events---
        EventManager.Instance.OnTurnOwnerChange += UpdateVisibility;
        EventManager.Instance.OnDraw += UpdateVisibility;
        EventManager.Instance.OnMove += UpdateVisibilityOnMove;
        return true;
    }
    ///<summary>Horrible piece of boilerplate code to get around unity's random awkening order</summary>
    private bool _tryToUnsubscribe(){
        if(EventManager.Instance == null) return false;
        isSubscribed = false;

        // ---Unsubscribe methods to events---
        EventManager.Instance.OnTurnOwnerChange -= UpdateVisibility;
        EventManager.Instance.OnDraw -= UpdateVisibility;
        EventManager.Instance.OnMove -= UpdateVisibilityOnMove;
        return true;
    }


    void Awake(){
        Debug.Log("Card :: "+gameObject.name+" :: Awake()");
        currentAttack = cardData.Attack;
        currentHealth = cardData.Health;
        manaCost = cardData.ManaCost;
        back = cardData.Back;
        front = cardData.Front;
        isBlocked = false;
        isVisible = false;
    }
    void OnEnable(){
        _tryToSubscribe();
        Debug.Log("Card :: "+gameObject.name+" :: OnEnable()");
    }
    internal void Start(){
        _tryToSubscribe();
        Debug.Log("Card :: "+gameObject.name+" :: Start()");
        //Init local variables based on cardData
        //Displays its information (updates text components, and sprites)
        behavior.print(cardData.Attack);
    }

    internal void OnDisable(){
        _tryToUnsubscribe();
    }

    public Player GetOwner()
    {
        return owner;
    }

    public Player BelongsTo(Player p)
    {
        Debug.Log(string.Format("{0} now belongs to {1}", gameObject.name, p.gameObject.name));
        return owner = p;
    }

    internal async Task<float> ApplyDMG(Card attacker, float attackValue)
    {
        return await behavior.Defend(this, attacker, attackValue);
    }

    internal async Task Attack(Field field, TurnOwner turnOwner, SlotType slotType, int columnIndex)
    {
        await behavior.Attack(this, field, turnOwner, slotType, columnIndex);
    }

    internal async Task Die(Card attacker)
    {
        await behavior.Die(this, attacker);
    }

    internal void UpdateVisibility(){
        if(GameManager.Instance.GetTurnPlayer() != owner){//not my turn
            CardSlot cs = transform?.parent.GetComponent<CardSlot>();
            if(cs == null)// if not on a card slot
                isVisible = false;
            else if(cs.type == SlotType.Placement || cs.type == SlotType.Hand) //if on placement or hand
                isVisible = false;
            else //if in battle pr preview
                isVisible = true;
        }
        else{//my turn
            Deck d = transform.parent?.GetComponent<Deck>();
            if(d != null)// if on a deck
                isVisible = false;
            else
                isVisible = true;
        }
    }
    internal void UpdateVisibilityOnMove(){
        /* if(transform.parent == null){
            Debug.Log("Card being dragged during turn change");
            isVisible = false;
            return;
        }
        if(transform.parent.GetComponent<CardSlot>()?.type == SlotType.Placement && GameManager.Instance.GetTurnPlayer() != owner){
            isVisible = false;
            return;
        } */
    }
}