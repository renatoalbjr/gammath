using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/* Should be used to create prefabs
*/

public abstract class PlaceholderBase : MonoBehaviour, IMoveHandler
{
    public virtual void OnMove(AxisEventData eventData){
        debugPrint(eventData);
    }

    internal virtual void debugPrint(AxisEventData eventData){
        Debug.Log(eventData.ToString());
    }
}
