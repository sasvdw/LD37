using System.Collections.Generic;
using System.Linq;
using LD37.Domain.Cousins;
using LD37.Domain.Items;
using LD37.Domain.Rooms;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private readonly Dictionary<Room, Transform> rooms;
    private readonly Dictionary<Cousin, Transform> players;
    private readonly List<Cousin> cousins;

    private Transform playerContainer;
    private Transform roomContainer;

    private bool initialSpawnCompleted = false;

    public int NumPlayers;
    public Transform PlayerPrefab;
    public Transform RoomPrefab;

    public GameController()
    {
        this.rooms = new Dictionary<Room, Transform>();
        this.players = new Dictionary<Cousin, Transform>();
        this.cousins = new List<Cousin>();
    }

    private void Awake()
    {
        this.playerContainer = transform.Find("Players");
        this.roomContainer = transform.Find("Rooms");

        this.cousins.AddRange(this.CreateCousinsForPlayers());
        var building = new Building(this.cousins, new ItemToSpawnSelector());
        this.CreateRooms(building);
    }

    private void Start() {}

    private IEnumerable<Cousin> CreateCousinsForPlayers()
    {
        var cousinsForPlayers = new List<Cousin>();
        var allCousins = Cousin.All.ToList();
        for(var i = 0; i < NumPlayers; i++)
        {
            var cousin = allCousins[i];
            cousinsForPlayers.Add(cousin);

            this.CreatePlayer(cousin, i);
        }
        return cousinsForPlayers;
    }

    private void CreatePlayer(Cousin cousin, int playerNumber)
    {
        var player = Instantiate(
            this.PlayerPrefab,
            new Vector2(playerNumber * 2, 0),
            Quaternion.identity,
            playerContainer
        );

        var playerControl = player.GetComponent<PlayerControl>();
        playerControl.SetCousin(cousin, playerNumber);
        playerControl.RewiredPlayerId = playerNumber;

        players.Add(cousin, player);

        cousin.RoomChanged += this.HandleCousinRoomChanged;
    }

    private void CreateRoom(Room room, Vector2 position)
    {
        var roomInstance = Instantiate(
            this.RoomPrefab,
            position,
            Quaternion.identity,
            roomContainer
        );

        roomInstance.GetComponent<TileFloors>().Room = room;

        this.rooms.Add(room, roomInstance);
    }

    private void CreateRooms(Building building)
    {
        var x = 100;
        var y = 100;
        foreach(var room in building.RoomList)
        {
            this.CreateRoom(room, new Vector2(x, y));

            x += 30;
        }
    }

    private void Update()
    {
        if(this.initialSpawnCompleted)
        {
            return;
        }

        foreach(var cousin in this.cousins)
        {
            var player = this.players[cousin];
            var camera = player.GetComponent<PlayerControl>().Camera;
            var room = this.rooms[cousin.SpawnRoom];

            player.position = room.position;
            camera.position = new Vector3(room.position.x, room.position.y, camera.position.z);
        }

        this.initialSpawnCompleted = true;
    }

    private void HandleCousinRoomChanged(object sender, RoomChangedEventArgs args)
    {
        var cousin = args.Cousin;
        var player = players[cousin];
        var camera = player.GetComponent<PlayerControl>().Camera;
        var room = rooms[cousin.CurrentRoom];

        var entrance = room
            .GetComponentsInChildren<EnterRoom>()
            .Single(x => x.Direction == args.SpawnDirection.DirectionEnum);

        player.position = entrance.gameObject.transform.position;
        camera.position = new Vector3(room.position.x, room.position.y, camera.position.z);
    }
}
