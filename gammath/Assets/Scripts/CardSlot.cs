using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SpriteRenderer sprite;

    private Color originalColor;

    void Awake(){
        sprite = GetComponent<SpriteRenderer>();
        if(sprite) originalColor = sprite.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        sprite.color = Color.green;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        sprite.color = originalColor;
    }
}
