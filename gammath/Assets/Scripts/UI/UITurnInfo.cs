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
    [SerializeField] private Image leftArrow;
    [SerializeField] private Image rightArrow;
    [SerializeField] private Color enabledColor;
    [SerializeField] private Color disabledColor;

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
        // ---Enabled / PlayerOne---
        playerOneTimer.text = GameManager.Instance.GetTurnStageTL().ToString(@"mm\:ss");
        playerOneTimer.color = enabledColor;
        leftArrow.color = enabledColor;
        // ---Disabled / PlayerTwo---
        playerTwoTimer.text = GameManager.Instance.GetTurnTimeLimit().ToString(@"mm\:ss");
        playerTwoTimer.color = disabledColor;
        rightArrow.color = disabledColor;
    }
    private void PlayerTwoTurn(){
        // ---Enabled / PlayerTwo---
        playerTwoTimer.text = GameManager.Instance.GetTurnStageTL().ToString(@"mm\:ss");
        playerTwoTimer.color = enabledColor;
        rightArrow.color = enabledColor;
        // ---Disabled / PlayerOne---
        playerOneTimer.text = GameManager.Instance.GetTurnTimeLimit().ToString(@"mm\:ss");
        playerOneTimer.color = disabledColor;
        leftArrow.color = disabledColor;
    }
    private void NoneTurn(){
        // ---Disabled / PlayerOne---
        playerOneTimer.text = GameManager.Instance.GetTurnTimeLimit().ToString(@"mm\:ss");
        playerOneTimer.color = disabledColor;
        leftArrow.color = disabledColor;
        // ---Disabled / PlayerTwo---
        playerTwoTimer.text = GameManager.Instance.GetTurnTimeLimit().ToString(@"mm\:ss");
        playerTwoTimer.color = disabledColor;
        rightArrow.color = disabledColor;
    }
}
