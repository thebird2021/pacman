using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Flags]
public enum WallState
{
    // 0000 -> NO WALLS
    // 1111 -> LEFT,RIGHT,UP,DOWN
    LEFT = 1, // 0001
    RIGHT = 2, // 0010
    UP = 4, // 0100
    DOWN = 8, // 1000

    VISITED = 128, // 1000 0000
}


public struct Neighbour
{
    public Vector2 Position;
    public WallState SharedWall;
}

public static class MazeGenerator
{

    private static WallState GetOppositeWall(WallState wall)
    {
        switch (wall)
        {
            case WallState.RIGHT: return WallState.LEFT;
            case WallState.LEFT: return WallState.RIGHT;
            case WallState.UP: return WallState.DOWN;
            case WallState.DOWN: return WallState.UP;
            default: return WallState.LEFT;
        }
    }

    private static WallState[,] ApplyRecursiveBacktracker(WallState[,] maze, int width, int height, int seed)
    {
        var rng = new System.Random();
        if (seed > 0)
        {
            rng = new System.Random(seed);
        }
        var positionStack = new Stack<Vector2>();
        var position = new Vector2(rng.Next(0, width), rng.Next(0, height));

        maze[(int)position.x, (int)position.y] |= WallState.VISITED;
        positionStack.Push(position);

        while (positionStack.Count > 0)
        {
            var current = positionStack.Pop();
            var neighbours = GetUnvisitedNeighbours(current, maze, width, height);

            if (neighbours.Count > 0)
            {
                positionStack.Push(current);

                var randIndex = rng.Next(0, neighbours.Count);
                var randomNeighbour = neighbours[randIndex];

                var nPosition = randomNeighbour.Position;
                maze[(int)current.x, (int)current.y] &= ~randomNeighbour.SharedWall;
                maze[(int)nPosition.x, (int)nPosition.y] &= ~GetOppositeWall(randomNeighbour.SharedWall);
                maze[(int)nPosition.x, (int)nPosition.y] |= WallState.VISITED;

                positionStack.Push(nPosition);
            }
        }

        return maze;
    }

    private static List<Neighbour> GetUnvisitedNeighbours(Vector2 p, WallState[,] maze, int width, int height)
    {
        var list = new List<Neighbour>();

        if (p.x > 0) // LEFT
        {
            if (!maze[(int)p.x - 1, (int)p.y].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Vector2(p.x - 1, p.y),
                    SharedWall = WallState.LEFT
                });
            }
        }

        if (p.y > 0) // DOWN
        {
            if (!maze[(int)p.x, (int)p.y - 1].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Vector2(p.x, p.y - 1),
                    SharedWall = WallState.DOWN
                });
            }
        }

        if (p.y < height - 1) // UP
        {
            if (!maze[(int)p.x, (int)p.y + 1].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Vector2(p.x, p.y + 1),
                    SharedWall = WallState.UP
                });
            }
        }

        if (p.x < width - 1) // RIGHT
        {
            if (!maze[(int)p.x + 1, (int)p.y].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Vector2(p.x + 1, p.y),
                    SharedWall = WallState.RIGHT
                });
            }
        }

        return list;
    }

    public static WallState[,] Generate(int width, int height, int seed, float fullness)
    {
        WallState[,] maze = new WallState[width, height];
        WallState initial = WallState.RIGHT | WallState.LEFT | WallState.UP | WallState.DOWN;
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                maze[i, j] = initial;
            }
        }
        maze = ApplyRecursiveBacktracker(maze, width, height, seed);



        var kill_count = Mathf.Round((width * height / 100) * (100 - fullness));
        Debug.Log(kill_count);
        var rng = new System.Random();
        if (seed > 0)
        {
            rng = new System.Random(seed);
        }
        while (kill_count > 0)
        {

            var p = new Vector2(rng.Next(0, width), rng.Next(0, height));
            Debug.Log("Pos");
            Debug.Log(p);
            var current = maze[(int)p.x, (int)p.y];
            var neighbours = new List<Neighbour>();

            if (current.HasFlag(WallState.UP) && p.y < height - 1)
            {
                neighbours.Add(new Neighbour
                {
                    Position = new Vector2(p.x, p.y + 1),
                    SharedWall = WallState.UP
                });
            }
            if (current.HasFlag(WallState.DOWN) && p.y > 0)
            {
                neighbours.Add(new Neighbour
                {
                    Position = new Vector2(p.x, p.y - 1),
                    SharedWall = WallState.DOWN
                });
            }
            if (current.HasFlag(WallState.LEFT) && p.x > 0)
            {
                neighbours.Add(new Neighbour
                {
                    Position = new Vector2(p.x - 1, p.y),
                    SharedWall = WallState.LEFT
                });
            }
            if (current.HasFlag(WallState.RIGHT) && p.x < width - 1)
            {
                neighbours.Add(new Neighbour
                {
                    Position = new Vector2(p.x + 1, p.y),
                    SharedWall = WallState.RIGHT
                });
            }
            if (neighbours.Count > 0)
            {
                var neighbour = neighbours[rng.Next(0, neighbours.Count)];
                Debug.Log(neighbour.Position);
                Debug.Log(neighbour.SharedWall);
                maze[(int)p.x, (int)p.y] &= ~neighbour.SharedWall;
                maze[(int)neighbour.Position.x, (int)neighbour.Position.y] &= ~GetOppositeWall(neighbour.SharedWall);
                kill_count--;

            }
        }

        return maze;
    }
}
