using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //Can be saved in file
public class Maze {

    private int height;
    public int Height
    {
        get { return height; }
    }
    private int width;
    public int Width
    {
        get { return width; }
    }

    // 0 = empty cell
    // 1 = wall
    private int[,] maze;
    
    private Vector2Int startPos;
    public Vector2Int StartPos
    {
        get { return startPos; }
        set {
            if ((value.x >= 0) && (value.y >= 0) && (value.x < height) && (value.y < width))
                startPos = value;
        }
    }
    private Vector2Int exitPos;
    public Vector2Int ExitPos
    {
        get { return exitPos; }
        set
        {
            if ((value.x >= 0) && (value.y >= 0) && (value.x < height) && (value.y < width))
                exitPos = value;
        }
    }

    private static System.Random rand = new System.Random();

    public Maze(int height, int width, Vector2Int startPos) 
    {
        
        this.height = height - (1 - height % 2);
        this.width = width - (1 - width % 2);
        this.startPos = startPos;

        //initialized at 0.
        maze = new int[height, width];

    }

    public Maze(int height, int width)
    {
        this.height = height - (1 - height % 2);
        this.width = width - (1 - width % 2);
        this.startPos = RandomStartPos();

        //initialized at 0.
        maze = new int[height, width];
    }

    /// <summary>
    /// Generate a perfect maze using Recursive Backtracking
    /// </summary>
    public void GenerateWith_RecursiveBacktracking()
    {
        Vector2Int currentPos, nextPos;
        int ind;

        //We reinitiliaze the maze with walls.
        for (int i = 0; i < maze.GetLength(0); i++)
            for (int j = 0; j < maze.GetLength(1); j++)
                maze[i, j] = 1;

        //We set up what we need to choose a random direction
        //System.Random rand = new System.Random();
        List<Vector2Int> directions = new List<Vector2Int>{ Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
        //rand.Next(4);
        Vector2Int randDir = directions[rand.Next(4)];

        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(startPos);

        //Debug.Log("StartPos = " + startPos);

        //While the stack is not empty
        while ( stack.Count > 0 )
        {


            //Peek the top coordinate.
            currentPos = stack.Peek();

            //We carve the current cell
            maze[currentPos.x, currentPos.y] = 0;

            //printMazeLog();
            //Debug.Log("Current pos = " + currentPos.ToString());

            //Dig into a random valid direction :
            //          For that we shuffle the directions array, and iterate through until a direction is valid.
            Shuffle(directions);
            ind = 0;
            nextPos = currentPos + (directions[ind]*2);
            while ( ind < 3 && getCell(nextPos.x, nextPos.y) != 1 )
            {
                ind++;
                //Debug.Log("ind = " + ind);
                nextPos = currentPos + (directions[ind]*2);
            }


            //If we can carve in this direction, we push the coord
            if ( getCell(nextPos.x, nextPos.y) == 1 )
            {

                //Debug.Log("Next pos = " + nextPos.ToString());
                stack.Push(nextPos);
                //We carve the cell between this one and the new one, because wall are 1 cell thick.
                currentPos = currentPos + directions[ind];
                maze[currentPos.x, currentPos.y] = 0;
            }
            else
            {   //We pop the top coordinate
                stack.Pop();
            }

        }

        //We set and exit case
        exitPos = RandomExitPos();

    }

    //Trim the size of the maze so the heigth and widht are odd numbers (better display)
    public void trimSize()
    {
        height -= (1 - height % 2);
        width -= (1 - width % 2);

    }

    private Vector2Int RandomStartPos()
    {
        int x, y;
        x = rand.Next(1, height);
        x -= (1 - x % 2);
        y = rand.Next(1, width);
        y -= (1 - y % 2);

        return new Vector2Int(x, y);

    }

    //Calculate a random possible ExitPos, on the side of the maze
    private Vector2Int RandomExitPos()
    {
        Vector2Int posExit;
        int exitBorder = rand.Next(4);
        switch (exitBorder)
        {
            //top
            case 0:
                posExit = new Vector2Int(0, rand.Next(1, width));
                posExit.y -= (1 - posExit.y % 2);
                break;
            //bottom
            case 1:
                posExit = new Vector2Int(height-1, rand.Next(1, width));
                posExit.y -= (1 - posExit.y % 2);
                break;
            //right
            case 2:
                posExit = new Vector2Int(rand.Next(1, height), width-1);
                posExit.x -= (1 - posExit.x % 2);
                break;
            //left
            case 3:
                posExit = new Vector2Int(rand.Next(1, height), 0);
                posExit.x -= (1 - posExit.x % 2);
                break;
            default:
                posExit = new Vector2Int(0, 1);
                break;
        }
        

        return posExit;

    }


    public bool IsInBound(int x, int y)
    {
        return (x >= 0) && (y >= 0) && (x < height) && (y < width);
    }
    public bool IsInBound(Vector2Int pos)
    {
        return IsInBound(pos.x, pos.y);
    }
    public int getCell(int x, int y)
    {
        if (!IsInBound(x, y))
            return -1;
        return maze[x, y];
    }
    public int getCell(Vector2Int pos)
    {
        return getCell(pos.x, pos.y);
    }

    private void printMazeLog()
    {
        string maze = "";
        for (int i = 0; i < width; i++)
        {
            maze += " -- ";
        }
        maze += "\n";
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (getCell(x, y) == 0)
                    maze += " 0 ";
                else
                    maze += " X ";
            }
            maze += "\n";
        }

        for (int i = 0; i < width; i++)
        {
            maze += " -- ";
        }
        maze += "\n";

        Debug.Log(maze);

    }

    public void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rand.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

}
