using UnityEngine;

public class TileCell : MonoBehaviour
{
    public Vector2Int coordinates { get; set; }
    public Tile1 tile { get; set; }

    public bool Empty => tile == null;
    public bool Occupied => tile != null;
}
