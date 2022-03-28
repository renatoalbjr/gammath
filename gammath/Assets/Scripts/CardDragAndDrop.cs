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

    void Awake(){
        myCollider = GetComponent<Collider2D>();
        mainCam = Camera.main;
        dragging = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = transform.position;

        offset = GetMousePosition(eventData) - transform.position;
        myCollider.enabled = false;   

        EventManager.current.StartCardBeginDrag(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
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
        if(transform.parent == null){
            Debug.Log("OnEndDrag: "+ transform.parent == null ? transform.parent.ToString() : "null");
            transform.SetParent(originalParent);
            //transform.SetSiblingIndex(originalSiblingIndex);
            transform.position = originalPosition;
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
