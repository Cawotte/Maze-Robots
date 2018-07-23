using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class MazeRenderer : MonoBehaviour
{

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
    private Vector3 frame_center;
    
    //Maze Frame
    public GameObject panel_UI;
    public GameObject panel_frame_UI;

    private void Start()
    {

        /*
        float camera_height = 2 * Camera.main.orthographicSize;
        float camera_width = camera_height * Camera.main.aspect;
        Debug.Log("Camera : (h = " + camera_height + ", w = " + camera_width + ")\n");
        */

        //We get the RectTransform to get the WorldPosition of the panel to calculate how much place the maze can take.
        RectTransform rect_framePanel = panel_frame_UI.GetComponent<RectTransform>();

        Vector3[] frame_corners = new Vector3[4];
        rect_framePanel.GetWorldCorners(frame_corners);
        


        //We get the size of the UI panel that will serve as a reference to scale the maze size, so it fits the screen on any resolution.
        frame_height = frame_corners[1].y - frame_corners[0].y;
        frame_width = frame_corners[3].x - frame_corners[0].x;
        frame_center = rect_framePanel.position;
        
        Debug.Log("bounds : (h = " + frame_height + ", w = " + frame_width + ")\n" +
                "      CenterPos = " + frame_center);
        

    }


    public void DisplayMaze(Maze maze)
    {

        if (maze == null)
        {
            Debug.LogWarning("There is no maze to display !", this);
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
        scaleAndCenterTilemap(maze);
    }
    

    private void scaleAndCenterTilemap(Maze maze)
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
        Vector3 centerPosMaze = new Vector3(maze.Width / 2f, maze.Height / 2f, 0f);
        centerPosMaze.Scale(new Vector3(minScale, minScale, 1f));
        mazeTilemap.GetComponent<Transform>().position = frame_center - centerPosMaze;

        //Debug.Log("Tilemap size : " + mazeTilemap.size);
        //Debug.Log("Tilemap cell's size" + mazeTilemap.cellSize);

        // !!! ----------- x and y a reversed in tilemaps. -------------------

    }

}
