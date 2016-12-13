using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScene : MonoBehaviour {

	void Start () {
        GameController gameController = GameController.Instance;
        ItemSpawnManager itemSpawnManager = ItemSpawnManager.Instance;
        RespawnManager respawnManager = RespawnManager.Instance;
	}
	
	void Update () {
		
	}
}
