using UnityEngine;
using Rewired;
using RewiredConsts;
using System;
using LD37.Domain.Cousins;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControl : MonoBehaviour {

    public int rewiredPlayerId = 0;
    public float moveSpeed = 3.0f;

    public Cousin cousin { get; private set; }

    public Transform camera { get; private set; }

    private Player rewiredPlayer;
    private Rigidbody2D character;
    private Vector2 moveVector;

    public void SetCousin(Cousin cousin, int playerNumber) {
        this.cousin = cousin;
        this.rewiredPlayerId = playerNumber;
        rewiredPlayer = ReInput.players.GetPlayer(playerNumber);
    }

	void Start () {
        character = GetComponent<Rigidbody2D>();

        foreach (Camera camera in FindObjectsOfType<Camera>()) {
            if (camera.gameObject.name.Equals("Camera Player" + rewiredPlayerId)) {
                this.camera = camera.gameObject.transform;
            }
        }
	}
	
	void Update () {
        GetInput();
        ProcessInput();
	}

    private void GetInput() {
        moveVector.x = rewiredPlayer.GetAxis(RewiredConsts.Action.MoveHorizontal);
        moveVector.y = rewiredPlayer.GetAxis(RewiredConsts.Action.MoveVertical);
    }

    private void ProcessInput() {
        if (moveVector.magnitude > 0) {
            character.velocity = moveVector * moveSpeed;
        } else {
            character.velocity = new Vector3(0, 0, 0);
        }
    }
}
