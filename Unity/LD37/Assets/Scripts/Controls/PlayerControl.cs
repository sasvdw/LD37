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

    public Transform Camera { get; private set; }

    public void SetCousin(Cousin cousin, int playerNumber)
    {
        this.Cousin = cousin;
        this.RewiredPlayerId = playerNumber;
        this.rewiredPlayer = ReInput.players.GetPlayer(playerNumber);
    }

    private void Start()
    {
        this.character = GetComponent<Rigidbody2D>();

        foreach(Camera camera in FindObjectsOfType<Camera>())
        {
            if(camera.gameObject.name.Equals("Camera Player" + this.RewiredPlayerId))
            {
                this.Camera = camera.gameObject.transform;
            }
        }
    }

    private void Update()
    {
        this.GetInput();
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
