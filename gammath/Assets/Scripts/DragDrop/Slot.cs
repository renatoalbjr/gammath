using UnityEngine;
using UnityEngine.EventSystems;

public class Slot: ContainerBase, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public virtual void OnDrop(PointerEventData eventData)
    {
        Draggable dragComp = eventData.pointerDrag.GetComponent<Draggable>();
        
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
        DestroyPlaceholder(eventData.pointerDrag?.GetComponent<Draggable>());
    }
}
