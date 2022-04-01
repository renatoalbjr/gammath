using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Deck : ContainerBase//, IPointerClickHandler
{
    public override void Start()
    {
        base.Start();
        placeholderPrefab = null; //There shouldn't be a placeholder
        placeholder = null;
        _UpdateLayout(0, transform.childCount);
    }

    private void _UpdateLayout(int firstIndex, int lastIndex){
        if(transform.childCount < 2) return;

        for (int i = firstIndex; i < lastIndex; i++)
        {
            Transform t = transform.GetChild(i);
            if(t == null){
                Debug.Log("Missing child of index "+i.ToString());
                return;
            }
            t.position = new Vector3(t.position.x, t.position.y, -transform.childCount+i);
        }
    }

    


    private void _PlaceAt(Transform cardTransform, int index){
        if(cardTransform == null){
            Debug.Log("Object being placed is null");
            return;
        }
        if(placeholder != null){
            Debug.Log("Placeholder not null in deck: "+gameObject.name);
            Debug.Log("Destroying placeholder at deck: "+gameObject.name);
            DestroyPlaceholder(cardTransform);
        }

        base.Place(cardTransform); //Parent the card and centralizes it with the Deck
        cardTransform.SetSiblingIndex(index);
        cardTransform.position = new Vector3(cardTransform.position.x,
                                                cardTransform.position.y,
                                                -transform.childCount+index);
    }
    public void PlaceAt(Transform cardTransform, int index){
        _checkIndexes(index);
        int fixedIndex = Mathf.Clamp(index, 0, filledCapacity-1);
        _PlaceAt(cardTransform, fixedIndex);
        _UpdateLayout(0, fixedIndex);
    }

    public void PlaceTop(Transform cardTransform){
        _PlaceAt(cardTransform, 0);
    }
    public void PlaceBottom(Transform cardTransform){
        _PlaceAt(cardTransform, filledCapacity);
    }

    //From 0 to filledCapacity-1
    private void _PlaceRandom(Transform cardTransform, int firstIndex, int lastIndex){
        int randIndex = Random.Range(firstIndex, lastIndex);
        _PlaceAt(cardTransform, randIndex);
    }
    public void PlaceRandom(Transform cardTransform, int topmostIndex, int bottommostIndex){
        _checkIndexes(topmostIndex, bottommostIndex);
        int firstIndex = Mathf.Min(topmostIndex, bottommostIndex, filledCapacity-1);
        int lastIndex = Mathf.Max(0, topmostIndex, bottommostIndex);

        _PlaceRandom(cardTransform, firstIndex, lastIndex);
    }


    public void Shuffle(int topmostIndex, int bottommostIndex){
        _checkIndexes(topmostIndex, bottommostIndex);
        int firstIndex = Mathf.Min(topmostIndex, bottommostIndex, filledCapacity-1);
        int lastIndex = Mathf.Max(0, topmostIndex, bottommostIndex);
        
        //For each child from firstIndex to lastIndex, unparent it then PlaceRandom again
        for(int i = firstIndex; i <= lastIndex; i++){
            Transform child = transform.GetChild(i);

            base.Remove(child);
            _PlaceRandom(child, firstIndex, lastIndex);
        }
    }

    internal override void ValidateCanPlace<T>(T tObj)
    {
        base.ValidateCanPlace(tObj);
    }

    //Guarantees that no placeholder will be created
    internal override void CreatePlaceholder<T>(T tObj){}

    private void _checkIndexes(int topmostIndex, int bottommostIndex){

        if(   topmostIndex    < 0 || topmostIndex    >= filledCapacity

           || bottommostIndex < 0 || bottommostIndex >= filledCapacity

           || topmostIndex >= bottommostIndex
           ){

            Debug.Log("Invalid indexes passed to DeckManager, topmostIndex = "
                      + topmostIndex.ToString()
                      + " bottommostIndex = "
                      + bottommostIndex.ToString());

        }
    }
    private void _checkIndexes(int index){
        if(index < 0 || index >= filledCapacity)
            Debug.Log("Invalid index passed to DeckManager, index = " + index.ToString());
    }

    private void _RemoveAt(int index){
        Remove(transform.GetChild(index));
    }
    public void RemoveAt(int index){
        _checkIndexes(index);
        int fixedIndex = Mathf.Clamp(index, 0, filledCapacity-1);
        _RemoveAt(fixedIndex);
    }
}
