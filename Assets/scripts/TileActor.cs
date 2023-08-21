using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TileActor : MonoBehaviour
{
    // materials
    public Material defaultMaterial;
    public Material[] amountBombMats; // value 0-8 equals to the amount of bombs near it, 9 = mine , 10 = flag
    public GameObject spikeExplo;

    public bool hasExploded;
    // its value referring to the previous comment
    public int valueOfTheTile;
    private GameObject explo;

    public void OnClickAction()
    {
        // when uncovered shows the material corresponding to its value
        gameObject.GetComponent<Renderer>().material = amountBombMats[valueOfTheTile];
        gameObject.GetComponent<InteractableObject>().CanOutline(false);
    }

    public void SetGameValueMat(int e) 
    {
        // sets its value in the current game
        if (e>=0 && e<10)
        {
            valueOfTheTile = e;
            
        }
        else
        {
            print("bro that aint it");
        }
        
        UnFlagTheTile();
        hasExploded = false;
        Destroy(explo);
        gameObject.GetComponent<InteractableObject>().CanOutline(true);

    }

    public void SetDefMat()
    {
        gameObject.GetComponent<Renderer>().material = defaultMaterial;
    }

    public void FlagTheTile()
    {
        // when the player adds a flag
        gameObject.GetComponent<Renderer>().material = amountBombMats[10];
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void UnFlagTheTile()
    {
        // when the player removes the flag
        print("remove");
        gameObject.GetComponent<Renderer>().material = defaultMaterial;
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void Explode()
    {
        hasExploded = true;
        explo = Instantiate(spikeExplo, new Vector3(gameObject.transform.position.x,gameObject.transform.position.y+0.5f,gameObject.transform.position.z),Quaternion.identity);
    }
}
