using UnityEngine;
using System.Linq;

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
    lake,
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
            tiles[i].UpdateFowAutoTileId();
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
        return true;
    }
}
