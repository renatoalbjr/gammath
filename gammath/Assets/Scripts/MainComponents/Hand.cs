using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hand : Slot
{
    #region Variables
    // ---Cache the main camera---
    private Camera mainCam;

    // ---Descriptive variables to be set in the inspector---
    [SerializeField] private Vector2 cardDimensions;
    [SerializeField] private Vector2 maxVisibleCardDimensions;
    [SerializeField] private Vector2 screenRelativePosition;
    [SerializeField] private Vector2 handSize;
    [SerializeField] private Vector2 handOffset;
    /// <summary>CSS shorthand style: x = top, y = right, z = bottom, w = left</summary>
    public Vector4 boxColliderPadding;

    // ---Internal use short variables---
    private Vector2 cSize;
    private Vector2 cVisible;
    private Vector2 sRelPos;
    /// <summary>CSS shorthand style: x = top, y = right, z = bottom, w = left</summary>
    private Vector4 boxCPadding;
    private BoxCollider2D boxCollider;
    #endregion
    
    // ########################################################################################## //

    #region Unity Methods
    #region Start
    // ########################################################################################## //

    internal override void Start()
    {
        base.Start();

        // ---Initialize variables and gather components---
        mainCam = Camera.main;
        boxCollider = GetComponent<BoxCollider2D>();

        cSize = cardDimensions;
        cVisible = maxVisibleCardDimensions;
        sRelPos = screenRelativePosition;
        boxCPadding = boxColliderPadding;

        // ---Clamps sRelPos values to 0,1---
        if(sRelPos.x > 1) sRelPos.x = 1;
        if(sRelPos.y > 1) sRelPos.y = 1;
        if(sRelPos.x < 0) sRelPos.x = 0;
        if(sRelPos.y < 0) sRelPos.y = 0;

        // ---Resizes the collider---
        boxCollider.size = handSize + new Vector2(boxCPadding.y + boxCPadding.w,
                                                 boxCPadding.x + boxCPadding.z);
        
        // ---Aligns with the given relative coordinates on screen---
        transform.position = mainCam.ScreenToWorldPoint(new Vector3(sRelPos.x * mainCam.scaledPixelWidth,
                                                                    sRelPos.y * mainCam.scaledPixelHeight,
                                                                    0));
        
        // ---Replaces to its proper layer---
        transform.position = new Vector3(transform.position.x + handOffset.x,
                                         transform.position.y + handOffset.y,
                                         -1);
        

        // ---Recenter the box collider applying the given padding---
        boxCollider.offset = new Vector2(boxCollider.size.x / 2 - (boxCPadding.w + handSize.x / 2),
                                        boxCollider.size.y / 2 - (boxCPadding.z + handSize.y / 2));
    }
    #endregion

    #region Update
    // ########################################################################################## //

    int lastChildCount = 0;
    /// <summary>Use intended to be only for development</summary>
    public void Update(){
        if(lastChildCount != transform.childCount)
            UpdateLayout();
        lastChildCount = transform.childCount;
    }
    #endregion
    #endregion

    // ########################################################################################## //

    #region Update layout
    // ########################################################################################## //

    internal void UpdateLayout(){
        int childCount = transform.childCount;
        if(childCount < 2) return;

        // ---Determines if can evenly distibute children without exceeding max height---
        float avgHeight = (handSize.y - cSize.y)/(childCount-1);
        float updateHeight = Mathf.Min(avgHeight, cVisible.y);
        // ---Calculates the offset to make the whole layout recentered---
        float updateOffset = ((childCount-1)*updateHeight+cSize.y)/2;

        /// ---Reposition each child---
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

    #endregion

    // ---Edit from here---


}