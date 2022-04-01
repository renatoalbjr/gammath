using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hand : Slot
{
    private Camera mainCam;

    public Vector2 cardDimensions;
    public Vector2 maxVisibleCardDimensions;
    public Vector2 screenRelativePosition;
    public Vector2 handSize;
    public Vector2 handOffset;

    //x = top, y = right, z = bottom, w = left
    public Vector4 boxColliderPadding;

    internal Vector2 cSize;
    internal Vector2 cVisible;
    internal Vector2 sRelPos;
    internal Vector4 bcPadding;
    internal BoxCollider2D myCollider;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        mainCam = Camera.main;
        myCollider = GetComponent<BoxCollider2D>();

        cSize = cardDimensions;
        cVisible = maxVisibleCardDimensions;
        sRelPos = screenRelativePosition;
        bcPadding = boxColliderPadding;

        if(sRelPos.x > 1) sRelPos.x = 1;
        if(sRelPos.y > 1) sRelPos.y = 1;
        if(sRelPos.x < 0) sRelPos.x = 0;
        if(sRelPos.y < 0) sRelPos.y = 0;

        myCollider.size = handSize + new Vector2(bcPadding.y + bcPadding.w,
                                                 bcPadding.x + bcPadding.z);
        
        transform.position = mainCam.ScreenToWorldPoint(new Vector3(sRelPos.x * mainCam.scaledPixelWidth,
                                                                    sRelPos.y * mainCam.scaledPixelHeight,
                                                                    0));
        
        transform.position = new Vector3(transform.position.x + handOffset.x,
                                         transform.position.y + handOffset.y,
                                         -1);
        

        myCollider.offset = new Vector2(myCollider.size.x / 2 - (bcPadding.w + handSize.x / 2),
                                        myCollider.size.y / 2 - (bcPadding.z + handSize.y / 2));
    }

    internal void UpdateLayout(){/* 
        Debug.Log("HandManager :: UpdateLayout :: Starting method..."); */
        int childCount = transform.childCount;
        if(childCount < 2) return;

        float avgHeight = (handSize.y - cSize.y)/(childCount-1);
        float updateHeight = Mathf.Min(avgHeight, cVisible.y);
        float updateOffset = ((childCount-1)*updateHeight+cSize.y)/2;/*

        Debug.Log("HandManager :: UpdateLayout :: avgHeight = "+avgHeight.ToString());
        Debug.Log("HandManager :: UpdateLayout :: updateHeight = "+updateHeight.ToString());
        Debug.Log("HandManager :: UpdateLayout :: updateOffset = "+updateOffset.ToString());
        Debug.Log("HandManager :: UpdateLayout :: firstHeight = "+firstHeight.ToString()); */

        for (int i = 0; i < childCount; i++)
        {
            Transform t = transform.GetChild(i);
            if(t == null){
                Debug.Log("Missing child of index "+i.ToString());
                return;
            }
            t.position = new Vector3(t.position.x, cSize.y/2+updateHeight*i-updateOffset, -childCount+i);
        }
    }

    int lastChildCount = 0;
    public void Update(){
        if(lastChildCount != transform.childCount)
            UpdateLayout();
        lastChildCount = transform.childCount;
    }
}