using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LD37.Domain.Movement;
using LD37.Domain.Rooms;
using System;

// Actually builds the whole room now...
public class TileFloors : MonoBehaviour {

    public Transform floorTile;
    public Transform wallTile;

    private const int WIDTH = 18;
    private const int HEIGHT = 10;

    private SpriteRenderer spriteRenderer;
    private Transform tilesParent;
    private Transform wallsParent;
    private int minX;
    private int maxX;
    private int minY;
    private int maxY;

    public void BuildRoom(Room room) {
        for (int x = minX; x <= maxX; x++) {
            for (int y = minY; y <= maxY; y++) {
                if (IsWallLocation(x, y) && !IsDoorLocation(x, y, room)) {
                    CreateWallTile(x, y);
                } else {
                    CreateFloorTile(x, y);
                }
            }
        }
    }

    private bool IsDoorLocation(int x, int y, Room room) {
        if ((x == minX && room.HasDoor(Direction.West)) || (x == maxX && room.HasDoor(Direction.East))) {
            return y == -1 || y == 0;
        } else if ((y == minY && room.HasDoor(Direction.South)) || (y == maxY && room.HasDoor(Direction.North))) {
            return x == -1 || x == 0;
        } else {
            return false;
        }
    }

    private void CreateFloorTile(int x, int y) {
        Instantiate(
            floorTile,
            new Vector3(tilesParent.position.x + x + 1.0f, tilesParent.position.y + y + 0.6f, tilesParent.position.z),
            Quaternion.identity,
            tilesParent
        );
    }

    private void CreateWallTile(int x, int y) {
        Instantiate(
            wallTile,
            new Vector3(wallsParent.position.x + x + 1.0f, wallsParent.position.y + y + 0.6f, wallsParent.position.z),
            Quaternion.identity,
            wallsParent
        );
    }

    private bool IsWallLocation(int x, int y) {
        return x == minX || x == maxX || y == minY || y == maxY;
    }

    void Start () {
        spriteRenderer = floorTile.GetComponent<SpriteRenderer>();
        tilesParent = transform.Find("Tiles");
        wallsParent = transform.Find("Walls");

        minX = -WIDTH / 2;
        maxX = WIDTH / 2 - 1;

        minY = -HEIGHT / 2;
        maxY = HEIGHT / 2 - 1;

        Room room = new Room();
        Room room2 = new Room();
        Room room3 = new Room();
        Room room4 = new Room();
        Room room5 = new Room();
        room.ConnectRoom(room2, Direction.North);
        room.ConnectRoom(room3, Direction.East);
        room.ConnectRoom(room4, Direction.South);
        room.ConnectRoom(room5, Direction.West);
        BuildRoom(room);
	}
	
	void Update () {
		
	}
}
