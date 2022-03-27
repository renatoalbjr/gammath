using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventManager.current.OnCardBeginDrag += CardBeginDragHandler;
    }

    private void OnDisable(){
        EventManager.current.OnCardBeginDrag -= CardBeginDragHandler;
    }

    //OnCardBeginDrag += CardBeginDragHandler
    //CardBeginDragHandler checks if the card is draggable and other stuff
    //CardBeginDragHandler(){}
    private void CardBeginDragHandler(GameObject card){
        CardDragAndDrop comp = card.GetComponent<CardDragAndDrop>();
        if(comp) comp.dragging = true;
    }
}
