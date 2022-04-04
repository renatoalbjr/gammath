using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnCollision : MonoBehaviour
{

    [SerializeField] private BoxCollider2D bx;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private PlayerMovement p;
    // Start is called before the first frame update
    void Start()
    {
        bx.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(bx.IsTouching(p.GetComponent<BoxCollider2D>())){
            sprite.enabled = false;
        }
        else
        {
            sprite.enabled = true;
        }
    }
}
