using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPlaceholder : PlaceholderBase
{
    private HandManager hm;

    public override void MoveUp(){
        if(transform.parent == null) return;

        hm = transform.parent.GetComponent<HandManager>();
        if(hm == null) return;

        hm.MoveChildUp(transform.GetSiblingIndex());
    }
    public override void MoveDown(){
        if(transform.parent == null) return;

        hm = transform.parent.GetComponent<HandManager>();
        if(hm == null) return;
        
        hm.MoveChildDown(transform.GetSiblingIndex());
    }
    public override void MoveLeft(){
        return;
    }
    public override void MoveRight(){
        return;
    }
}
