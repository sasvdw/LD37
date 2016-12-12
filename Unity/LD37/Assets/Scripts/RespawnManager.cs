using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LD37.Domain.Cousins;

public class RespawnManager : Singleton<RespawnManager> {

    List<DeadCousin> respawned = new List<DeadCousin>();
    private List<DeadCousin> deadCousins = new List<DeadCousin>();

    public void RespawnInSeconds(Cousin deadCousin, float timeSeconds) {
        deadCousins.Add(new DeadCousin(deadCousin, timeSeconds));
    }

    public void Update() {
        
        foreach (DeadCousin deadCousin in deadCousins) {
            deadCousin.timeToRespawn -= Time.deltaTime;

            if (deadCousin.ShouldRespawn) {
                deadCousin.cousin.Respawn();
                respawned.Add(deadCousin);
            }
        }

        deadCousins.RemoveAll(respawned.Contains);
        respawned.Clear();
    }

    private class DeadCousin {
        public Cousin cousin;
        public float timeToRespawn;

        public DeadCousin(Cousin cousin, float timeToRespawn) {
            this.cousin = cousin;
            this.timeToRespawn = timeToRespawn;
        }

        public bool ShouldRespawn {
            get {
                return timeToRespawn <= 0.0f;
            }
        }
    }
}