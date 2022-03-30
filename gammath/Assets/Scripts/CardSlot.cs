using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool canDrop;
    public bool isEmpty;

    //[SerializeField] private PlaceholderBase placeholderPrefab { get; set; }
    public PlaceholderBase placeholderPrefab;

    void Start(){
        canDrop = false;
        isEmpty = true;
    }

    //This is called before OnEndDrag
    public void OnDrop(PointerEventData eventData)
    {
        EventManager.current.StartDropOnSlot(this, eventData.pointerDrag);
        if(canDrop)
            PlaceCard(eventData.pointerDrag.GetComponent<CardDragAndDrop>());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("OnPointerEnter");
        //Triggered when a object being dragged enters this area
        if(eventData.pointerDrag != null && transform.childCount == 0){
            CardDragAndDrop dragComp = eventData.pointerDrag.GetComponent<CardDragAndDrop>();
            //Create and give away the placeholder object
            CreatePlaceholder(dragComp);
            isEmpty = true;
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

    public void CreatePlaceholder(CardDragAndDrop dragComp){
        if(dragComp == null) return;
        dragComp.placeholder = Instantiate(placeholderPrefab, transform);
    }

    private void DestroyPlaceholder(CardDragAndDrop dragComp)
    {
        if(dragComp == null) return;
        if(dragComp.placeholder == null) return;
        Destroy(dragComp.placeholder.gameObject);
    }

    public void PlaceCard(CardDragAndDrop dragComp){
        canDrop = false;
        isEmpty = false;

        dragComp.transform.SetParent(transform);
        dragComp.transform.position = transform.position;
        DestroyPlaceholder(dragComp);
    }
}
