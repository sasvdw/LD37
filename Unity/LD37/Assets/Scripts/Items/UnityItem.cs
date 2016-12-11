using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnityItem : MonoBehaviour {

	public virtual void Start () {
	}
	
	public virtual void Update () {
	}

    public abstract bool Fire();
}
