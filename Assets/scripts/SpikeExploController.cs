using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeExploController : MonoBehaviour
{

    public int maxSize;

    public Vector3 scaleChange;

    public float xAngle, yAngle, zAngle;
    // Start is called before the first frame update
    void Start()
    {
        //scaleChange = new Vector3(0.005f, 0.005f, 0.005f);
    }

    void FixedUpdate()
    {
        if (gameObject.transform.localScale.y < maxSize)
        {
            gameObject.transform.localScale += scaleChange;
        }

        gameObject.transform.Rotate(xAngle,yAngle,zAngle,Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tile"))
        {
            if (other.GetComponent<TileActor>().valueOfTheTile ==9 && !other.GetComponent<TileActor>().hasExploded)
            {
                other.GetComponent<TileActor>().Explode();
            }
        }
    }
}
