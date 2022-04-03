using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is meant to be used inside prefabs which will combine it with a behavior
[RequireComponent(typeof(SpriteRenderer))]
public class Card : MonoBehaviour, IBelong
{
    private Player owner;
    /// Reference a scriptable object which contains the card data (baseHealth, baseAttack, description, etc...)
    [SerializeField] private CardData cardData;
    //This behavior must be attached to this GameObject (This prefab) //Will set to automatically gather later
    [SerializeField] private CardBehavior behavior;

    //Must override StartBeginDragEvent to trigger a OnCardDrag event

    void Awake(){
        Debug.Log("Card :: "+gameObject.name+" :: Awake()");
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
}