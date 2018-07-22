using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class MazeManager : MonoBehaviour {

    static int MAZE_MIN_SIZE = 5;
    static int MAZE_MAX_SIZE = 299;

    //Tilemap on which to display the Maze
    public Tilemap mazeTilemap;

    //Tiles used to generate maze (graphic assets)
    public Tile emptyCell_Tile;
    public Tile wall_Tile;
    public Tile starPos_Tile;
    public Tile exitPos_Tile;

    //Frame in which the maze has to fit
    private float frame_width;
    private float frame_height;
    private Vector3 centerPosFrame;

    //UI Components
    public InputField inputField_height;
    public InputField inputField_width;


    //private List<Maze> poolOfMaze = new List<Maze>();
    private Maze maze;

    public int mazeHeight;
    public int mazeWidth;

    private void Start()
    {

        //We trim the size of the maze to odd numbers :
        mazeHeight -= 1 - (mazeHeight % 2);
        mazeWidth -= 1 - (mazeWidth % 2);
        //We display the actual size of the maze to be generated
        inputField_width.text = mazeWidth.ToString();
        inputField_height.text = mazeHeight.ToString();

        //Get the "frame" of the maze representated by a gameObject (Just a RectTranstorm)
        RectTransform mazeBounds = (RectTransform) GameObject.Find("MazeBounds").transform;
        if ( mazeBounds == null )
        {
            Debug.Log("No MazeBounds found !");
        }
        else
        {
            //bound_height = 2 * Camera.main.orthographicSize;
            //bound_width = height * Camera.main.aspect;

            frame_height = mazeBounds.rect.height;
            frame_width = mazeBounds.rect.width;
            centerPosFrame = mazeBounds.position;
            Debug.Log("bounds : (h = " + frame_height + ", w = " + frame_width + ")\n" +
                "      CenterPos = " + centerPosFrame);

        }

    }


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
        maze = new Maze(mazeHeight, mazeWidth);

        maze.GenerateWith_RecursiveBacktracking();
        DisplayMaze();
    }

   
    private void scaleAndCenterTilemap()
    {
        float xScale = 1f;
        float yScale = 1f;
        float minScale;

        //Camera settings
        //float xWidth = 19f;
        //float yWidth = 10f;


        //Tilemap size scaled to fit into the frame
        if (mazeTilemap.size.x > frame_width)
        {
            xScale = frame_width / mazeTilemap.size.x;
            //Debug.Log("xScale = " + xScale);
        }
        if (mazeTilemap.size.y > frame_height)
        {
            yScale = frame_height / mazeTilemap.size.y;
            //Debug.Log("yScale = " + yScale);
        }

        minScale = Mathf.Min(xScale, yScale);

        //We scale the tilemap size to fit the screen.
        mazeTilemap.GetComponent<Transform>().localScale = new Vector3(minScale, minScale, 1f);

        //We recalculate the origin so it fits the screen.
        Vector3 centerPosMaze = new Vector3( maze.Width / 2f, maze.Height / 2f, 0f);
        centerPosMaze.Scale(new Vector3(minScale, minScale, 1f));
        mazeTilemap.GetComponent<Transform>().position = centerPosFrame - centerPosMaze;

        //Debug.Log("Tilemap size : " + mazeTilemap.size);
        //Debug.Log("Tilemap cell's size" + mazeTilemap.cellSize);

        // !!! ----------- x and y a reversed in tilemaps. -------------------

    }

    //Functions for the UI

    public void increaseHeight()
    {
        mazeHeight += 2;
        mazeHeight = fitSizeToBounds(mazeHeight);
        inputField_height.text = mazeHeight.ToString();
    }

    public void decreaseHeigth()
    {
        mazeHeight -= 2;
        mazeHeight = fitSizeToBounds(mazeHeight);
        inputField_height.text = mazeHeight.ToString();
    }

    public void increaseWidth()
    {
        mazeWidth += 2;
        mazeWidth = fitSizeToBounds(mazeWidth);
        inputField_width.text = mazeWidth.ToString();
    }

    public void decreaseWidth()
    {
        mazeWidth -= 2;
        mazeWidth = fitSizeToBounds(mazeWidth);
        inputField_width.text = mazeWidth.ToString();
    }

    public void changeHeight()
    {
        int newHeight = int.Parse(inputField_height.text);
        
        mazeHeight = fitSizeToBounds(newHeight);

        inputField_height.text = mazeHeight.ToString();
    }

    public void changeWidth()
    {
        int newWidth = int.Parse(inputField_width.text);

        mazeWidth = fitSizeToBounds(newWidth);

        inputField_width.text = mazeWidth.ToString();
    }
    
    public int fitSizeToBounds(int size)
    {
        if (size < MAZE_MIN_SIZE)
        {
            size = MAZE_MIN_SIZE;
        }
        else if (size > MAZE_MAX_SIZE)
        {
            size = MAZE_MAX_SIZE;
        }
        else if (size % 2 == 0)
        {
            size--;
        }
        return size;
    }
}
