using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class UITurnInfo : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text playerOneTimer;
    [SerializeField] private TMPro.TMP_Text playerTwoTimer;
    [SerializeField] private TMPro.TMP_Text turnCounter;
    
    [Range(0f, 1f)]
    [SerializeField] private float disabledAlfa;

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.GetTurnOwner() == TurnOwner.PlayerOne)
            PlayerOneTurn();
        else if(GameManager.Instance.GetTurnOwner() == TurnOwner.PlayerTwo)
            PlayerTwoTurn();
        else
            NoneTurn();
        turnCounter.text = GameManager.Instance.GetTurnCounter().ToString();
    }

    private void PlayerOneTurn(){
        // ---Enabled---
        playerOneTimer.alpha = 1f;
        playerOneTimer.text = GameManager.Instance.GetTurnStageTL().ToString(@"mm\:ss");
        // ---Diabled---
        playerTwoTimer.text = GameManager.Instance.GetTurnTimeLimit().ToString(@"mm\:ss");
        playerTwoTimer.alpha = disabledAlfa;
    }
    private void PlayerTwoTurn(){
        // ---Enabled---
        playerTwoTimer.alpha = 1f;
        playerTwoTimer.text = GameManager.Instance.GetTurnStageTL().ToString(@"mm\:ss");
        // ---Diabled---
        playerOneTimer.text = GameManager.Instance.GetTurnTimeLimit().ToString(@"mm\:ss");
        playerOneTimer.alpha = disabledAlfa;
    }
    private void NoneTurn(){
        // ---Diabled---
        playerOneTimer.text = GameManager.Instance.GetTurnTimeLimit().ToString(@"mm\:ss");
        playerOneTimer.alpha = disabledAlfa;
        // ---Diabled---
        playerTwoTimer.text = GameManager.Instance.GetTurnTimeLimit().ToString(@"mm\:ss");
        playerTwoTimer.alpha = disabledAlfa;
    }
}
