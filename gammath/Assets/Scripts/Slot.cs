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
            Place(dragComp);
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
    public virtual void OnPointerExit(PointerEventData eventData)
    {
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
    
}
