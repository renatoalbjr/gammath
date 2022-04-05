using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Deck : ContainerBase, IBelong, IPointerClickHandler
{
    private Player owner;

    #region Variables
    
    #endregion

    // ########################################################################################## //

    #region Unity Methods
    
    #region Awake
    internal void Awake(){
        
    }
    #endregion

    #region Start
    // ########################################################################################## //

    internal override void Start()
    {
        base.Start();
        placeholderPrefab = null; //There shouldn't be a placeholder
        placeholder = null;
        _UpdateLayout(0, transform.childCount-1);
    }
    #endregion

    #region Update
    // ########################################################################################## //

    #endregion
    #endregion

    #region Overrides
    internal override void ValidateCanPlace<T>(T tObj)
    {
        // ---Checks if theres room for the object in the container---
        base.ValidateCanPlace(tObj);
    }

    //Guarantees that no placeholder will be created
    internal override bool CreatePlaceholder<T>(T tObj){return false;}
    #endregion

    #region Update Layout
    // ########################################################################################## //

    private void _UpdateLayout(int firstIndex, int lastIndex){
        if(transform.childCount < 2) return;
        if(transform.childCount <= lastIndex) return;

        for (int i = firstIndex; i <= lastIndex; i++)
        {
            Transform t = transform.GetChild(i);
            if(t == null){
                Debug.Log("Missing child of index "+i.ToString());
                return;
            }
            t.position = new Vector3(t.position.x, t.position.y, -transform.childCount+i);
        }
    }
    #endregion

    #region PlaceAt
    // ########################################################################################## //

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
        // ---Disable Draggable---
        Draggable dragComp = cardTransform.GetComponent<Draggable>();
        if(dragComp != null) dragComp.enabled = false;
    }
    public void PlaceAt(Transform cardTransform, int index){
        _checkIndexes(index);
        int fixedIndex = Mathf.Clamp(index, 0, filledCapacity-1);
        _PlaceAt(cardTransform, fixedIndex);
        _UpdateLayout(0, fixedIndex);
    }
    public void PlaceAtTop(Transform cardTransform){
        _PlaceAt(cardTransform, 0);
    }
    public void PlaceAtBottom(Transform cardTransform){
        _PlaceAt(cardTransform, filledCapacity);
    }
    #endregion

    #region PlaceRandom
    // ########################################################################################## //

    /// <summary>
    /// Place a card at a random index between a range
    /// </summary>
    /// <param name = "cardTransform">
    /// The card to be placed
    /// </param>
    /// <param name = "firstIndex">
    /// From 0 to filledCapacity-1
    /// </param>
    /// <param name = "lastIndex">
    /// From 0 to filledCapacity-1
    /// </param>
    public void PlaceRandom(Transform cardTransform, int topmostIndex, int bottommostIndex){
        _checkIndexes(topmostIndex, bottommostIndex);
        int firstIndex = Mathf.Min(topmostIndex, bottommostIndex, filledCapacity-1);
        int lastIndex = Mathf.Max(0, topmostIndex, bottommostIndex);

        _PlaceRandom(cardTransform, firstIndex, lastIndex);
    }
    private void _PlaceRandom(Transform cardTransform, int firstIndex, int lastIndex){
        int randIndex = Random.Range(firstIndex, lastIndex);
        _PlaceAt(cardTransform, randIndex);
    }
    #endregion

    #region Shuffle
    // ########################################################################################## //
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
    #endregion

    #region RemoveAt
    private Transform _RemoveAt(int index){
        // ---Get the removed object reference---
        Transform removed = transform.GetChild(index);

        // ---Enable Draggable at card---
        Draggable dragComp = removed.GetComponent<Draggable>();
        if(dragComp != null) dragComp.enabled = true;


        // ---Remove using ContainerBase method---
        Remove(removed);
        return removed;
    }
    public Transform RemoveAt(int index){
        //Can't remove if there's nothing inside
        if(filledCapacity == 0) return null;

        _checkIndexes(index);
        int fixedIndex = Mathf.Clamp(index, 0, filledCapacity-1);
        return _RemoveAt(fixedIndex);
    }
    #endregion

    #region Debug printers
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

    #endregion
    public Player GetOwner()
    {
        return owner;
    }

    public Player BelongsTo(Player p)
    {
        Debug.Log(string.Format("{0} now belongs to {1}", gameObject.name, p.gameObject.name));
        return owner = p;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("The deck "+gameObject.name+" was clicked");
        EventManager.Instance.StartClickOnDeck(this);
    }
}
