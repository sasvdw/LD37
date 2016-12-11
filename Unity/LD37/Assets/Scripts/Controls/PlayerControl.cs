using LD37.Domain.Cousins;
using Rewired;
using RewiredConsts;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControl : MonoBehaviour
{
    private Player rewiredPlayer;
    private Rigidbody2D character;
    private Vector2 moveVector;

    public int RewiredPlayerId = 0;
    public float MoveSpeed = 3.0f;

    public Cousin Cousin { get; private set; }

    public Transform Camera { get; set; }

    public Color Color { get; set; }

    public void SetCousin(Cousin cousin, int playerNumber)
    {
        this.Cousin = cousin;
        this.RewiredPlayerId = playerNumber;
        this.rewiredPlayer = ReInput.players.GetPlayer(playerNumber);
    }

    private void Start()
    {
        this.character = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        this.GetInput();
    }

    private void FixedUpdate()
    {
        this.ProcessInput();
    }

    private void GetInput()
    {
        this.moveVector.x = this.rewiredPlayer.GetAxis(Action.MoveHorizontal);
        this.moveVector.y = this.rewiredPlayer.GetAxis(Action.MoveVertical);
    }

    private void ProcessInput()
    {
        if(this.moveVector.magnitude > 0)
        {
            this.character.velocity = this.moveVector * this.MoveSpeed;
            return;
        }
        this.character.velocity = new Vector3(0, 0, 0);
    }
}
