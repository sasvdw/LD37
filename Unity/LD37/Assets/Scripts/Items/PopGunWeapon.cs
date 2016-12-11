using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopGunWeapon : ProjectileWeapon {

    public int ammunition = 6;
    private int ammunitionLeft;

    public override void Start() {
        ammunitionLeft = ammunition;

        base.Start();
    }

    public override bool Fire() {
        bool fired = base.Fire();
        if (fired) {
            this.ammunitionLeft -= 1;
        }

        if (this.ammunitionLeft == 0) {
            Destroy(this.gameObject);
            // TODO: Somehow let the domain know so the player reverts back to fists
        }

        return fired;
    }
}
