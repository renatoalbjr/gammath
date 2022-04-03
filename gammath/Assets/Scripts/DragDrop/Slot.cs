using UnityEngine;
using UnityEngine.EventSystems;

public class Slot: ContainerBase, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IBelong
{
    private Player owner;
    public Player BelongsTo(Player p)
    {
        Debug.Log(string.Format("{0} now belongs to {1}", gameObject.name, p.gameObject.name));
        return owner = p;
    }

    public Player GetOwner()
    {
        return owner;
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        Place(eventData.pointerDrag?.GetComponent<Draggable>());
    }

    //Handle placeholder creation
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        //Triggered when a object being dragged enters this area and can be dropped
        if (eventData.pointerDrag != null)
        {
            Draggable dragComp = eventData.pointerDrag.GetComponent<Draggable>();
            CreatePlaceholder(dragComp);
        }
    }

    //Handle placeholder deletion
    //Bug Found: Somehow the user can stop dragging and drop in another position after PointerEnter but before PointerExit
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        //Drop can happen somewhere else before this function trigger (then it needs to destroy the placeholder anyway)
        DestroyPlaceholder(eventData.pointerDrag?.GetComponent<Draggable>());
    }

    internal override void ValidateCanPlace<T>(T tObj)
    {
        base.ValidateCanPlace(tObj);
        EventManager.Instance.StartDropOnSlot(this, tObj?.GetComponent<Draggable>());
    }
}
