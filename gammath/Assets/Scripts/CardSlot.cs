using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IDropHandler
{
    public bool canDrop;

    void Start(){
        canDrop = false;
    }

    //This is called before OnEndDrag
    public void OnDrop(PointerEventData eventData)
    {
        EventManager.current.StartDropOnSlot(this, eventData.pointerDrag);
        if(canDrop){
            GameObject card = eventData.pointerDrag;
            card.transform.position = transform.position;
            card.transform.SetParent(transform);
            
            canDrop = false;
        }
    }
}
