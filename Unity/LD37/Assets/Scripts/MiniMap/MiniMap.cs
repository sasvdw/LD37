using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LD37.Domain.Cousins;
using LD37.Domain.Rooms;
using System.Drawing;
using System.Linq;

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

        foreach (Cousin cousin in gameController.Cousins) {
            UnityEngine.Color color = gameController.GetCousinColor(cousin);
            miniMapTiles[cousin.SpawnRoom].GetComponent<MiniMapTile>().emptyColor =
                new UnityEngine.Color(color.r * 0.5f, color.g * 0.5f, color.b * 0.5f, 1.0f);

            
            miniMapTiles[cousin.SpawnRoom].GetComponent<MiniMapTile>().SetOccupied(gameController.GetCousinColor(cousin));

            cousin.RoomChanged += HandleCousinRoomChanged;
        }

        initialised = true;
	}

    private void HandleCousinRoomChanged(object sender, RoomChangedEventArgs args) {
        var cousin = args.Cousin;
        var oldRoomTile = miniMapTiles[args.OldRoom].GetComponent<MiniMapTile>();

        if (args.OldRoom.Cousins.Any()) {
            Cousin remainingCousin = args.OldRoom.Cousins.First<Cousin>();
            oldRoomTile.SetOccupied(gameController.GetCousinColor(remainingCousin));
        } else {
            oldRoomTile.SetUnoccupied();
        }

        var newRoomTile = miniMapTiles[args.NewRoom].GetComponent<MiniMapTile>();
        newRoomTile.SetOccupied(gameController.GetCousinColor(cousin));
    }
}
