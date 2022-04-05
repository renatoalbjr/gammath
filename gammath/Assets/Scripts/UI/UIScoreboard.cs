using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreboard : MonoBehaviour
{
    [SerializeField] private Image indicator;
    [SerializeField] private List<TMPro.TMP_Text> scores;
    private int lastScore = 0;
    private const int MANUAL_FIX = -5;

    void Start(){
        indicator.enabled = true;
        indicator.transform.position = scores[6].transform.position;
        scores[6].enabled = false;
    }

    void Update()
    {
        if(GameManager.Instance.GetScore() != lastScore){
            scores[lastScore+6].enabled = true;
            lastScore = GameManager.Instance.GetScore();
            //DOTween move indicator to right place
            indicator.transform.position = scores[lastScore+6].transform.position;
            if(lastScore >= 0) indicator.transform.position = new Vector3(indicator.transform.position.x + MANUAL_FIX, indicator.transform.position.y, indicator.transform.position.z);
            scores[lastScore+6].enabled = false;
        }
    }
}
