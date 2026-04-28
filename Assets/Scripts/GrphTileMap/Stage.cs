using UnityEngine;

public class Stage : MonoBehaviour
{
    public GameObject tilePrefabs;
    private GameObject[] tileObjs;

    public int mapWidth = 20;
    public int mapHeight = 20;


    [Range(0f, 0.9f)]
    public float erodePercent = 0.1f;
    public int erodeIterations = 2;
    [Range(0f, 0.9f)]
    public float lakePercent = 0.1f;
    [Range(0f, 0.9f)]
    public float treePercent = 0.5f;
    [Range(0f, 0.9f)]
    public float hillPercent = 0.5f;
    [Range(0f, 0.9f)]
    public float mountainPercent = 0.5f;
    [Range(0f, 0.9f)]
    public float townPercent = 0.5f;
    [Range(0f, 0.9f)]
    public float monsterPercent = 0.5f;

    private Vector3 FirstTilePos
    {
        get
        {
            float x = -(mapWidth * tileSize.x) / 2f + tileSize.x / 2f;
            float y = (mapHeight * tileSize.y) / 2f - tileSize.y / 2f;
            return new Vector3(x, y, 0);
        }
    }
    private int prevTileId = -1;

    public Vector2 tileSize = new Vector2(16, 16);
    public Sprite[] islandSprites;
    public Sprite[] FowSprites;

    private Map map;
    public Map Map => map;

    public PlayerMovement playerPrefab;
    public PlayerMovement player;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetStage();
        }
        if (tileObjs != null)
        {
            int currentTileId = ScreenPosToTileId(Input.mousePosition);
            if (prevTileId != currentTileId)
            {
                tileObjs[currentTileId].GetComponent<SpriteRenderer>().color = Color.green;
                if (prevTileId >= 0 && prevTileId < tileObjs.Length)
                {
                    tileObjs[prevTileId].GetComponent<SpriteRenderer>().color = Color.white;
                }
                prevTileId = currentTileId;
            }
        }
    }

    private void ResetStage()
    {
        map = new Map();
        map.Init(mapHeight, mapWidth);
        map.CreateIsland(erodePercent, erodeIterations, lakePercent, treePercent, hillPercent, mountainPercent, townPercent, monsterPercent);
        CreateGrid();
        CreatePlayer();// 플레이어 기준으로 내가 설정한 만큼 맵 열기
    }

    private void CreatePlayer()
    {
        if (player != null)
        {
            Destroy(player.gameObject);
        }
        player = Instantiate(playerPrefab);
        player.MoveTo(map.startTile.id);
    }

    private void CreateGrid()
    {
        if (tileObjs != null)
        {
            foreach (var tile in tileObjs)
            {
                Destroy(tile.gameObject);
            }
        }
        tileObjs = new GameObject[mapWidth * mapHeight];

        var position = FirstTilePos;
        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                var tileId = i * mapWidth + j;
                var newGo = Instantiate(tilePrefabs, transform);
                newGo.transform.position = position;
                position.x += tileSize.x;

                tileObjs[tileId] = newGo;
                DecorateTile(tileId);
            }
            position.x = FirstTilePos.x;
            position.y -= tileSize.y;
        }
    }

    public void DecorateTile(int tileId)// 방문안한애들은 fow visted 정보를 가지고 island인지 fow인지
    {
        var tile = map.tiles[tileId];
        var tileGo = tileObjs[tileId];
        var ren = tileGo.GetComponent<SpriteRenderer>();
        if (tile.isVisited)
        {
            if (tile.autoTileId != (int)TileTypes.Empty)
            {
                ren.sprite = islandSprites[tile.autoTileId];
            }
            else
            {
                ren.sprite = null;
            }
        }
        else
        {
            Debug.Log("UpdateFowAutoTileId 호출됨");
            tile.UpdateFowAutoTileId();
            Debug.Log($"tileId:{tileId} fowTileId:{tile.fowTileId}");
            ren.sprite = FowSprites[tile.fowTileId];
        }
    }

    public int ScreenPosToTileId(Vector3 screenPos)
    {
        screenPos.z = Mathf.Abs(transform.position.z - Camera.main.transform.position.z);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return WorldPosToTileId(worldPos);
    }

    public int WorldPosToTileId(Vector3 worldPos)
    {
        var first = FirstTilePos;
        int x = Mathf.FloorToInt((worldPos.x - first.x) / tileSize.x + 0.5f);
        int y = Mathf.FloorToInt((first.y - worldPos.y) / tileSize.y + 0.5f);
        x = Mathf.Clamp(x, 0, mapWidth - 1);
        y = Mathf.Clamp(y, 0, mapHeight - 1);
        return y * mapWidth + x;
    }

    public Vector3 GetTilePos(int y, int x)
    {
        float posX = FirstTilePos.x + x * tileSize.x;
        float posY = FirstTilePos.y - y * tileSize.y;
        return new Vector3(posX, posY, 0);
    }

    public Vector3 GetTilePos(int tileId)
    {
        int row = tileId / mapWidth;
        int col = tileId % mapWidth;
        return GetTilePos(row, col);
    }

}
