using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using LD37.Domain.Cousins;

public class WinnerScene : MonoBehaviour {

    float inputDelay;

	void Start () {
        Cousin cousin = GameController.Instance.Winner;

        GameObject.Find("Canvas").transform.FindChild("WinnerName").GetComponent<Text>().text = cousin.Name;

        inputDelay = 3.0f;
	}
	
	void Update () {
		if (inputDelay > 0.0f) {
            inputDelay -= Time.deltaTime;
            return;
        }

        GameObject.Find("Canvas").transform.FindChild("AnyKeyLabel").gameObject.SetActive(true);

        if (Input.anyKey) {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
