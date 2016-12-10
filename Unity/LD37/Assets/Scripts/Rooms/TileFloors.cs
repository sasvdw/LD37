using LD37.Domain.Movement;
using LD37.Domain.Rooms;
using UnityEngine;

// Actually builds the whole room now...
public class TileFloors : MonoBehaviour
{
    private const int width = 18;
    private const int height = 10;
    private readonly int minX;
    private readonly int maxX;
    private readonly int minY;
    private readonly int maxY;

    private Transform tilesParent;
    private Transform wallsParent;
    private bool built;
    public Transform FloorTile;
    public Transform WallTile;

    public Room Room { get; set; }

    public TileFloors()
    {
        this.built = false;

        minX = -width / 2;
        maxX = width / 2 - 1;

        minY = -height / 2;
        maxY = height / 2 - 1;
    }

    public void BuildRoom()
    {
        for(var x = minX; x <= maxX; x++)
        {
            for(var y = minY; y <= maxY; y++)
            {
                if(this.IsWallLocation(x, y) && !this.IsDoorLocation(x, y, this.Room))
                {
                    this.CreateWallTile(x, y);
                }
                else
                {
                    this.CreateFloorTile(x, y);
                }
            }
        }

        this.built = true;
    }

    private bool IsDoorLocation(int x, int y, Room room)
    {
        if((x == this.minX && room.HasDoor(Direction.West)) || (x == this.maxX && room.HasDoor(Direction.East)))
        {
            return y == -1 || y == 0;
        }
        if((y == this.minY && room.HasDoor(Direction.South)) || (y == this.maxY && room.HasDoor(Direction.North)))
        {
            return x == -1 || x == 0;
        }
        return false;
    }

    private void CreateFloorTile(int x, int y)
    {
        Instantiate(
            this.FloorTile,
            new Vector3(this.tilesParent.position.x + x + 0.5f, this.tilesParent.position.y + y + 0.5f, this.tilesParent.position.z),
            Quaternion.identity,
            this.tilesParent
        );
    }

    private void CreateWallTile(int x, int y)
    {
        Instantiate(
            this.WallTile,
            new Vector3(this.wallsParent.position.x + x + 0.5f, this.wallsParent.position.y + y + 0.5f, this.wallsParent.position.z),
            Quaternion.identity,
            this.wallsParent
        );
    }

    private bool IsWallLocation(int x, int y)
    {
        return x == this.minX || x == this.maxX || y == this.minY || y == this.maxY;
    }

    private void Start()
    {
        this.FloorTile.GetComponent<SpriteRenderer>();
        this.tilesParent = transform.Find("Tiles");
        this.wallsParent = transform.Find("Walls");
    }

    private void Update()
    {
        if(!this.built && this.Room != null)
        {
            this.BuildRoom();
        }
    }
}
