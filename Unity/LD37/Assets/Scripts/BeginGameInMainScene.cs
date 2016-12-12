using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginGameInMainScene : MonoBehaviour {

    // Should only be added to the main scene and not on any prefabs
	void Start () {
        GameController.Instance.BeginGame();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
