using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{

    private IRayProvider rayProvider;
    private ISelector selector;
    private ISelectionResponse selectionResponse;

    private GameObject previousSelection;
    private GameObject currentSelection;

    // Start is called before the first frame update
    void OnAwake()
    {
        selector = GetComponent<ISelector>();
        rayProvider = GetComponent<IRayProvider>();
        selectionResponse = GetComponent<ISelectionResponse>();
    }

    // Update is called once per frame
    void Update()
    {
        currentSelection = selector.Check(rayProvider.CreateRay());

        if (currentSelection == null)
        {
            if (previousSelection != currentSelection)
            {
                selectionResponse.OnDeselect(previousSelection);
                selectionResponse.OnSelect(currentSelection);
                currentSelection = previousSelection;
            }
        }
    }
}
