using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LD37.Domain.Movement;

public class ExitRoom : MonoBehaviour {

    public DirectionEnum direction = DirectionEnum.North;

    void OnTriggerEnter2D(Collider2D other) {
        PlayerControl playerController = other.gameObject.GetComponent<PlayerControl>();
        if (playerController != null) {
            Debug.Log("Moving " + direction);
        }
    }

    void Start () {
	}
	
	void Update () {
	}
}
