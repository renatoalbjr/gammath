using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Designed to contain the cards stats, working as a immutable reference for every game object that needs it
[CreateAssetMenu(fileName ="Card Data")]
public class CardData : ScriptableObject
{
    #region Variables
    [SerializeField] private int _attack;

    public int Attack { get => _attack; private set => _attack = value; }

    #endregion

    void Awake(){
        Debug.Log("Card Data :: Awake()");
    }

    void OnEnable(){

        Debug.Log("Card Data :: OnEnable()");
    }

    void Start()
    {
        Debug.Log("Card Data :: Start()");
        Debug.Log("Card Data :: Attack = "+Attack.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
