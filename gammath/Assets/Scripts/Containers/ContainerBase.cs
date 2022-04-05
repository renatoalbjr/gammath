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
        canPlace = true;
        filledCapacity = transform.childCount;
    }

    /// <summary>
    /// Instatiate the placeholder with the prefab and parent it to this slot. If there is no placeholder prefab it doesn't do nothing.
    /// </summary>
    /// <param name="tObj">
    /// A component that will be passed as a parameter to place validation and placeholder initialization
    /// </param>
    internal virtual bool CreatePlaceholder<T>(T tObj)
    where T : Component
    {
        // ---If theres no placeholder prefab, don't create it---
        if(placeholderPrefab == null) return false;
        
        // ---Check if can place, by default it can---
        ValidateCanPlace(tObj);

        if(!canPlace){
            // --- Resets canPlace---
            canPlace = true;
            return false;
        }
        canPlace = true;

        placeholder = Instantiate(placeholderPrefab,
                                  new Vector3(transform.position.x, transform.position.y, transform.position.z - 1),
                                  Quaternion.identity,
                                  transform);
        placeholder.Init(tObj);
        return true;                                  
    }

    /// <summary>
    /// Destroys the current placeholder
    /// </summary>
    /// <param name="tObj">
    /// A component that might be passed as a parameter to place validation and placeholder initialization
    /// </param>
    internal virtual void DestroyPlaceholder<T>(T tObj)
    where T : Component
    {
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
        // ---If there's nothing to place---
        if(tObj == null) return false;

        // ---Check if can place, by default it can---
        ValidateCanPlace(tObj);

        if(!canPlace){
            // --- Resets canPlace---
            canPlace = true;
            return false;
        }
        canPlace = true;

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
        return true;
    }

    internal virtual bool PlaceUnsafe<T>(T tObj)
    where T : Component
    {
        // ---If there's nothing to place---
        if(tObj == null) return false;

        // ---Check if can place, by default it can---
        //ValidateCanPlace(tObj);
        if(!canPlace){ }
        else
            if(filledCapacity + 1 > maxCapacity) canPlace = false;

        if(!canPlace){
            // --- Resets canPlace---
            canPlace = true;
            return false;
        }
        canPlace = true;

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

    ///<summary>
    ///Check the capacity and the object by default
    ///</summary>
    internal virtual void ValidateCanPlace<T>(T tObj)
    where T : Component
    {
        if(tObj == null) return;
        if(!canPlace) return;
        if(filledCapacity + 1 > maxCapacity) canPlace = false;
    }
}
