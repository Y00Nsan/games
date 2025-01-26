using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    [SerializeField] private Tile1 tilePrefab;
    [SerializeField] private TileState[] tileStates;

    private TileGrid grid;
    private List<Tile1> tiles;
    private bool waiting;

    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private bool isSwipe = false;

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile1>(16);
    }

    public void ClearBoard()
    {
        foreach (var cell in grid.cells) {
            cell.tile = null;
        }

        foreach (var tile in tiles) {
            Destroy(tile.gameObject);
        }

        tiles.Clear();
    }

    public void CreateTile()
    {
        Tile1 tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[0]);
        tile.Spawn(grid.GetRandomEmptyCell());
        tiles.Add(tile);
    }

    private void Update()
{
    if (!waiting)
    {
        if (Input.GetMouseButtonDown(0)) // 왼쪽 마우스 버튼 또는 터치를 감지합니다
        {
            touchStartPos = Input.mousePosition;
            isSwipe = true;
        }
        else if (Input.GetMouseButton(0)) // 왼쪽 마우스 버튼 또는 터치가 눌린 상태를 지속적으로 감지합니다
        {
            if (isSwipe)
            {
                touchEndPos = Input.mousePosition;
                Vector2 direction = touchEndPos - touchStartPos;

                if (direction.magnitude > 50) // 조정 가능한 값 (감도)
                {
                    direction.Normalize();

                    if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                    {
                        if (direction.x > 0)
                        {
                            Move(Vector2Int.right, grid.Width - 2, -1, 0, 1);
                        }
                        else
                        {
                            Move(Vector2Int.left, 1, 1, 0, 1);
                        }
                    }
                    else
                    {
                        if (direction.y > 0)
                        {
                            Move(Vector2Int.up, 0, 1, 1, 1);
                        }
                        else
                        {
                            Move(Vector2Int.down, 0, 1, grid.Height - 2, -1);
                        }
                    }

                    isSwipe = false;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0)) // 왼쪽 마우스 버튼 또는 터치가 해제되었음을 감지합니다
        {
            isSwipe = false;
        }
    }
}


    private void Move(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;

        for (int x = startX; x >= 0 && x < grid.Width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.Height; y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);

                if (cell.Occupied) {
                    changed |= MoveTile(cell.tile, direction);
                }
            }
        }

        if (changed) {
            StartCoroutine(WaitForChanges());
        }
    }

    private bool MoveTile(Tile1 tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null)
        {
            if (adjacent.Occupied)
            {
                if (CanMerge(tile, adjacent.tile))
                {
                    MergeTiles(tile, adjacent.tile);
                    return true;
                }

                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }

        return false;
    }

    private bool CanMerge(Tile1 a, Tile1 b)
    {
        return a.state == b.state && !b.locked;
    }

    private void MergeTiles(Tile1 a, Tile1 b)
    {
        tiles.Remove(a);
        a.Merge(b.cell);

        int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
        TileState newState = tileStates[index];

        b.SetState(newState);
        GameManager1.Instance.IncreaseScore(newState.number);
    }

    private int IndexOf(TileState state)
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            if (state == tileStates[i]) {
                return i;
            }
        }

        return -1;
    }

    private IEnumerator WaitForChanges()
    {
        waiting = true;

        yield return new WaitForSeconds(0.1f);

        waiting = false;

        foreach (var tile in tiles) {
            tile.locked = false;
        }

        if (tiles.Count != grid.Size) {
            CreateTile();
        }

        if (CheckForGameOver()) {
            GameManager1.Instance.GameOver();
        }
    }

    public bool CheckForGameOver()
    {
        if (tiles.Count != grid.Size) {
            return false;
        }

        foreach (var tile in tiles)
        {
            TileCell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if (up != null && CanMerge(tile, up.tile)) {
                return false;
            }

            if (down != null && CanMerge(tile, down.tile)) {
                return false;
            }

            if (left != null && CanMerge(tile, left.tile)) {
                return false;
            }

            if (right != null && CanMerge(tile, right.tile)) {
                return false;
            }
        }

        return true;
    }
}
