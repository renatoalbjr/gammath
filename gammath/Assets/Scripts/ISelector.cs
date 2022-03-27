using UnityEngine;

internal interface ISelector
{
    public GameObject Check(Ray ray);

    public GameObject GetSelection();
}