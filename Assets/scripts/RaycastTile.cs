using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastTile : MonoBehaviour
{
    // This script is here to check if the player interacts with the environment
    private Ray ray; // the raycast
    private RaycastHit hit; // the object hit by the raycast
    public bool canPlay = true;
    public bool isFlag = false;

    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition); // draws a raycast from the mouse position
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0)) // in case of a left click
            {
                if (hit.collider.CompareTag("Tile") && canPlay) // if the object hit by the raycast has the tag Tile
                {
                    Gameplan.Coords
                        temp = ParseTileCoords(hit.collider.name); // gets the tile's coordinates from its name
                    if (isFlag)
                    {
                        gameObject.GetComponent<Gameplan>()
                            .flagIt(temp.X, temp.Y); // calls the function to "flag" the corresponding tile
                    }
                    else
                    {
                        gameObject.GetComponent<Gameplan>()
                            .uncoverTileUSER(temp.X, temp.Y); // calls the function to "uncover" the corresponding tile
                    }

                }

            }
        }
    }

    public Gameplan.Coords ParseTileCoords(string name)
    {
        // Parses the tile's name into its actual coordinates
        return new Gameplan.Coords(Int32.Parse(name.Substring(5, name.IndexOf(":") - 5)),
            Int32.Parse(name.Substring(name.IndexOf(":") + 1)));
    }

    public void SwitchFlag()
    {
        isFlag = !isFlag;
    }
}
