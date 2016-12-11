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

    public PlayerControl()
    {
        this.moveVector = Vector2.zero;
    }

    public void SetCousin(Cousin cousin, int playerNumber)
    {
        this.Cousin = cousin;
        this.RewiredPlayerId = playerNumber;
        this.rewiredPlayer = ReInput.players.GetPlayer(playerNumber);
    }

    private void Start()
    {
        this.character = GetComponent<Rigidbody2D>();
        this.rewiredPlayer = ReInput.players.GetPlayer(this.RewiredPlayerId);
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
        var x = this.rewiredPlayer.GetAxis(Action.MoveHorizontal);
        var y = this.rewiredPlayer.GetAxis(Action.MoveVertical);

        this.moveVector.Set(x, y);
    }

    private void ProcessInput()
    {
        if(this.moveVector.magnitude > 0)
        {
            this.character.velocity = this.moveVector * this.MoveSpeed;
            return;
        }
        this.character.velocity = Vector2.zero;
    }
}
