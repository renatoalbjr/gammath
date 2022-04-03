using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    internal Camera mainCam;
    internal Collider2D boxCollider;
    internal Vector3 offset;
    internal Transform originalParent;
    internal Vector3 originalPosition;
    public bool canDrag;
    public bool dragging;

    internal virtual void Awake(){
        boxCollider = GetComponent<Collider2D>();
        mainCam = Camera.main;
        canDrag = true;
        dragging = false;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        StartBeginDragEvents(eventData);
        if(!canDrag){
            canDrag = true;
            return;
        }

        offset = GetMousePosition(eventData) - transform.position;
        originalParent = transform.parent;
        originalPosition = transform.position;

        ContainerBase cBase = transform.parent?.GetComponent<ContainerBase>();
        if(cBase != null){
            cBase.Remove(this);
        }
        else
        {
            transform.parent = null;
        }

        dragging = true;
        boxCollider.enabled = false;
    }

    public virtual void OnDrag(PointerEventData eventData)
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
            transform.position = new Vector3(newPos.x, newPos.y, mainCam.transform.position.z+1);
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if(!dragging) return; //This is triggered whether the drag is valid or not


        dragging = false;
        boxCollider.enabled = true;
        
        // ---Checks if it was placed---
        if(transform.parent != null) return;

        // ---If the originalParent is a ContainerBase, place it back---
        // ---Else, reparent and reposition---
        
        ContainerBase cBase = originalParent?.gameObject.GetComponent<ContainerBase>();

        if(cBase != null){
            cBase.Place(this);
        }
        else{
            transform.SetParent(originalParent);
            transform.position = originalPosition;
        }

    }

    #region Utilities
    internal virtual Vector3 GetMousePosition(PointerEventData eventData){
        return mainCam.ScreenToWorldPoint(eventData.position); 
    }

    internal virtual Vector3 GetWorldPointInScreenPosition(Vector3 position){
        return mainCam.WorldToScreenPoint(position);
    }
    #endregion

    //Please don't change above this line (Alway check the box collider on scene editor)
    //Set dragging to true by default
    internal virtual void StartBeginDragEvents(PointerEventData eventData){
        EventManager.Instance.StartBeginDrag(this);
    }
}
