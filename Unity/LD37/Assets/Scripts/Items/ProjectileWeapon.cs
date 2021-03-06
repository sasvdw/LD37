﻿using UnityEngine;

public abstract class ProjectileWeapon : UnityItem
{
    protected float coolDownRemaining = 0.0f;

    public Transform bulletPrefab = null;
    public int damage = 1;
    public float duration = 0.7f;
    public float speed = 0.5f;
    public float coolDown = 1.5f;

    public override void Update()
    {
        if(coolDownRemaining > 0.0f)
        {
            coolDownRemaining -= Time.deltaTime;
        }
    }

    public override bool Fire()
    {
        var playerControl = GetComponentInParent<PlayerControl>();

        if(!playerControl)
        {
            return false;
        }

        if(coolDownRemaining > 0.0f)
        {
            return false;
        }

        Vector3 spawnPosition = playerControl.transform.position;
        spawnPosition.x += playerControl.Facing.x * 0.6f;
        spawnPosition.y += playerControl.Facing.y * 0.6f;
        Transform projectileTransform = Instantiate(
            bulletPrefab,
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
        return true;
    }
}
