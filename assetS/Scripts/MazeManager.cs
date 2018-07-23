using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class MazeManager : MonoBehaviour {

    static int MAZE_MIN_SIZE = 5;
    static int MAZE_MAX_SIZE = 299;

    /*
    //Tilemap on which to display the Maze
    public Tilemap mazeTilemap;

    //Tiles used to generate maze (graphic assets)
    public Tile emptyCell_Tile;
    public Tile wall_Tile;
    public Tile starPos_Tile;
    public Tile exitPos_Tile; */

    //Frame in which the maze has to fit
    private float frame_width;
    private float frame_height;
    private Vector3 centerPosFrame;

    //UI Components
    public InputField inputField_height;
    public InputField inputField_width;
    //Maze Frame
    public GameObject frame_Maze;


    //private List<Maze> poolOfMaze = new List<Maze>();
    private Maze maze;

    public int mazeHeight;
    public int mazeWidth;

    private MazeRenderer mazeRenderer;

    private void Start()
    {

        //We trim the size of the maze to odd numbers :
        mazeHeight -= 1 - (mazeHeight % 2);
        mazeWidth -= 1 - (mazeWidth % 2);
        //We display the actual size of the maze to be generated
        inputField_width.text = mazeWidth.ToString();
        inputField_height.text = mazeHeight.ToString();

        mazeRenderer = this.GetComponent<MazeRenderer>();
        if (mazeRenderer == null) Debug.LogWarning("Pas de script MazeRenderer trouvé !");

    }
    

    public void GenerateMaze()
    {
        maze = new Maze(mazeHeight, mazeWidth);

        maze.GenerateWith_RecursiveBacktracking();
        mazeRenderer.DisplayMaze(maze);
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
