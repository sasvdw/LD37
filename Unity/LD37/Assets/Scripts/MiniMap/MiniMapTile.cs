using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapTile : MonoBehaviour {

    public Color emptyColor = Color.white;
    private Color occupiedColor = Color.white;
    private bool occupied = false;

    private SpriteRenderer spriteRenderer;

    public void SetOccupied(Color color) {
        this.occupiedColor = new Color(color.r, color.g, color.b, 1.0f);
        this.occupied = true;
    }

    public void SetUnoccupied() {
        this.occupied = false;
    }

	void Start () {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	void Update () {
        if (occupied) {
            this.spriteRenderer.color = occupiedColor;
        } else {
            this.spriteRenderer.color = emptyColor;
        }
	}
}
