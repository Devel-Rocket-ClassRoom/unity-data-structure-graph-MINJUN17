using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public enum TileTypes
{
    Empty = -1,
    Grass = 15,
    Tree,
    Hills,
    Moutains,
    Towns,
    Castle,
    Monster,
}
public class Map
{
    public int rows = 0;
    public int cols = 0;
    public Tile[] tiles;

    public Tile[] CoastTiles => tiles.Where(t => t.autoTileId >= 0 && t.autoTileId < (int)TileTypes.Grass).ToArray();
    public Tile[] LandTile => tiles.Where(t => t.autoTileId == (int)TileTypes.Grass).ToArray();


    public Tile startTile;
    public Tile castleTile;


    public void Init(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;

        tiles = new Tile[rows * cols];
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = new Tile();
            tiles[i].id = i;
        }

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int index = r * cols + c;
                var adjacents = tiles[index].adjacents;
                if (r - 1 >= 0)
                {
                    adjacents[(int)Sides.Top] = tiles[index - cols];
                }
                if (c + 1 < cols)
                {
                    adjacents[(int)Sides.Right] = tiles[index + 1];
                }
                if (c - 1 >= 0)
                {
                    adjacents[(int)Sides.Left] = tiles[index - 1];
                }
                if (r + 1 < rows)
                {
                    adjacents[(int)Sides.Bottom] = tiles[index + cols];
                }
            }
        }
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].UpdateAutoTileId();
            tiles[i].UpdateFowTileId();
        }
    }
    
    public void ShuffleTiles(Tile[] tiles)
    {
        for (int i = tiles.Length - 1; i >= 0; --i)
        {
            int rand = Random.Range(0, i + 1);
            (tiles[rand], tiles[i]) = (tiles[i], tiles[rand]);
        }
    }
    public void DecorateTiles(Tile[] tiles, float percent, TileTypes tileType)
    {
        ShuffleTiles(tiles);
        int total = Mathf.FloorToInt(tiles.Length * percent);
        for (int i = 0; i < total; ++i)
        {
            if (tileType == TileTypes.Empty)
            {
                tiles[i].ClearAdjacents();
            }
            tiles[i].autoTileId = (int)tileType;
        }
    }
    
    public bool CreateIsland(float erodePercent,
        int erodeIterations,
        float lakePercent,
        float treePercent,
        float hillPercent,
        float mountainPercent,
        float townPercent,
        float monsterPercent)
    {
        for (int i = 0; i < erodeIterations; i++)
        {
            DecorateTiles(CoastTiles, erodePercent, TileTypes.Empty);
        }
        DecorateTiles(LandTile, lakePercent, TileTypes.Empty);
        DecorateTiles(LandTile, treePercent, TileTypes.Tree);
        DecorateTiles(LandTile, hillPercent, TileTypes.Hills);
        DecorateTiles(LandTile, mountainPercent, TileTypes.Moutains);
        DecorateTiles(LandTile, townPercent, TileTypes.Towns);
        DecorateTiles(LandTile, monsterPercent, TileTypes.Monster);
        var towns = tiles.Where(x => x.autoTileId == (int)TileTypes.Towns).ToArray();
        ShuffleTiles(towns);

        startTile = towns[0];
        castleTile = towns[1];
        castleTile.autoTileId = (int)TileTypes.Castle;
        
        var path = PathFindingAStar(startTile, castleTile);
        return path.Count > 0;
    }
    public List<Tile> PathFindingAStar(int startTile, int endTile)
    {
        return PathFindingAStar(tiles[startTile], tiles[endTile]);
    }

    public List<Tile> PathFindingAStar(Tile startTile, Tile endTile)
    {
        List<Tile> path = new List<Tile>();
        path.Clear();
        foreach(Tile tile in tiles)
        {
            tile.ClearPreviousTile();
        }
        var visited = new HashSet<Tile>();
        var pqueue = new PriorityQueue<Tile, int>();
        var distances = new int[tiles.Length];

        for (int i = 0; i < tiles.Length; i++)
        {
            distances[i] = int.MaxValue;
        }

        distances[startTile.id] = 0;
        pqueue.Enqueue(startTile, Heuristic(startTile, endTile));

        while (pqueue.Count > 0)
        {
            var current = pqueue.Dequeue();
            if (visited.Contains(current))
            {
                continue;
            }
            visited.Add(current);
            if (current == endTile)
            {
                break;
            }

            for (int i = 0; i < current.adjacents.Length; i++)
            {
                var adj = current.adjacents[i];
                if (adj == null || !adj.CanMove || visited.Contains(adj))
                {
                    continue;
                }
                if (distances[current.id] == int.MaxValue)
                {
                    continue;
                }
                int newDist = distances[current.id] + adj.Weight;

                if (newDist < distances[adj.id])
                {
                    tiles[adj.id].previous = current;
                    distances[adj.id] = newDist;
                    pqueue.Enqueue(adj, newDist + Heuristic(adj, endTile));
                }
            }
        }
        var temp = endTile;
        while (temp != null)
        {
            path.Add(temp);
            temp = tiles[temp.id].previous;
        }
        path.Reverse();
        return path;
    }

    private int Heuristic(Tile a, Tile b)
    {
        int ax = a.id % cols;
        int ay = a.id / cols;
        int bx = b.id % cols;
        int by = b.id / cols;
        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by); // 맨하탄 거리
    }
}
