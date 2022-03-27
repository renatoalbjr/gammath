using UnityEngine;
using System;

public class EventManager : MonoBehaviour 
{
    public static EventManager current;
    public event Action<GameObject> OnCardBeginDrag;

    private void Awake()
    {
        if(current == null)
            current = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void StartCardBeginDrag(GameObject card){
        OnCardBeginDrag?.Invoke(card);
    }
}
