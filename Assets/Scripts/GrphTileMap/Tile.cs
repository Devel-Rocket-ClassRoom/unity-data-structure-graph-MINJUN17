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
    public Tile[] adjacents = new Tile[4];

    public int autoTileId;

    public int fowTileId;

    public bool isVisited = false;

    public bool CanMove => autoTileId != (int)TileTypes.Empty;

    public void UpdateFowAutoTileId()
    {
        fowTileId = 0;
        for(int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null || !adjacents[i].isVisited)
            {
                fowTileId |= (1 << i);
            }
        }
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
                UpdateFowAutoTileId();
                break;
            }
        }
    }

    public void ClearAdjacents()
    {
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
        UpdateFowAutoTileId();
    }
}
