using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnityItem : MonoBehaviour {

	void Start () {
	}
	
	void Update () {
	}

    public abstract void Fire();

    public abstract void Drop();
}
