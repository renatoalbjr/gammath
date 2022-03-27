using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragAndDrop : MonoBehaviour, IBeginDragHandler,IDragHandler, IEndDragHandler
{
    private Camera mainCam;
    private Vector3 offset;
    private Vector3 newPos;
    private Vector3 newPosOnScreen;

    private Collider2D myCollider;
    public bool dragging;

    void Awake(){
        myCollider = GetComponent<Collider2D>();

        mainCam = Camera.main;
        dragging = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        offset = GetMousePosition(eventData) - transform.position; 
        //dragging = true;
        myCollider.enabled = false;   

        //Trigger an event
        EventManager.current.StartCardBeginDrag(gameObject);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!dragging) return;

        newPos = GetMousePosition(eventData) - offset;
        newPosOnScreen = GetWorldPointInScreenPosition(newPos);
        if(
            newPosOnScreen.x > 0
            && newPosOnScreen.y > 0
            && newPosOnScreen.x < mainCam.scaledPixelWidth
            && newPosOnScreen.y < mainCam.scaledPixelHeight
        ){
            //newPos.z = transform.position.z;
            transform.position = newPos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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
