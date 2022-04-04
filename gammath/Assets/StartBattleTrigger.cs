using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StartBattleTrigger : MonoBehaviour
{
    [SerializeField] private PlayerMovement p;
    [SerializeField] private BoxCollider2D bc;
    [SerializeField] private UnityEvent OnPortal = new UnityEvent();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(bc.IsTouching(p.GetComponent<BoxCollider2D>())){
            OnPortal?.Invoke();
        }
    }
}
