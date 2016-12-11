using System;
using System.Collections.Generic;
using System.Linq;
using LD37.Domain.Cousins;
using LD37.Domain.Items;
using LD37.Domain.Rooms;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    private readonly Dictionary<Room, Transform> rooms;
    private readonly Dictionary<Cousin, Transform> players;
    private readonly Dictionary<UnityItem, Item> itemLookup;
    private readonly Dictionary<Item, UnityItem> unityItemLookup;
    private readonly List<Cousin> cousins;
    private readonly Vector3 offScreenPosition;

    private Transform playerContainer;
    private Transform roomContainer;
    private Transform camerasContainer;
    private Transform itemsContainer;

    private bool initialSpawnCompleted = false;

    public Color[] PlayerColors;
    public int NumPlayers;
    public Transform PlayerPrefab;
    public Transform RoomPrefab;
    public Transform PlayerCameraPrefab;
    public Transform BekerPrefab;
    public Transform FistsItemPrefab;
    public Transform PopGunPrefab;
    public float respawnDelay = 3.0f;

    public Building Building { get; private set; }

    public IEnumerable<Cousin> Cousins
    {
        get
        {
            return this.cousins;
        }
    }

    public GameController()
    {
        this.rooms = new Dictionary<Room, Transform>();
        this.players = new Dictionary<Cousin, Transform>();
        this.itemLookup = new Dictionary<UnityItem, Item>();
        this.unityItemLookup = new Dictionary<Item, UnityItem>();
        this.cousins = new List<Cousin>();
        this.offScreenPosition = new Vector3(-1000.0f, -1000.0f, 0f);
    }

    public Item GetDomainItem(UnityItem item)
    {
        return this.itemLookup[item];
    }

    public Color GetCousinColor(Cousin cousin)
    {
        return players[cousin].GetComponent<PlayerControl>().Color;
    }

    private void Awake()
    {
        this.playerContainer = transform.Find("Players");
        this.roomContainer = transform.Find("Rooms");
        this.camerasContainer = transform.Find("Cameras");
        this.itemsContainer = transform.Find("Items");

        this.cousins.AddRange(this.CreateCousinsForPlayers());
        this.Building = new Building(this.cousins, new ItemToSpawnSelector(this.SpawnPopGun));
        this.CreateRooms(Building);
        this.SpawnBeker();

        Room.ItemSpawned += HandleItemSpawned;
    }

    private void HandleItemSpawned(object sender, ItemSpawnedEventArgs e)
    {
        var room = (Room)sender;
        var roomTransform = this.rooms[room];
        var unityItem = this.unityItemLookup[e.Item];

        unityItem.transform.position = roomTransform.transform.position;
    }

    private Item SpawnPopGun()
    {
        var popGunTransform = Instantiate(
            this.PopGunPrefab,
            Vector3.zero,
            Quaternion.identity,
            this.itemsContainer
        );
        var popGunUnity = popGunTransform.GetComponent<UnityItem>();

        var popGun = new PopGun();

        this.itemLookup.Add(popGunUnity, popGun);
        this.unityItemLookup.Add(popGun, popGunUnity);

        return popGun;
    }

    private void SpawnBeker()
    {
        var bekerRoom = this.Building.Rooms.Single(x => x is BekerRoom);

        var bekerRoomTransform = this.rooms[bekerRoom];

        var bekerTransform = Instantiate(
            this.BekerPrefab,
            bekerRoomTransform.position,
            Quaternion.identity,
            this.itemsContainer
        );

        var unityItem = bekerTransform.GetComponent<UnityItem>();
        this.itemLookup.Add(unityItem, Beker.Instance);
        this.unityItemLookup.Add(Beker.Instance, unityItem);
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
            this.playerContainer
        );

        var playerControl = player.GetComponent<PlayerControl>();
        playerControl.SetCousin(cousin, playerNumber);
        playerControl.RewiredPlayerId = playerNumber;
        playerControl.Color = PlayerColors[playerNumber];
        playerControl.Camera = CreateCameraForPlayer(playerNumber);
        playerControl.Fists = Instantiate(
            FistsItemPrefab,
            player.transform.position,
            Quaternion.identity,
            player
        );
        playerControl.CurrentItem = playerControl.Fists;

        players.Add(cousin, player);

        cousin.RoomChanged += this.HandleCousinRoomChanged;
        cousin.Died += this.HandleCousinDied;
        cousin.Respawned += this.HandleCousinRespawned;
        cousin.ItemDropped += this.HandleCousinItemDropped;
        cousin.ItemPickedUp += this.HandleCousinItemPickedUp;
        cousin.ItemDestroyed += this.HandleCousinItemDestroyed;
    }

    private Transform CreateCameraForPlayer(int playerNumber)
    {
        Transform cameraTransform = Instantiate(
            PlayerCameraPrefab,
            new Vector3(0f, 0f, -10.0f),
            Quaternion.identity,
            camerasContainer
        );

        cameraTransform.name = cameraTransform.name + " " + playerNumber;
        Camera camera = cameraTransform.GetComponent<Camera>();
        camera.backgroundColor = PlayerColors[playerNumber];

        if(playerNumber == 0)
        {
            camera.rect = new Rect(0f, 0.5f, 0.5f, 0.5f);
        }
        else if(playerNumber == 1)
        {
            camera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
        }
        else if(playerNumber == 2)
        {
            camera.rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
        }
        else if(playerNumber == 3)
        {
            camera.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
        }

        return cameraTransform;
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
        foreach(var room in building.Rooms)
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

    private void HandleCousinDied(object sender, DiedEventArgs args)
    {
        Transform playerTransform = players[(Cousin)sender];
        playerTransform.position = this.offScreenPosition;

        RespawnManager.Instance.RespawnInSeconds((Cousin)sender, respawnDelay);
    }

    private void HandleCousinItemDropped(object sender, ItemDroppedEventArgs e)
    {
        var cousin = (Cousin)sender;
        var player = this.players[cousin];
        var unityItem = this.unityItemLookup[e.Item];
        var playerControl = player.GetComponent<PlayerControl>();

        if(playerControl.CurrentItem == unityItem.transform)
        {
            playerControl.CurrentItem = playerControl.Fists;
        }

        unityItem.transform.position = player.transform.position;
        unityItem.transform.SetParent(this.itemsContainer);
    }

    private void HandleCousinItemPickedUp(object sender, ItemPickedUpEventArgs e)
    {
        var cousin = (Cousin)sender;
        var player = this.players[cousin];
        var unityItem = this.unityItemLookup[e.Item];
        player.GetComponent<PlayerControl>().CurrentItem = unityItem.transform;

        unityItem.transform.position = this.offScreenPosition;
        unityItem.transform.SetParent(player);
    }

    private void HandleCousinRespawned(object sender, RespawnEventArgs args)
    {
        var cousin = (Cousin)sender;
        var player = players[cousin];
        var camera = player.GetComponent<PlayerControl>().Camera;
        var room = rooms[cousin.SpawnRoom];

        player.position = room.gameObject.transform.position;
        camera.position = new Vector3(room.position.x, room.position.y, camera.position.z);
    }

    private void HandleCousinItemDestroyed(object sender, ItemDestroyedEventArgs args)
    {
        var cousin = (Cousin)sender;
        var player = this.players[cousin];
        var unityItem = this.unityItemLookup[args.Item];
        var playerControl = player.GetComponent<PlayerControl>();
        playerControl.CurrentItem = playerControl.Fists;

        this.unityItemLookup.Remove(args.Item);
        this.itemLookup.Remove(unityItem);

        Destroy(unityItem);
    }

    public void DestroyUnityItem(UnityItem unityItem)
    {
        var itemToDestroy = this.itemLookup[unityItem];

        this.Building.Destroy(itemToDestroy);
    }
}
