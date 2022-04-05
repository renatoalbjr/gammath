using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Designed to contain the cards stats, working as a immutable reference for every game object that needs it
[CreateAssetMenu(fileName ="Card Data")]
public class CardData : ScriptableObject
{
    #region Variables
    [SerializeField] private int _attack;
    [SerializeField] private float _health;
    [SerializeField] private int _manaCost;
    [SerializeField] private Sprite _back;
    [SerializeField] private Sprite _front;

    public int Attack { get => _attack; private set => _attack = value; }
    public float Health { get => _health; private set => _health = value; }
    public int ManaCost { get => _manaCost; private set => _manaCost = value; }
    public Sprite Back { get => _back; private set => _back = value; }
    public Sprite Front { get => _front; private set => _front = value; }

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
