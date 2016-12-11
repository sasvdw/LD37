using LD37.Domain.Rooms;
using UnityEngine;

public class ItemSpawnManager : Singleton<ItemSpawnManager>
{
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
            return GameController.Instance.respawnDelay;
        }
    }

    private float timeBeforeNextSpawn;

    private void Update()
    {
        this.timeBeforeNextSpawn -= Time.deltaTime;

        if(this.timeBeforeNextSpawn > 0.0f)
        {
            return;
        }

        this.building.SpawnItem();
        this.timeBeforeNextSpawn = this.respawnTime;
    }
}