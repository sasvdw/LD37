using LD37.Domain.Cousins;
using LD37.Domain.Movement;
using UnityEngine;

public class ExitRoom : MonoBehaviour
{
    public DirectionEnum DirectionEnum = DirectionEnum.North;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var playerController = other.gameObject.GetComponent<PlayerControl>();
        if(!playerController)
        {
            return;
        }

        Debug.Log("Moving " + this.DirectionEnum);

        Cousin cousin = playerController.Cousin;
        cousin.Move(Direction.GetDirection(this.DirectionEnum));
    }

    private void Start() {}

    private void Update() {}
}
