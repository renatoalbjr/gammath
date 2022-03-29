using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{

    public int nRows;
    public int nColumns;
    public float cardSlotWidth;
    public float cardSlotHeight;

    public CardSlot slotPrefab;

    void Awake()
    {
        for(int i = 0; i < nRows; i++){
            for(int j = 0; j < nColumns; j++){
                GameObject newSlot = Instantiate(slotPrefab.gameObject,new Vector3((j+0.5f)*cardSlotWidth, (i+0.5f)*cardSlotHeight), Quaternion.identity);
                newSlot.transform.SetParent(transform);
            }
        }
        transform.position = new Vector3(-nColumns*cardSlotWidth/2, -nRows*cardSlotHeight/2);
    }
}
