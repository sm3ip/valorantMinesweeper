using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    bool objHighlighted;
    private bool canHigh = true;

    static void Swaplayer(GameObject obj, string layerName)
    {
        Swaplayer(obj, layerName, true);
    }

    static void Swaplayer(GameObject obj, string layerName, bool children)
    {
        obj.layer = LayerMask.NameToLayer(layerName);

        if (children)
        {
            foreach (Transform child in obj.transform)
            {
                Swaplayer(child.gameObject, layerName, children);
            }
        }
    }

    public void ObjectMouseEnter()
    {
        if (canHigh)
        {
            gameObject.transform.GetChild(1).gameObject.SetActive(true);
            //Swaplayer(gameObject, "HighlightOutline");
        }
        
    }

    public void ObjectMouseExit()
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
        //Swaplayer(gameObject, "Default");
    }

    public void CanOutline(bool f)
    {
        //print(f + gameObject.name);
        canHigh = f;
    }
}
