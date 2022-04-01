using UnityEngine;
using UnityEngine.EventSystems;

public class Slot: ContainerBase, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public virtual void OnDrop(PointerEventData eventData)
    {
        Draggable dragComp = eventData.pointerDrag.GetComponent<Draggable>();
        if(dragComp == null){
            Debug.Log("Dropped object does not have a Draggable component.");
            return;
        }
        ValidateCanPlace(dragComp);
        if (canPlace)
            Place(dragComp); //Unity says OnDrop will always be triggered before OnEndDrag (If so, the placeholder will still exists during this call)
    }

    //Handle placeholder creation
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        //Triggered when a object being dragged enters this area and can be dropped
        if (eventData.pointerDrag != null)
        {
            Draggable dragComp = eventData.pointerDrag.GetComponent<Draggable>();
            if(dragComp == null){
                Debug.Log("Object being dragged does not have a Draggable component.");
                return;
            }
            ValidateCanPlace(dragComp);
            if (canPlace)
            {
                canPlace = false;
                //Create and give away the placeholder object
                CreatePlaceholder(dragComp);
            }
        }
    }

    //Handle placeholder deletion
    //Bug Found: Somehow the user can stop dragging and drop in another position after PointerEnter but before PointerExit
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        //Drop can happen somewhere else before this function trigger (then it needs to destroy the placeholder anyway)
        //Triggered when a object being dragged exits this area
        if (eventData.pointerDrag != null)
        {
            Draggable dragComp = eventData.pointerDrag.GetComponent<Draggable>();
            if(dragComp == null){
                Debug.Log("Object being dragged does not have a Draggable component.");
                return;
            }
            //Destroy the placeholder object (if it exists)
            DestroyPlaceholder(dragComp);
        }
    }

    internal override void CreatePlaceholder<T>(T tObj)
    {
        base.CreatePlaceholder(tObj);
        EventManager.Instance.OnEndDrag += DestroyPlaceholder;
    }

    internal override void DestroyPlaceholder<T>(T tObj)
    {
        base.DestroyPlaceholder(tObj);
        EventManager.Instance.OnEndDrag -= DestroyPlaceholder;
    }
}
