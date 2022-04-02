using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is the base of all card behaviors
//Provides a set of behavior that are used by the BattleManager injecting the appropriated values
public class CardBehavior : MonoBehaviour
{
    void Awake(){
        Debug.Log("Card Behavior :: "+gameObject.name+" :: Awake()");
    }
    void OnEnable(){
        Debug.Log("Card Behavior :: "+gameObject.name+" :: OnEnable()");
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Card Behavior :: "+gameObject.name+" :: Start()");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal virtual void print(int attack){
        Debug.Log("Card Behavior :: Attack = "+attack.ToString());
    }
}
