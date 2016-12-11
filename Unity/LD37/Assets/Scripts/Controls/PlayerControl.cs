using LD37.Domain.Cousins;
using Rewired;
using UnityEngine;
using Action = RewiredConsts.Action;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerControl : MonoBehaviour
{
    private Player rewiredPlayer;
    private Rigidbody2D character;
    private Animator animator;

    private Vector2 moveVector;
    private Vector2 facing = new Vector2(0f, -1.0f);

    public int RewiredPlayerId = 0;
    public float MoveSpeed = 3.0f;
    private GameController gameController;
    private UnityItem itemInProximity;

    public Cousin Cousin { get; private set; }

    public Transform Camera { get; set; }

    public Color Color { get; set; }

    public Transform CurrentItem { get; set; }
    public Transform Fists { get; set; }

    public Vector2 Facing { get {
            return this.facing;
        }
    }

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
        this.gameController = GameController.Instance;
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

        if (this.rewiredPlayer.GetButton(Action.Activate)) {
            CurrentItem.GetComponent<UnityItem>().Fire();
        }

        var pickupOrDrop = this.rewiredPlayer.GetButton(Action.PickupOrDrop);
        if(pickupOrDrop && this.itemInProximity)
        {
            var itemToPickup = this.gameController.GetDomainItem(this.itemInProximity);
            this.Cousin.PickUp(itemToPickup);

            Debug.Log(string.Format("{0} picked up the {1}", this.Cousin.Name, itemToPickup.Name));
        }
        if(pickupOrDrop && !this.itemInProximity)
        {
            this.Cousin.DropItem();
        }
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
            SetAnimatorBoolIfNotSet("faceSide", true);
            SetAnimatorBoolIfNotSet("faceUp", false);
            SetAnimatorBoolIfNotSet("faceDown", false);

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
            SetAnimatorBoolIfNotSet("faceSide", false);

            if (this.character.velocity.y < 0) {
                this.facing = new Vector2(0.0f, -1.0f);

                SetAnimatorBoolIfNotSet("faceUp", false);
                SetAnimatorBoolIfNotSet("faceDown", true);
            } else {
                this.facing = new Vector2(0.0f, 1.0f);

                SetAnimatorBoolIfNotSet("faceUp", true);
                SetAnimatorBoolIfNotSet("faceDown", false);
            }
        }
    }

    private void SetAnimatorBoolIfNotSet(string name, bool value) {
        if (this.animator.GetBool(name) != value) {
            this.animator.SetBool(name, value);
        }
    }

    public void Damage(int damage) {
        Cousin.Damage(damage);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        this.itemInProximity = collision.gameObject.GetComponent<UnityItem>();
        Debug.Log("enter");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var itemLeaving = collision.gameObject.GetComponent<UnityItem>();

        if(itemLeaving)
        {
            this.itemInProximity = null;
        }

        Debug.Log("exit");
    }
}
