using UnityEngine;

internal interface ISelectionResponse
{
    public void OnSelect(GameObject selection);    
    public void OnDeselect(GameObject selection);    
}