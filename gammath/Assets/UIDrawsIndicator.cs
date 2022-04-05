using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDrawsIndicator : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text TMP;
    [SerializeField] private TurnOwner player;

    // Update is called once per frame
    void Update()
    {
        TMP.text = GameManager.Instance.GetDrawsLeft(player).ToString();
    }
}
