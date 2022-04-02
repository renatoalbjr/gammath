using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is meant to be used inside prefabs which will combine it with a behavior
public class Card : MonoBehaviour
{
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
}