﻿using System;
using System.Collections.Generic;
using System.Linq;
using LD37.Domain.Cousins;
using LD37.Domain.Items;
using LD37.Domain.Rooms;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : Singleton<GameController> {
    private List<GamePlayer> gamePlayers;
    private readonly Dictionary<Room, Transform> rooms;
    private readonly Dictionary<Cousin, Transform> players;
    private readonly Dictionary<Cousin, Transform> playerUIs;
    private readonly Dictionary<UnityItem, Item> itemLookup;
    private readonly Dictionary<Item, UnityItem> unityItemLookup;
    private readonly List<Cousin> cousins;
    private readonly Vector3 offScreenPosition;

    private Transform playerContainer;
    private Transform roomContainer;
    private Transform camerasContainer;
    private Transform itemsContainer;

    private InternalSettings settings;

    private bool initialSpawnCompleted = false;
    private bool running = false;
    public Cousin Winner { get; private set; }

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
        this.gamePlayers = new List<GamePlayer>();
        this.rooms = new Dictionary<Room, Transform>();
        this.players = new Dictionary<Cousin, Transform>();
        this.playerUIs = new Dictionary<Cousin, Transform>();
        this.itemLookup = new Dictionary<UnityItem, Item>();
        this.unityItemLookup = new Dictionary<Item, UnityItem>();
        this.cousins = new List<Cousin>();
        this.offScreenPosition = new Vector3(-1000.0f, -1000.0f, 0f);
    }

    public bool Running {
        get {
            return this.running;
        }
    }

    public Item GetDomainItem(UnityItem item)
    {
        return this.itemLookup[item];
    }

    public Color GetCousinColor(Cousin cousin)
    {
        return players[cousin].GetComponent<PlayerControl>().Color;
    }

    public void Reset() {
        this.gamePlayers = new List<GamePlayer>();
        this.rooms.Clear();
        this.players.Clear();
        this.playerUIs.Clear();
        this.itemLookup.Clear();
        this.unityItemLookup.Clear();
        this.cousins.Clear();

        Cousin.All.Reset();

        DestroyChildren(this.playerContainer);
        DestroyChildren(this.roomContainer);
        DestroyChildren(this.camerasContainer);
        DestroyChildren(this.itemsContainer);

        this.running = false;
        this.initialSpawnCompleted = false;
    }

    private void DestroyChildren(Transform transform) {
        for (int i = 0; i < transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void Awake()
    {
        
        this.playerContainer = CreateGameObjectFolder("Players").transform;
        this.roomContainer = CreateGameObjectFolder("Rooms").transform;
        this.camerasContainer = CreateGameObjectFolder("Cameras").transform;
        this.itemsContainer = CreateGameObjectFolder("Items").transform;

        settings = GameObject.Find("InternalSettings").GetComponent<InternalSettings>();

        Room.ItemSpawned += HandleItemSpawned;
    }

    private GameObject CreateGameObjectFolder(String name) {
        GameObject obj = new GameObject();
        obj.name = name;
        obj.transform.parent = this.transform;
        return obj;
    }

    public void AddGamePlayer(Cousin cousin, Rewired.Player rewiredPlayer) {
        this.gamePlayers.Add(new GamePlayer(cousin, rewiredPlayer));
    }

    public void BeginGame() {
        SetupPlayers();
        this.Building = new Building(this.cousins, new ItemToSpawnSelector(this.SpawnPopGun));
        this.CreateRooms(Building);
        this.SpawnBeker();

        this.running = true;
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
            settings.PopGunPrefab,
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
            settings.BekerPrefab,
            bekerRoomTransform.position,
            Quaternion.identity,
            this.itemsContainer
        );

        var unityItem = bekerTransform.GetComponent<UnityItem>();
        this.itemLookup.Add(unityItem, Beker.Instance);
        this.unityItemLookup.Add(Beker.Instance, unityItem);
    }

    private void Start() {}

    private void SetupPlayers() {
        for (int i = 0; i < settings.NumPlayers; i++) {
            Cousin cousin = null;
            int rewiredPlayerId = -1;

            if (i < gamePlayers.Count) {
                GamePlayer gamePlayer = gamePlayers[i];
                cousin = gamePlayer.cousin;
                rewiredPlayerId = gamePlayer.rewiredPlayer.id;
            } else {
                // Make random player
                // TODO: Make it an AI player

                cousin = GetUnusedCousin();
                rewiredPlayerId = i;
            }

            this.CreatePlayer(cousin, rewiredPlayerId, i);
        }
        
    }

    private Cousin GetUnusedCousin() {
        return Cousin.All.First<Cousin>(x => !cousins.Contains(x));
    }

    private void CreatePlayer(Cousin cousin, int rewiredPlayerId, int playerNumber)
    {
        this.cousins.Add(cousin);

        var player = Instantiate(
            settings.PlayerPrefab,
            new Vector2(playerNumber * 2, 0),
            Quaternion.identity,
            this.playerContainer
        );

        var playerControl = player.GetComponent<PlayerControl>();
        playerControl.SetCousin(cousin, playerNumber);
        playerControl.RewiredPlayerId = rewiredPlayerId;
        playerControl.Color = settings.PlayerColors[playerNumber];
        playerControl.Camera = CreateCameraForPlayer(playerNumber);
        playerControl.Fists = Instantiate(
            settings.FistsItemPrefab,
            player.transform.position,
            Quaternion.identity,
            player
        );
        playerControl.CurrentItem = playerControl.Fists;

        players.Add(cousin, player);

        Transform playerUI = GameObject.Find("ScorePanel" + playerNumber).transform;
        Transform uiNameText = playerUI.FindChild("NameText");
        uiNameText.GetComponentInChildren<Text>().text = cousin.Name;
        playerUIs.Add(cousin, playerUI);

        // Setup player caricature on UI
        int indexOfCaricature = Array.IndexOf(settings.cousinNames, cousin.Name);
        if (indexOfCaricature >= 0 && indexOfCaricature < settings.cousinCaricature.Count()) {
            playerUI.FindChild("CaricatureImage").GetComponent<Image>().sprite = settings.cousinCaricature[indexOfCaricature];
        }

        // Add event handlers
        cousin.RoomChanged += this.HandleCousinRoomChanged;
        cousin.Died += this.HandleCousinDied;
        cousin.Respawned += this.HandleCousinRespawned;
        cousin.ItemDropped += this.HandleCousinItemDropped;
        cousin.ItemPickedUp += this.HandleCousinItemPickedUp;
        cousin.ItemDestroyed += this.HandleCousinItemDestroyed;
        cousin.CousinScored += this.HandleCousinScored;
        cousin.ScoreChanged += this.HandleCousinScoreChanged;
        cousin.CousinHealthChanged += this.HandleCousinHealthChanged;
    }
    
    private void HandleCousinScoreChanged(object sender, CousinScoreChangeEventArgs e)
    {
        var cousin = (Cousin)sender;
        var playerUI = this.playerUIs[cousin];
        var scoreText = playerUI.FindChild("ScoreText");
        var text = scoreText.GetComponentInChildren<Text>();
        text.text = e.NewScore.ToString();
    }

    private Transform CreateCameraForPlayer(int playerNumber)
    {
        Transform cameraTransform = Instantiate(
            settings.PlayerCameraPrefab,
            new Vector3(0f, 0f, -10.0f),
            Quaternion.identity,
            camerasContainer
        );

        cameraTransform.name = cameraTransform.name + " " + playerNumber;
        Camera camera = cameraTransform.GetComponent<Camera>();
        camera.backgroundColor = settings.PlayerColors[playerNumber];

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
            settings.RoomPrefab,
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
        if (Input.GetKey(KeyCode.Escape)) {
            Application.Quit();
        }

        if (!this.Running) {
            return;
        }

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

        RespawnManager.Instance.RespawnInSeconds((Cousin)sender, settings.respawnDelay);
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

    private void HandleCousinScored(object sender, CousinScoredEventArgs e)
    {
        var cousin = (Cousin)sender;
        var player = this.players[cousin];
        var unityItem = this.unityItemLookup[e.Beker];
        var playerControl = player.GetComponent<PlayerControl>();

        if (e.Won) {
            this.Winner = cousin;
            SceneManager.LoadScene("Victory");
            return;
        }
        
        if (playerControl.CurrentItem == unityItem.transform)
        {
            playerControl.CurrentItem = playerControl.Fists;
        }

        var bekerRoom = this.Building.BekerRoom;
        var bekerRoomTransform = this.rooms[bekerRoom];

        unityItem.transform.position = bekerRoomTransform.transform.position;
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

    private void HandleCousinHealthChanged(object sender, CousinHealthChangedEventArgs e) {
        var cousin = (Cousin)sender;
        var playerUI = playerUIs[cousin];

        for (int i = 3; i > 0; i--) {
            GameObject heartObject = playerUI.transform.FindChild("Heart" + i).gameObject;
            if (i <= e.Health) {
                heartObject.SetActive(true);
            } else {
                heartObject.SetActive(false);
            }
        }
    }

    public void DestroyUnityItem(UnityItem unityItem)
    {
        var itemToDestroy = this.itemLookup[unityItem];

        this.Building.Destroy(itemToDestroy);
    }

    private class GamePlayer {
        public Cousin cousin { get; private set; }
        public Rewired.Player rewiredPlayer { get; private set; }

        public GamePlayer(Cousin cousin, Rewired.Player rewiredPlayer) {
            this.cousin = cousin;
            this.rewiredPlayer = rewiredPlayer;
        }
    }
}
