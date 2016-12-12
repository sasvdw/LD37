using LD37.Domain.Rooms;
using UnityEngine;

public class ItemSpawnManager : Singleton<ItemSpawnManager>
{
    private InternalSettings settings;

    private Building building
    {
        get
        {
            return GameController.Instance.Building;
        }
    }

    private float respawnTime
    {
        get
        {
            return settings.respawnDelay;
        }
    }

    private float timeBeforeNextSpawn;

    private void Start() {
        settings = GameObject.Find("InternalSettings").GetComponent<InternalSettings>();
    }

    private void Update()
    {
        if (!GameController.Instance.Running) {
            return;
        }

        this.timeBeforeNextSpawn -= Time.deltaTime;

        if(this.timeBeforeNextSpawn > 0.0f)
        {
            return;
        }

        this.building.SpawnItem();
        this.timeBeforeNextSpawn = this.respawnTime;
    }
}