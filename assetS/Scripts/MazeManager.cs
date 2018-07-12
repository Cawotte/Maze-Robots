using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeManager : MonoBehaviour {

    //Tilemap on which to display the Maze
    public Tilemap mazeTilemap;

    public Tile emptyCell_Tile;
    public Tile wall_Tile;
    public Tile starPos_Tile;
    public Tile exitPos_Tile;

    private Maze maze;

    public int height;
    public int width;


    public void DisplayMaze()
    {

        if ( maze == null )
        {
            Debug.Log("There is no maze to display !", this);
            return;
        }

        Tile mazeTile;

        //We remove all the previous tiles
        mazeTilemap.ClearAllTiles();

        for (int x = 0; x < maze.Height; x++)
        {
            for (int y = 0; y < maze.Width; y++)
            {
                if (new Vector2Int(x, y) == maze.StartPos)
                    mazeTile = starPos_Tile;
                else if (new Vector2Int(x, y) == maze.ExitPos)
                    mazeTile = exitPos_Tile;
                else if (maze.getCell(x, y) == 0)
                    mazeTile = emptyCell_Tile;
                else
                    mazeTile = wall_Tile;
                

                //reversed coordinate for common render, sorry.
                mazeTilemap.SetTile(new Vector3Int(y, x, 0), mazeTile);

            }
        }

        mazeTilemap.RefreshAllTiles();
        scaleAndCenterTilemap();
    }

    public void GenerateMaze()
    {
        maze = new Maze(height, width);

        maze.GenerateWith_RecursiveBacktracking();
        DisplayMaze();
    }

   
    private void scaleAndCenterTilemap()
    {
        float xScale = 1f;
        float yScale = 1f;
        float minScale;


        //Tilemap size fitting into the camera
        //10 cell tall, 19 cell wides.
        //Could be nice to calculate them ?
        if (mazeTilemap.size.x > 19)
        {
            xScale = 19f / mazeTilemap.size.x;
            //Debug.Log("xScale = " + xScale);
        }
        if (mazeTilemap.size.y > 10)
        {
            yScale = 10f / mazeTilemap.size.y;
            //Debug.Log("yScale = " + yScale);
        }

        minScale = Mathf.Min(xScale, yScale);

        //We scale the tilemap size to fit the screen.
        mazeTilemap.GetComponent<Transform>().localScale = new Vector3(minScale, minScale, 1f);

        //We recalculate the origin so it fits the screen.
        Vector3 centerPos = new Vector3( maze.Width / 2f, maze.Height / 2f, 0f);
        centerPos.Scale(new Vector3(minScale, minScale, 1f));

        mazeTilemap.GetComponent<Transform>().position = Vector3.zero - centerPos;
   
        //Debug.Log("Tilemap size : " + mazeTilemap.size);
        //Debug.Log("Tilemap cell's size" + mazeTilemap.cellSize);

        //x and y a reversed in tilemaps.

        
    }
}
