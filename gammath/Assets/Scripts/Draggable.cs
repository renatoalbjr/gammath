using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    internal Camera mainCam;
    internal Collider2D myCollider;
    internal Vector3 offset;
    internal Transform originalParent;
    internal Vector3 originalPosition;
    public bool dragging;

    void Awake(){
        myCollider = GetComponent<Collider2D>();
        mainCam = Camera.main;
        dragging = false;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        StartBeginDragEvent(eventData);
        if(!dragging) return;

        offset = GetMousePosition(eventData) - transform.position;
        originalParent = transform.parent;
        originalPosition = transform.position;

        if(transform.parent != null)
        {
            Slot slot = transform.parent.GetComponent<Slot>();
            if(slot != null){
                slot.RemoveDraggable(this);
            }
            else
            {
                transform.SetParent(null);
            }
        }

        myCollider.enabled = false;
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
            transform.position = newPos;
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if(transform.parent != null){
            dragging = false;
            myCollider.enabled = true;
            return;
        }
        
        Slot slot = originalParent?.gameObject?.GetComponent<Slot>();

        if(slot != null){
            slot.PlaceDraggable(this);
        }
        else{
            transform.position = originalPosition;
            transform.SetParent(originalParent);
        }
        
        dragging = false;
        myCollider.enabled = true;
    }

    internal virtual Vector3 GetMousePosition(PointerEventData eventData){
        return mainCam.ScreenToWorldPoint(eventData.position); 
    }

    internal virtual Vector3 GetWorldPointInScreenPosition(Vector3 position){
        return mainCam.WorldToScreenPoint(position);
    }

    //Set dragging to true by default
    internal virtual void StartBeginDragEvent(PointerEventData eventData){
        dragging = true;
    }
}
