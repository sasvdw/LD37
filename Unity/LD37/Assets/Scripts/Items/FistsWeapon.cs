using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistsWeapon : UnityItem {

    public Transform punchPrefab = null;
    public int damage = 1;
    public float duration = 0.7f;
    public float speed = 0.5f;
    public float coolDown = 1.5f;

    private PlayerControl playerControl;
    private float coolDownRemaining = 0.0f;

	void Start () {
        this.playerControl = GetComponentInParent<PlayerControl>();
	}
	
	void Update () {
		if (coolDownRemaining > 0.0f) {
            coolDownRemaining -= Time.deltaTime;
        }
	}

    public override void Fire() {
        if (coolDownRemaining > 0.0f) {
            return;
        }

        Vector3 spawnPosition = transform.position;
        spawnPosition.x += playerControl.Facing.x * 0.6f;
        spawnPosition.y += playerControl.Facing.y * 0.6f;
        Transform projectileTransform = Instantiate(
            punchPrefab,
            spawnPosition,
            Quaternion.identity
        );

        Rigidbody2D projectileBody = projectileTransform.GetComponent<Rigidbody2D>();
        projectileBody.velocity = speed * playerControl.Facing;
        projectileBody.rotation = Mathf.Atan2(playerControl.Facing.y, playerControl.Facing.x) * Mathf.Rad2Deg;

        Projectile projectile = projectileTransform.GetComponent<Projectile>();
        projectile.damage = damage;
        projectile.duration = duration;

        coolDownRemaining = coolDown;
    }
    
    public override void Drop() {
        // Cannot drop your fists...
    }
}
