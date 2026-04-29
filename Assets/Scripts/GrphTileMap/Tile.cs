using UnityEngine;


public enum Sides
{
    None = -1,
    Top,
    Left,
    Right,
    Bottom,
}
public class Tile
{
    public int id;

    public int Weight => tableWeight[autoTileId + 1];

    public static readonly int[] tableWeight =
    {
        int.MaxValue,
        1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
        2,4, int.MaxValue,1,1,1,
    };
    public Tile[] adjacents = new Tile[4];

    public int autoTileId;

    public int fowTileId;

    public bool isVisited = false;
    public Tile previous = null;
    public bool CanMove => Weight != int.MaxValue;

    public void UpdateFowTileId()
    {
        fowTileId = 0;
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null || !adjacents[i].isVisited)
            {
                fowTileId |= (1 << i);
            }
        }
    }
    public void ClearPreviousTile()
    {
        previous = null;
    }
    public void UpdateAutoTileId()
    {
        autoTileId = 0;
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] != null)
            {
                autoTileId |= (1 << i);
            }
        }
    }

    public void RemoveAdjacents(Tile tile)
    {
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null)
            {
                continue;
            }
            if (adjacents[i].id == tile.id)
            {
                adjacents[i] = null;
                UpdateAutoTileId();
                break;
            }
        }
    }

    public void ClearAdjacents()
    {
        autoTileId = (int)TileTypes.Empty;
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null)
            {
                continue;
            }
            adjacents[i].RemoveAdjacents(this);
            adjacents[i] = null;
        }
        UpdateAutoTileId();
    }
}
