using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelPlayerSetup : MonoBehaviour {

    public int playerIndex = 0;

	void Start () {
        var playerText = transform.FindChild("PlayerText").gameObject;
        playerText.GetComponent<Text>().text = "COUSIN " + (playerIndex + 1);

        var color = GameController.Instance.PlayerColors[playerIndex];
        var panelImage = GetComponent<Image>();
        panelImage.color = color;
    }
	
	void Update () {
	}
}
