using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour {

	void Start () {
        GameController gameController = GameController.Instance;
        ItemSpawnManager itemSpawnManager = ItemSpawnManager.Instance;
        RespawnManager respawnManager = RespawnManager.Instance;

        gameController.BeginGame();
	}
	
	void Update () {
		
	}
}
