using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Actually builds the whole room now...
public class TileFloors : MonoBehaviour {

    public Transform floorTile;
    public Transform wallTile;

    private const int WIDTH = 18;
    private const int HEIGHT = 10;

    private SpriteRenderer spriteRenderer;
    private Transform tilesParent;
    private Transform wallsParent;

    public void BuildRoom() {
        float ppu = spriteRenderer.sprite.pixelsPerUnit;

        int minX = -WIDTH / 2;
        int maxX = WIDTH / 2 - 1;
        int minY = -HEIGHT / 2;
        int maxY = HEIGHT / 2 - 1;
        for (int x = minX; x <= maxX; x++) {
            for (int y = minY; y <= maxY; y++) {
                Instantiate(
                    floorTile,
                    new Vector3(tilesParent.position.x + x + 1.0f, tilesParent.position.y + y + 0.6f, tilesParent.position.z),
                    Quaternion.identity,
                    tilesParent
                );

                if (x == minX || x == maxX || y == minY || y == maxY) {
                    Instantiate(
                        wallTile,
                        new Vector3(wallsParent.position.x + x + 1.0f, wallsParent.position.y + y + 0.6f, wallsParent.position.z),
                        Quaternion.identity,
                        wallsParent
                    );
                }
            }
        }
    }

    void Start () {
        spriteRenderer = floorTile.GetComponent<SpriteRenderer>();
        tilesParent = transform.Find("Tiles");
        wallsParent = transform.Find("Walls");

        BuildRoom();
	}
	
	void Update () {
		
	}
}
