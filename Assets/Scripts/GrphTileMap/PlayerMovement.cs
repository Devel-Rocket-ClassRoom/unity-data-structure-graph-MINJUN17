using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Stage stage;
    private Animator animator;

    private int currentTileId; // 올라와 있는 타일

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.speed = 0f;

        var findGo = GameObject.FindWithTag("Map");
        stage = findGo.GetComponent<Stage>();
    }

    private void Update()
    {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");

        var direction = Sides.None;
        if (Input.GetKeyDown(KeyCode.W))
        {
            direction = Sides.Top;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            direction = Sides.Bottom;

        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            direction = Sides.Right;

        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            direction = Sides.Left;
        }
        if (direction != Sides.None)
        {
            var targetTile = stage.Map.tiles[currentTileId].adjacents[(int)direction];
            if (targetTile != null && targetTile.CanMove)
            {
                MoveTo(targetTile.id);
            }
        }


    }

    public void MoveTo(int tileId)
    {
        currentTileId = tileId;
        transform.position = stage.GetTilePos(currentTileId);
        RevealTiles(tileId, 3);
    }
    private void RevealTiles(int centerTileId, int range)
    {
        int centerRow = centerTileId / stage.mapWidth;
        int centerCol = centerTileId % stage.mapWidth;

        for (int r = centerRow - range; r <= centerRow + range; r++)
        {
            for (int c = centerCol - range; c <= centerCol + range; c++)
            {
                if (r < 0 || r >= stage.mapHeight || c < 0 || c >= stage.mapWidth) continue;

                int tileId = r * stage.mapWidth + c;
                var tile = stage.Map.tiles[tileId];
                tile.isVisited = true;
                stage.DecorateTile(tileId);
            }
        }
    }

}
