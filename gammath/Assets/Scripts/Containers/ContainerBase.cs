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

    internal virtual void Start()
    {
        canPlace = false;
        filledCapacity = transform.childCount;
    }

    //Instatiate the placeholder with the prefab and parent it to this slot
    internal virtual bool CreatePlaceholder<T>(T tObj)
    where T : Component
    {
        //if(tObj == null) return;
        if (placeholderPrefab == null) return false;
        ValidateCanPlace(tObj);
        if(!canPlace) return false;
        placeholder = Instantiate(placeholderPrefab,
                                  new Vector3(transform.position.x, transform.position.y, transform.position.z - 1),
                                  Quaternion.identity,
                                  transform);
        placeholder.Init(tObj);
        return true;                                  
    }

    //Only destroy the placeholder object
    internal virtual void DestroyPlaceholder<T>(T tObj)
    where T : Component
    {
        //if(tObj == null) return;
        if (placeholder == null) return;
        Destroy(placeholder.gameObject);
    }

    /// <summary>
    /// Validates the object to be placed, then either replaces the placeholder or parent and centralize
    /// </summary>
    /// <param name="tObj">
    /// A component of the object to be placed
    /// </param>
    internal virtual bool Place<T>(T tObj)
    where T : Component
    {
        if(tObj == null) return false;
        ValidateCanPlace(tObj);
        if(!canPlace) return false;
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
        //DestroyPlaceholder(tObj);
        return true;
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
