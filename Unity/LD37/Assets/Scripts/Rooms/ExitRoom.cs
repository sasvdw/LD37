using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LD37.Domain.Cousins;
using LD37.Domain.Movement;

public class ExitRoom : MonoBehaviour {

    public DirectionEnum direction = DirectionEnum.North;

    void OnTriggerEnter2D(Collider2D other) {
        PlayerControl playerController = other.gameObject.GetComponent<PlayerControl>();
        if (playerController != null) {
            Debug.Log("Moving " + direction);

            Cousin cousin = playerController.cousin;
            cousin.Move(Direction.GetDirection(direction));
        }
    }

    void Start () {
	}
	
	void Update () {
	}
}
