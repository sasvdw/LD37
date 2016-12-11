using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LD37.Domain.Rooms;
using System.Drawing;

public class MiniMap : MonoBehaviour {

    public Transform miniMapRoomPrefab;

    private GameController gameController;
    private Dictionary<Room, Transform> miniMapTiles = new Dictionary<Room, Transform>();
    private bool initialised = false;

	void Start () {
        gameController = GameController.Instance;
	}
	
	void Update () {
        if (initialised) {
            return;
        }

        if (gameController.Building == null) {
            return;
        }

        Building building = gameController.Building;
        foreach (Point point in building.rooms.Keys) {
            Room room = building.rooms[point];
            Transform tileTransform = Instantiate(
                miniMapRoomPrefab,
                point.ToVector2() * 0.25f,
                Quaternion.identity,
                this.transform
            );

            miniMapTiles.Add(room, tileTransform);


        }

        initialised = true;
	}
}
