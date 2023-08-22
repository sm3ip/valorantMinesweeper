using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    bool objHighlighted;
    private bool canHigh = true;

    public void ObjectMouseEnter()
    {
        if (canHigh)
        {
            gameObject.transform.GetChild(1).gameObject.SetActive(true);
        }
        
    }

    public void ObjectMouseExit()
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void CanOutline(bool f)
    {
        canHigh = f;
    }
}
