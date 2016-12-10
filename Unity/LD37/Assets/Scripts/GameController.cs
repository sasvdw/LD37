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

    private Transform playerContainer;
    private Transform roomContainer;

	void Start () {
        playerContainer = transform.Find("Players");
        roomContainer = transform.Find("Rooms");

        List<Cousin> cousins = CreateCousinsForPlayers();
        CousinsToAddToBuilding cousinsToAdd = new CousinsToAddToBuilding(cousins);
        Building building = new Building(cousinsToAdd, new ItemToSpawnSelector());

        int x = 100;
        int y = 100;
        foreach (Room room in building.RoomList) {
            CreateRoom(room, new Vector2(x, y));

            x += 30;
        }
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
    }

    private void CreateRoom(Room room, Vector2 position) {
        Transform roomInstance = Instantiate(
            roomPrefab,
            position,
            Quaternion.identity,
            roomContainer
        );

        roomInstance.GetComponent<TileFloors>().room = room;
    }

    void Update () {
		
	}
}
