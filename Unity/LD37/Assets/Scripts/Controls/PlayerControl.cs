using LD37.Domain.Cousins;
using Rewired;
using RewiredConsts;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControl : MonoBehaviour
{
    private Player rewiredPlayer;
    private Rigidbody2D character;
    private Animator animator;

    private Vector2 moveVector;
    private Vector2 facing = new Vector2(0f, -1.0f);

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
        this.animator = GetComponent<Animator>();
    }

    private void Update()
    {
        this.GetInput();
    }

    private void FixedUpdate()
    {
        this.ProcessInput();
        this.UpdateFacing();
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

    private void UpdateFacing() {
        if (this.character.velocity.magnitude <= 0) {
            this.animator.SetFloat("speed", 0.0f);
            return;
        }

        this.animator.SetFloat("speed", this.character.velocity.magnitude);
        if (System.Math.Abs(this.character.velocity.x) >= System.Math.Abs(this.character.velocity.y)) {
            SetAnimatorBooltIfNotSet("faceSide", true);
            SetAnimatorBooltIfNotSet("faceUp", false);
            SetAnimatorBooltIfNotSet("faceDown", false);

            if (this.character.velocity.x < 0) {
                this.facing = new Vector2(-1.0f, 0.0f);

                Vector3 scale = transform.localScale; ;
                scale.x = -1;
                transform.localScale = scale;
            } else {
                this.facing = new Vector2(1.0f, 0.0f);

                Vector3 scale = transform.localScale; ;
                scale.x = 1;
                transform.localScale = scale;
            }
        } else {
            SetAnimatorBooltIfNotSet("faceSide", false);

            if (this.character.velocity.y < 0) {
                this.facing = new Vector2(0.0f, -1.0f);

                SetAnimatorBooltIfNotSet("faceUp", false);
                SetAnimatorBooltIfNotSet("faceDown", true);
            } else {
                this.facing = new Vector2(0.0f, 1.0f);

                SetAnimatorBooltIfNotSet("faceUp", true);
                SetAnimatorBooltIfNotSet("faceDown", false);
            }
        }
    }

    private void SetAnimatorBooltIfNotSet(string name, bool value) {
        if (this.animator.GetBool(name) != value) {
            this.animator.SetBool(name, value);
        }
    }
}
