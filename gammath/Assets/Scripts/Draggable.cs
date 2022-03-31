using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    internal Camera mainCam;
    internal Collider2D myCollider;
    internal Vector3 offset;
    internal Transform originalParent;
    internal Vector3 originalPosition;
    public bool canDrag;

    void Awake(){
        myCollider = GetComponent<Collider2D>();
        mainCam = Camera.main;
        canDrag = false;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        StartBeginDragEvent(eventData);
        if(!canDrag) return;

        offset = GetMousePosition(eventData) - transform.position;
        originalParent = transform.parent;
        originalPosition = transform.position;

        if(transform.parent != null)
        {
            Slot slot = transform.parent.GetComponent<Slot>();
            if(slot != null){
                slot.Remove(this);
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
        if(!canDrag) return;

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
        if(transform.parent != null){
            canDrag = false;
            myCollider.enabled = true;
            return;
        }
        
        Slot slot = originalParent?.gameObject?.GetComponent<Slot>();

        if(slot != null){
            slot.Place(this);
        }
        else{
            transform.position = originalPosition;
            transform.SetParent(originalParent);
        }
        
        canDrag = false;
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
        canDrag = true;
    }
}
