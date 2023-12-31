using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Gameplan : MonoBehaviour
{
    public struct Coords
    {
        // the coordinates structure 
        public Coords(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X { get; }
        public int Y { get; }
        public override string ToString() => $"({X},{Y})";
    }
    
    //vars
    
    private System.Random rnd = new System.Random(); // a random generator
    private int _amountBombs; // the amount of bombs in the current game (feature is yet to be implemented)
    public GameObject tile; // the prefab of the tiles to generate the grid
    public GameObject track; // to track it and give it a name
    public GameObject canvas; // the ui containing the option to restart and difficulty options
    public GameObject victoryScene;
    public GameObject loseScene;
    public Vector3 origin; // the beginning of the grid
    public float marginX; // the margin in the X-axis between the tiles
    public float marginZ; // the margin in the Z-axis between the tiles
    private Coords _startClick; // the tile uncovered by the system to begin a game
    public int difficultyLvl = 13;

    public int nbHeight;
    public int nbWidth;
    private int[,] dataTab;
    private int[,] stateTab;
    public GameObject bombText;
    public GameObject flagText;
    public GameObject buttonDiff1;
    public GameObject buttonDiff2;
    public GameObject buttonDiff3;
    public GameObject difficultyText;
    public GameObject streakTxt;
    private int _streak;
    private int flagsUsed;
        
    // Start is called before the first frame update
    void Start()
    {
        SetStreak(0);
        _startClick = new Coords(0, 0); // sets it as the first tile
        
        dataTab = new int[nbHeight, nbWidth]; // contains the numbers corresponding to the amount of nearest mines (-1 means that there's a bomb)
        stateTab = new int[nbHeight, nbWidth]; // 1 if hidden, 2 if discovered, 0 if flagged
        Vector3 tmpOri = origin; // create a temporary origin for modification purposes
        // goes through the grid to instantiate the tiles
        for (int i = 0; i < nbHeight; i++)
        {
            for (int j = 0; j < nbWidth; j++)
            {
                track = Instantiate(tile, tmpOri, Quaternion.identity);
                track.name = "tile_" + i + ":" + j;
                tmpOri = new Vector3(tmpOri.x + marginX, tmpOri.y, tmpOri.z);
            }
            tmpOri = new Vector3(origin.x, tmpOri.y, tmpOri.z + marginZ);
        }
        startGame(difficultyLvl);// automatically starts a game
        uncoverTileUSER(_startClick.X,_startClick.Y); // and uncovers the first tile 
    }

    public void RestartGame()
    {
        for (int i = 0; i < nbHeight; i++) // resets the material for each tile
        {
            for (int j = 0; j < nbWidth; j++)
            {
                GameObject Tile = GameObject.Find("tile_" + i + ":" + j);
                Tile.GetComponent<TileActor>().SetDefMat();
            }
        }
        loseScene.SetActive(true);
        victoryScene.SetActive(true);
        canvas.SetActive(false);
        startGame(difficultyLvl);
        uncoverTileUSER(_startClick.X,_startClick.Y);
        gameObject.GetComponent<RaycastTile>().canPlay = true;
    }

    public void startGame(int bombOccurence) 
    {
        // setups the multiple variables to start a game
        //setup of bombs
        _amountBombs = 0;
        flagsUsed = 0;
        for (int i = 0; i < nbHeight; i++)
        {
            for (int j = 0; j < nbWidth; j++)
            {
                dataTab[i, j] = 0;
                stateTab[i, j] = 1; // everything is set as hidden
                int Rd = rnd.Next(1, bombOccurence);
                if (Rd == 1)
                {
                    dataTab[i, j] = -1; // sets up the mines randomly ( 1 chance out of bombOccurence) 
                    _amountBombs += 1;
                }
            }
        }
        
        //setup of numbers (close bombs)
        for (int i = 0; i < nbHeight; i++) // goes through the lines
        {
            for (int j = 0; j < nbWidth; j++) // goes through the columns
            {
                GameObject tileOnScene = GameObject.Find("tile_" + i + ":" + j); // finds the corresponding tile in the scene
                if (dataTab[i,j]!=-1) // if it aint a mine
                {
                    // counts the nearest bombs
                    int amountOfBombs = seekBombs(i,j);
                    dataTab[i, j] = amountOfBombs;
                    tileOnScene.GetComponent<TileActor>().SetGameValueMat(amountOfBombs);
                    if (amountOfBombs == 0) // if there's no bomb near this tile, stores its coordinates to be the first tile uncovered
                    {
                        _startClick = new Coords(i, j);
                    }
                }
                else
                {
                    tileOnScene.GetComponent<TileActor>().SetGameValueMat(9);
                }
            }
        }

        bombText.GetComponent<TMPro.TextMeshProUGUI>().text = _amountBombs.ToString();
        flagText.GetComponent<TMPro.TextMeshProUGUI>().text = flagsUsed.ToString();
    }

    public int seekBombs(int x, int y)
    {
        // returns the amount of bombs near a particular tile (0-8)
        int result = 0;
        for (int k = x-1; k < x+2; k++) // goes through the lines before present and after
        {
            if (k<nbHeight && k>=0) // if it aint out of bounds
            {
                //check the line of 3
                for (int l = y-1; l < y+2; l++) // goes through the columns before present and after
                {
                    if (l<nbWidth && l>=0) // if it aint out of bounds
                    {
                        if (dataTab[k,l]==-1) // if its a bomb add it to the counter
                        {
                            result += 1;
                        }
                    }
                }
                            
            }
        }
        return result;
    }

    public void uncoverTileUSER(int x, int y)
    {
        // when the user / or the first move made by the system tries to uncover a particular tile
        if (stateTab[x,y]==1) // if the tile is already hidden
        {
            // uncovers it both for the system and the user
            stateTab[x, y] = 2;
            GameObject.Find("tile_" + x + ":" + y).GetComponent<TileActor>().OnClickAction();
            if (dataTab[x,y]==0) // if there's no nearest bomb
            {
                List<Coords> toDomino = new List<Coords>();
                // uncovers every near tile
                for (int k = x-1; k < x+2; k++) // goes through the lines before present and after
                {
                    if (k<nbHeight && k>=0) // if it aint out of bounds
                    {
                        //check the line of 3
                        for (int l = y-1; l < y+2; l++) // goes through the columns before present and after
                        {
                            if (l<nbWidth && l>=0) // if it aint out of bounds
                            {
                                stateTab[k, l] = 2;
                                GameObject.Find("tile_" + k + ":" + l).GetComponent<TileActor>().OnClickAction();
                                if (dataTab[k,l]==0)
                                {
                                    toDomino.Add(new Coords(k,l));
                                }
                            }
                        }
                            
                    }
                }
                // for each tile that was at 0 nearest bomb
                foreach (var domi in toDomino)
                {
                    uncoverTileDOMINO(domi.X,domi.Y);
                }
            }else if (dataTab[x,y]==-1) // if it was a bomb
            {
                GameObject.Find("tile_" + x + ":" + y).GetComponent<TileActor>().Explode();
                OnLost();
            }

            if (hasWon()) // check if won
            {
                OnVictory();
            }
        }
    }

    public void uncoverTileDOMINO(int x, int y)
    {
        // uncover tiles by itself if previous uncover tile had 0 bombs near it
        List<Coords> toReDomino = new List<Coords>();
        for (int k = x-1; k < x+2; k++) // goes through the lines before present and after
        {
            if (k<nbHeight && k>=0) // if it aint out of bounds
            {
                //check the line of 3
                for (int l = y-1; l < y+2; l++) // goes through the columns before present and after
                {
                    if (l<nbWidth && l>=0) // if it aint out of bounds
                    {
                        
                        if (dataTab[k,l]==0 && stateTab[k,l]!=2)
                        {
                            toReDomino.Add(new Coords(k,l));
                        }
                        stateTab[k, l] = 2;
                        GameObject.Find("tile_" + k + ":" + l).GetComponent<TileActor>().OnClickAction();
                    }
                }
                            
            }
        }

        foreach (var reDomi in toReDomino)
        {
            // recalls itself if other tile with 0 bombs near it were found
            uncoverTileDOMINO(reDomi.X,reDomi.Y);
        }
    }

    public void flagIt(int x, int y)
    {
        // called when the player tries to flag a tile
        switch (stateTab[x,y])
        {
            case 1: // if hidden flag it
                stateTab[x, y] = 0;
                GameObject.Find("tile_" + x + ":" + y).GetComponent<TileActor>().FlagTheTile();
                flagsUsed += 1;
                flagText.GetComponent<TMPro.TextMeshProUGUI>().text = flagsUsed.ToString();
                break;
            case 0: // if flagged hides it
                stateTab[x, y] = 1;
                GameObject.Find("tile_" + x + ":" + y).GetComponent<TileActor>().UnFlagTheTile();
                flagsUsed -= 1;
                flagText.GetComponent<TMPro.TextMeshProUGUI>().text = flagsUsed.ToString();
                break;
        }
    }

    public bool hasWon()
    {
        // checks if the player has won ( might modify the conditions, not sure tho)
        for (int i = 0; i < nbHeight; i++)
        {
            for (int j = 0; j < nbWidth; j++)
            {
                if (stateTab[i,j]!=2 && dataTab[i,j] != -1)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void DifficultyChange(int n)
    {
        switch (n)
        {
            case 1:
                if (difficultyLvl != 13)
                {
                    SetStreak(0);
                    difficultyLvl = 13;
                    buttonDiff1.transform.GetChild(1).gameObject.SetActive(true);
                    buttonDiff2.transform.GetChild(1).gameObject.SetActive(false);
                    buttonDiff3.transform.GetChild(1).gameObject.SetActive(false);
                    difficultyText.GetComponent<TMPro.TextMeshProUGUI>().text = "Ez";
                }
                break;
            case 2:
                if (difficultyLvl !=8)
                {
                    SetStreak(0);
                    difficultyLvl = 8;
                    buttonDiff1.transform.GetChild(1).gameObject.SetActive(false);
                    buttonDiff2.transform.GetChild(1).gameObject.SetActive(true);
                    buttonDiff3.transform.GetChild(1).gameObject.SetActive(false);
                    difficultyText.GetComponent<TMPro.TextMeshProUGUI>().text = "Moderate";
                }
                break;
            case 3:
                if (difficultyLvl!=5)
                {
                    SetStreak(0);
                    difficultyLvl = 5;
                    buttonDiff1.transform.GetChild(1).gameObject.SetActive(false);
                    buttonDiff2.transform.GetChild(1).gameObject.SetActive(false);
                    buttonDiff3.transform.GetChild(1).gameObject.SetActive(true);
                    difficultyText.GetComponent<TMPro.TextMeshProUGUI>().text = "Hard";
                }
                break;
        }
    }

    public void OnLost()
    {
        SetStreak(0);
        gameObject.GetComponent<RaycastTile>().canPlay = false;
        canvas.SetActive(true);
        victoryScene.SetActive(false);
    }

    public void OnVictory()
    {
        SetStreak(_streak+1);
        gameObject.GetComponent<RaycastTile>().canPlay = false;
        canvas.SetActive(true);
        loseScene.SetActive(false);
    }

    public void SetStreak(int i)
    {
        _streak = i;
        streakTxt.GetComponent<TMPro.TextMeshProUGUI>().text = _streak.ToString() + " in a row";
    }
}
