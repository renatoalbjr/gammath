using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    internal virtual async Task Attack(Card attacker, Field field, TurnOwner turnOwner, SlotType slotType, int columnIndex)
    {
        // ---Checks if the card can attack---
        if(!canAttack(attacker, field, turnOwner, slotType, columnIndex)) return;

        // ---Gets the attack value---
        float attack = GetAttackValue(attacker, field, turnOwner, slotType, columnIndex);

        // ---Await the card to GetEnemies and attack the player if possible---
        List<Card> enemies = await GetEnemies(attacker, field, turnOwner, slotType, columnIndex);

        // ---If there's no enemy to attack return---
        if(enemies.Count == 0) return;

        // ---Atacks all enemies---
        foreach(Card e in enemies){
            // ---Play attack animation---
            await Task.Delay(2000);

            // ---Play defend and die animations---
            float dmgDealt = await e.ApplyDMG(attacker, attack);
            
            Debug.Log("Card "+ gameObject.name
                    + " of "+ turnOwner.ToString()
                    + " at "+ slotType.ToString()+ "_"+ columnIndex.ToString()
                    + " attacked "+e.name+" at "+e.transform.position.ToString()
                    + " with "+ attack.ToString()+ "pt of attack"
                    + " dealing "+dmgDealt.ToString()+"pt of damage");
        }
    }

    internal async Task<float> Defend(Card card, Card attacker, float attackValue)
    {
        // ---Plays the defend animation---
        await Task.Delay(300);

        float dmgDealt = Mathf.Min(card.currentHealth, attackValue);

        card.currentHealth = Mathf.Max(card.currentHealth - attackValue, 0);
        Debug.Log("The card "+card.name+" took "+dmgDealt.ToString()+" and now has "+card.currentHealth.ToString());
        if(card.currentHealth <= 0){
            // ---Die---
            await card.Die(attacker);
        }
        return dmgDealt;
    }

    internal async Task Die(Card card, Card attacker)
    {
        // ---Move to cemetery animation---
        //GameManager.Instance.MoveToCemetery(card);
        await Task.Delay(100);
        Debug.Log("The card "+card.name+" have died");
    }

    // ---Validate the attack---
    internal virtual bool canAttack(Card attacker, Field field, TurnOwner turnOwner, SlotType slotType, int columnIndex){
        if(slotType != SlotType.FrontColumn) return false;
        return true;
    }

    // ---Get the attack value---
    internal virtual float GetAttackValue(Card attacker, Field field, TurnOwner turnOwner, SlotType slotType, int columnIndex){
        return attacker.currentAttack;
    }

    // ---Get the enemies to attack---
    internal virtual async Task<List<Card>> GetEnemies(Card attacker, Field field, TurnOwner turnOwner, SlotType slotType, int columnIndex){
        if(turnOwner == TurnOwner.None) return new List<Card>();

        TurnOwner enemy = turnOwner == TurnOwner.PlayerOne ? TurnOwner.PlayerTwo : TurnOwner.PlayerOne;
        Card card = field.GetPlacedCard(SlotType.FrontColumn, enemy, columnIndex);
        card = card != null ? card : field.GetPlacedCard(SlotType.BackColumn, enemy, columnIndex);

        if(card == null){
            await AttackPlayer(enemy);
            return new List<Card>();
        }
        
        List<Card> cl = new List<Card>();
        cl.Add(card);
        return cl;
    }
    // ---Attack player---
    // ---Might get subscribed to some events (as Attack)---
    internal virtual async Task AttackPlayer(TurnOwner enemy){
        Debug.Log("Card "+gameObject.name+" attack "+enemy.ToString());
        /// ---Player animaiton---
        /// ---Set score---
        await Task.Delay(1000);
    }
}
