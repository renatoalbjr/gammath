using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragAndDrop : MonoBehaviour, IBeginDragHandler,IDragHandler, IEndDragHandler
{
    private Camera mainCam;
    private Collider2D myCollider;
    private Vector3 offset;
    private Transform originalParent;
    private Vector3 originalPosition;
    //private int originalSiblingIndex;
    public bool dragging;

    public PlaceholderBase placeholder;

    void Awake(){
        myCollider = GetComponent<Collider2D>();
        mainCam = Camera.main;
        dragging = false;
        //Debug.Log(mainCam.orthographicSize.ToString());
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        Debug.Log("OnBeginDrag: "+gameObject.name);
        //Cache information about its last valid transform
        originalParent = transform.parent;
        originalPosition = transform.position;

        //Get a offset for dragging and disables its own collider
        offset = GetMousePosition(eventData) - transform.position;
        myCollider.enabled = false;   

        //Trigger the event informing it started to being dragged
        EventManager.current.StartCardBeginDrag(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag: "+gameObject.name+".draggin = "+dragging.ToString());
        if(!dragging) return;

        Vector3 newPos = GetMousePosition(eventData) - offset;
        Vector3 newPosOnScreen = GetWorldPointInScreenPosition(newPos);
        if(
            newPosOnScreen.x > 0
            && newPosOnScreen.y > 0
            && newPosOnScreen.x < mainCam.scaledPixelWidth
            && newPosOnScreen.y < mainCam.scaledPixelHeight
        ){
            transform.position = newPos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag: "+gameObject.name);
        if(transform.parent == null){
            if(originalParent != null){
                CardSlot slot = originalParent.GetComponent<CardSlot>();
                if(slot != null)
                    slot.PlaceCard(this);
            }
            else{
                transform.position = originalPosition;
            }    
        }
        dragging = false;
        myCollider.enabled = true;
    }

    Vector3 GetMousePosition(PointerEventData eventData){
        return mainCam.ScreenToWorldPoint(eventData.position); 
    }

    Vector3 GetWorldPointInScreenPosition(Vector3 position){
        return mainCam.WorldToScreenPoint(position);
    }
}
