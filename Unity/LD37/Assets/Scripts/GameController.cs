using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LD37.Domain.Cousins;
using LD37.Domain.Rooms;
using System;
using LD37.Domain.Items;

public class GameController : MonoBehaviour {

    public int NumPlayers;
    public Transform playerPrefab;
    public Transform roomPrefab;

    private Dictionary<Room, Transform> rooms = new Dictionary<Room, Transform>();
    private Dictionary<Cousin, Transform> players = new Dictionary<Cousin, Transform>();
    private List<Cousin> cousins;

    private Transform playerContainer;
    private Transform roomContainer;

    private bool initialSpawnCompleted = false;

    void Awake() {
        playerContainer = transform.Find("Players");
        roomContainer = transform.Find("Rooms");

        cousins = CreateCousinsForPlayers();
        CousinsToAddToBuilding cousinsToAdd = new CousinsToAddToBuilding(cousins);
        Building building = new Building(cousinsToAdd, new ItemToSpawnSelector());
        CreateRooms(building);
    }

    void Start() {
    }

    private List<Cousin> CreateCousinsForPlayers() {
        List<Cousin> cousins = new List<Cousin>();
        for (int i = 0; i < NumPlayers; i++) {
            Cousin cousin = Cousin.all[i];
            cousins.Add(cousin);

            CreatePlayer(cousin, i);
        }
        return cousins;
    }

    private void CreatePlayer(Cousin cousin, int playerNumber) {
        Transform player = Instantiate(
            playerPrefab,
            new Vector2(playerNumber * 2, 0),
            Quaternion.identity,
            playerContainer
        );

        PlayerControl playerControl = player.GetComponent<PlayerControl>();
        playerControl.SetCousin(cousin, playerNumber);
        playerControl.rewiredPlayerId = playerNumber;

        players.Add(cousin, player);
    }

    private void CreateRoom(Room room, Vector2 position) {
        Transform roomInstance = Instantiate(
            roomPrefab,
            position,
            Quaternion.identity,
            roomContainer
        );

        roomInstance.GetComponent<TileFloors>().room = room;

        rooms.Add(room, roomInstance);
    }

    private void CreateRooms(Building building) {
        int x = 100;
        int y = 100;
        foreach (Room room in building.RoomList) {
            CreateRoom(room, new Vector2(x, y));

            x += 30;
        }
    }

    void Update () {
		if (!initialSpawnCompleted) {
            foreach (Cousin cousin in cousins) {
                Transform player = players[cousin];
                Transform camera = player.GetComponent<PlayerControl>().camera;
                Transform room = rooms[cousin.SpawnRoom];

                player.position = room.position;
                camera.position = new Vector3(room.position.x, room.position.y, camera.position.z);
            }

            initialSpawnCompleted = true;
        }
	}
}
