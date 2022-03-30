using UnityEngine;
using UnityEngine.EventSystems;

public class Slot: MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    /*  Base class for slots
        1. The slot should trigger a event when something is dropped on it (To get the drop validated)
        2. The slot can hold up to maxCapacity objects, and at any given moment it has filledCapacity
        3. The slot can handle children layout (with overrides in its derived classes)
        4. The slot will handle placeholder positioning through callbacks triggered by the placeholder OnPointerExit method
        5. By default it replaces the placeholder with the dropped object, if there is no placholder then it parent and centralize
    */

    internal bool canDrop;
    [SerializeField] internal int maxCapacity;
    internal int filledCapacity;
    [SerializeField] internal PlaceholderBase placeholderPrefab;
    internal PlaceholderBase placeholder;

    public virtual void Start(){
        canDrop = false;
        filledCapacity = 0;
    }

    //Trigger and event to get canDrop validation, then it places the object
    public virtual void OnDrop(PointerEventData eventData){
        StartDropOnSlotEvent(eventData);
        if(canDrop)
            PlaceDraggable(eventData.pointerDrag.GetComponent<Draggable>());
    }

    //Resets canDrop, increases filledCapacity, either replaces the placeholder or parent and centralize
    internal virtual void PlaceDraggable(Draggable dragComp)
    {
        canDrop = false;
        filledCapacity++;

        if(placeholder != null){
            int placeholderSibilingIndex = placeholder.transform.GetSiblingIndex();
            //dragComp.transform.SetParent(transform);
            dragComp.transform.parent = transform;
            dragComp.transform.SetSiblingIndex(placeholderSibilingIndex);

            dragComp.transform.position = placeholder.transform.position;
        }
        else{
            dragComp.transform.position = transform.position;
            dragComp.transform.SetParent(transform);
        }
        DestroyPlaceholder(dragComp);
    }

    internal virtual void RemoveDraggable(Draggable dragComp){
        if(dragComp.transform.IsChildOf(transform)){
            dragComp.transform.SetParent(null);
            filledCapacity--;
        }
    }

    //Handle placeholder creation
    public virtual void OnPointerEnter(PointerEventData eventData){
        //Triggered when a object being draggable object enters this area and can be dropped
        if (eventData.pointerDrag != null)
        {
            StartDropOnSlotEvent(eventData);
            if(canDrop){
                canDrop = false;
                Draggable dragComp = eventData.pointerDrag.GetComponent<Draggable>();
                //Create and give away the placeholder object
                CreatePlaceholder(dragComp);
            }
        }
    }

    //Handle placeholder deletion
    public virtual void OnPointerExit(PointerEventData eventData){
        //Triggered when a object being dragged exits this area
        if (eventData.pointerDrag != null)
        {
            Draggable dragComp = eventData.pointerDrag.GetComponent<Draggable>();
            //Destroy the placeholder object (if it exists)
            DestroyPlaceholder(dragComp);
        }
    }

    //Instatiate the placeholder with the prefab and parent it to this slot
    internal virtual void CreatePlaceholder(Draggable dragComp)
    {
        placeholder = Instantiate(placeholderPrefab, transform);
    }

    //Only destroy the placeholder object
    internal virtual void DestroyPlaceholder(Draggable dragComp)
    {
        if(placeholder == null) return;
        Destroy(placeholder.gameObject);
    }

    //Check the capacity by default
    internal virtual void StartDropOnSlotEvent(PointerEventData eventData){
        if(filledCapacity+1 <= maxCapacity) canDrop = true;
    }
}
