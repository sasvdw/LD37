using UnityEngine;
using Rewired;
using RewiredConsts;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerControl : MonoBehaviour {

    public int rewiredPlayerId = 0;
    public float moveSpeed = 3.0f;

    private Player rewiredPlayer;
    private CharacterController character;
    private Vector2 moveVector;

	void Start () {
        rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);

        character = GetComponent<CharacterController>();
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
            character.Move(moveVector * moveSpeed * Time.deltaTime);
        }
    }
}
