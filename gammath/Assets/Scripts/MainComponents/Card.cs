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

    public float currentAttack { get; internal set; }
    public float currentHealth { get; internal set; }
    public bool isBlocked { get; internal set; }

    //Must override StartBeginDragEvent to trigger a OnCardDrag event

    void Awake(){
        Debug.Log("Card :: "+gameObject.name+" :: Awake()");
        currentAttack = cardData.Attack;
        currentHealth = cardData.Health;
        isBlocked = false;
    }
    void OnEnable(){
        Debug.Log("Card :: "+gameObject.name+" :: OnEnable()");
    }
    internal void Start(){
        Debug.Log("Card :: "+gameObject.name+" :: Start()");
        //Init local variables based on cardData
        //Displays its information (updates text components, and sprites)
        behavior.print(cardData.Attack);
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
}