using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandManager : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Camera mainCam;
    public int maxCapacity;
    public float cardHeight;
    public float cardSlotWidth;
    public float maxVisibleHeight;
    public HandPlaceholder placeholderPrefab;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        transform.position = mainCam.ScreenToWorldPoint(new Vector3(0, mainCam.scaledPixelHeight/2, 0));
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);

        EventManager.current.OnCardBeginDrag += CardBeginDragHandler;
    }
    public void CardBeginDragHandler(CardDragAndDrop dragComp)
    {
        //Triggered when one of its children begins being dragged
        /* if(dragComp.transform.IsChildOf(transform)){
            //Create and give away the placeholder object
            int originalSibilingIndex = dragComp.transform.GetSiblingIndex();
            CreatePlaceholder(dragComp);
            dragComp.placeholder.transform.SetSiblingIndex(originalSibilingIndex);
            dragComp.placeholder.transform.position = dragComp.transform.position;
        } */
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("OnPointerEnter");
        //Triggered when a object being dragged enters this area
        if(eventData.pointerDrag != null){
            CardDragAndDrop dragComp = eventData.pointerDrag.GetComponent<CardDragAndDrop>();
            //Create and give away the placeholder object
            CreatePlaceholder(dragComp);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //Triggered when a object being dragged exits this area
        if(eventData.pointerDrag != null){
            CardDragAndDrop dragComp = eventData.pointerDrag.GetComponent<CardDragAndDrop>();
            //Destroy the placeholder object
            DestroyPlaceholder(dragComp);
        }
    }


    public void OnDrop(PointerEventData eventData){
        //Check if the object can be dropped
        EventManager.current.StartDropOnHand(this, eventData.pointerDrag);

        CardDragAndDrop dragComp = eventData.pointerDrag.GetComponent<CardDragAndDrop>();
        //If it can, then replace the placeholder with the dropped object
        int originalSibilingIndex = dragComp.placeholder.transform.GetSiblingIndex();
        dragComp.transform.SetParent(transform);
        dragComp.transform.SetSiblingIndex(originalSibilingIndex);
        dragComp.transform.position = dragComp.placeholder.transform.position;
        DestroyPlaceholder(dragComp);
    }

    public void MoveChildUp(int childIndex){
        //Handle how a child object would be moved up (sibilingIndex--)
    }

    public void MoveChildDown(int childIndex){
        //Handle how a child object would be moved down (sibilingIndex++)
    }

    public void UpdateLayout(){
        //Handle how to layout its children (called when their number change)
    }

    public void CreatePlaceholder(CardDragAndDrop dragComp){
        if(dragComp == null) return;
        dragComp.placeholder = Instantiate(placeholderPrefab, transform);
    }

    private void DestroyPlaceholder(CardDragAndDrop dragComp)
    {
        if(dragComp == null) return;
        Destroy(dragComp.placeholder.gameObject);
    }
}
