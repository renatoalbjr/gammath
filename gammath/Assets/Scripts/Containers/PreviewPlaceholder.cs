using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PreviewPlaceholder : PlaceholderBase
{
    internal override void Init<T>(T dragComp)
    {
        // ---Try to get sprite renderer comp---
        SpriteRenderer sr = dragComp.GetComponent<SpriteRenderer>();
        if(sr == null) return;
        gameObject.GetComponent<SpriteRenderer>().sprite = sr.sprite;
    }
}
