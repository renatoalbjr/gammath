using UnityEngine;
using System;

public class EventManager : MonoBehaviour 
{
    public static EventManager current;
    public event Action ExampleEvent;

    private void Awake()
    {
        if(current == null)
            current = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Update(){
        if(Input.GetMouseButtonDown(0)){
            //if(ExampleEvent != null)
            //   ExampleEvent();

            ExampleEvent?.Invoke(); 
        }
    }

    /* public void StartExampleEvent(int id){
        ExampleEvent?.Invoke();
    } */
}
