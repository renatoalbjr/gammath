using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class PlaceholderBase : MonoBehaviour
{
    internal abstract void Init<T>(T comp) where T : Component;
}
