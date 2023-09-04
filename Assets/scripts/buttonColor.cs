using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttonColor : MonoBehaviour
{
    public Color flagActCol;
    public bool isFlagged = false;
    public Color flagDisCol;

    public ColorBlock colors;
    // Start is called before the first frame update
    public void SwitchButtonCol()
    {
        isFlagged = !isFlagged;
        colors = GetComponent<Button>().colors;
        if (isFlagged)
        {
            SetButCol(flagActCol);
        }
        else
        {
            SetButCol(flagDisCol);
        }

        GetComponent<Button>().colors = colors;
    }

    public void SetButCol(Color col)
    {
        colors.normalColor = col;
        colors.selectedColor = col;
        colors.highlightedColor = col;
        colors.pressedColor = col;
    }
}
