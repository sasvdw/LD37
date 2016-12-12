using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float duration = 1.0f;
    public int damage = 1;
    public bool destroyOnHit = false;

	void Start () {
		
	}
	
	void Update () {
        if (duration > 0.0f) {
            duration -= Time.deltaTime;
        } else {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        PlayerControl playerController = collision.gameObject.GetComponent<PlayerControl>();
        if (playerController != null && this.damage > 0) {
            playerController.TakeDamage(damage);
            damage = 0;

            if (destroyOnHit) {
                Destroy(this.gameObject);
            }
        }
    }
}
