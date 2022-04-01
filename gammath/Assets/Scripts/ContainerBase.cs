using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContainerBase : MonoBehaviour
{
    internal bool canPlace;
    [SerializeField] internal int maxCapacity;
    internal int filledCapacity;

    //If null don't create placeholders
    [SerializeField] internal PlaceholderBase placeholderPrefab;
    internal PlaceholderBase placeholder;

    public virtual void Start()
    {
        canPlace = false;
        filledCapacity = transform.childCount;
    }

    //Instatiate the placeholder with the prefab and parent it to this slot
    internal virtual void CreatePlaceholder<T>(T tObj)
    where T : Component
    {
        //if(tObj == null) return;
        if (placeholderPrefab == null) return;
        placeholder = Instantiate(placeholderPrefab,
                                  new Vector3(transform.position.x, transform.position.y, transform.position.z - 1),
                                  Quaternion.identity,
                                  transform);
    }

    //Only destroy the placeholder object
    internal virtual void DestroyPlaceholder<T>(T tObj)
    where T : Component
    {
        //if(tObj == null) return;
        if (placeholder == null) return;
        Destroy(placeholder.gameObject);
    }

    //Resets canPlace, increases filledCapacity, either replaces the placeholder or parent and centralize
    internal virtual void Place<T>(T tObj)
    where T : Component
    {
        if(tObj == null) return;
        canPlace = false;
        filledCapacity++;

        if (placeholder != null)
        {
            int placeholderSibilingIndex = placeholder.transform.GetSiblingIndex();
            tObj.transform.parent = transform;
            tObj.transform.SetSiblingIndex(placeholderSibilingIndex);

            tObj.transform.position = placeholder.transform.position;
        }
        else
        {
            tObj.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
            tObj.transform.SetParent(transform);
        }
        DestroyPlaceholder(tObj);
    }

    internal virtual void Remove<T>(T tObj)
    where T : Component
    {
        if(tObj == null) return;
        if (tObj.transform.IsChildOf(transform))
        {
            tObj.transform.SetParent(null);
            filledCapacity--;
        }
    
    }

    //Check the capacity and the object by default
    internal virtual void ValidateCanPlace<T>(T tObj)
    where T : Component
    {
        if (tObj == null) return;
        if (filledCapacity + 1 <= maxCapacity) canPlace = true;
    }
}
