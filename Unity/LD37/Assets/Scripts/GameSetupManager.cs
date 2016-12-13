using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Rewired;
using RewiredConsts;
using LD37.Domain.Cousins;
using System;

public class GameSetupManager : MonoBehaviour {

    private List<Player> playersToRemoveFromUnused;
    private List<Player> unusedPlayers;
    private List<Player> players;
    private List<int> cousins;
    private List<Cousin> allCousins;
    private List<bool> ready;
    private List<Transform> playerUIs;
    private List<float> switchCoolDown;
    private bool setupCompleted = false;

    private const float SWITCH_COOLDOWN = 0.75f;
    
	void Start () {
        playersToRemoveFromUnused = new List<Player>(4);
        unusedPlayers = new List<Player>(4);
        players = new List<Player>(4);
        cousins = new List<int>(4);
        allCousins = new List<Cousin>(4);
        ready = new List<bool>(4);
        playerUIs = new List<Transform>(4);
        switchCoolDown = new List<float>(4);

        for (int i = 0; i < 4; i++) {
            unusedPlayers.Add(ReInput.players.GetPlayer(i));
            switchCoolDown.Add(0.0f);
            ready.Add(false);
        }

        allCousins.AddRange(Cousin.All);

        playerUIs.Add(GameObject.Find("PanelPlayer0").transform);
        playerUIs.Add(GameObject.Find("PanelPlayer1").transform);
        playerUIs.Add(GameObject.Find("PanelPlayer2").transform);
        playerUIs.Add(GameObject.Find("PanelPlayer3").transform);
    }
	
	void Update () {
        if (setupCompleted) {
            return;
        }

        for (int i = 0; i < switchCoolDown.Count; i++) {
            switchCoolDown[i] -= Time.deltaTime;
        }

        foreach (Player player in unusedPlayers) {
            if (player.GetButton(RewiredConsts.Action.Activate)) {
                players.Add(player);
                playersToRemoveFromUnused.Add(player);

                ready.Add(false);
                cousins.Add(GetNextAvailableCousin(0));

                switchCoolDown[players.Count - 1] = SWITCH_COOLDOWN; // Prevent instant READY

                UpdateUIForPlayer(players.Count - 1);

                //TODO: player.GetCurrentInputSources and show correct instructions
            }
        }

        if (playersToRemoveFromUnused.Count > 0) {
            unusedPlayers.RemoveAll(playersToRemoveFromUnused.Contains);
            playersToRemoveFromUnused.Clear();
        }

        bool allReady = true;
        foreach (Player player in players) {
            int playerIndex = players.IndexOf(player);
            allReady = allReady && ready[playerIndex];

            if (ready[playerIndex]) {
                continue;
            }

            if (switchCoolDown[playerIndex] > 0.0f) {
                continue;
            }

            float xAxis = player.GetAxis(RewiredConsts.Action.MoveHorizontal);
            if (player.GetButton(RewiredConsts.Action.Activate)) {
                ready[playerIndex] = true;
                UpdateUIForPlayer(playerIndex);
            } else if (xAxis < 0) {
                switchCoolDown[playerIndex] = SWITCH_COOLDOWN;
                cousins[playerIndex] = GetPreviousAvailableCousin(cousins[playerIndex]);
                UpdateUIForPlayer(playerIndex);
            } else if (xAxis > 0) {
                switchCoolDown[playerIndex] = SWITCH_COOLDOWN;
                cousins[playerIndex] = GetNextAvailableCousin(cousins[playerIndex]);
                UpdateUIForPlayer(playerIndex);
            }
        }

        if (players.Count > 0 && allReady) {
            for (int i = 0; i < players.Count; i++) {
                GameController.Instance.AddGamePlayer(allCousins[cousins[i]], players[i]);
            }

            GameObject.Find("Canvas").transform.FindChild("Fade").gameObject.SetActive(true);

            setupCompleted = true;
            SceneManager.LoadScene("Test");
        }
    }

    private int GetNextAvailableCousin(int from) {
        for (int i = from + 1; i < from + allCousins.Count; i++) {
            int index = i < allCousins.Count ? i : i - allCousins.Count;

            if (!cousins.Contains(index)) {
                return index;
            }
        }

        return 0;
    }

    private int GetPreviousAvailableCousin(int from) {
        for (int i = from - 1; i > from - allCousins.Count; i--) {
            int index = i >= 0 ? i : allCousins.Count + i;

            if (!cousins.Contains(index)) {
                return index;
            }
        }
        
        return 0;
    }

    void UpdateUIForPlayer(int index) {
        Transform uiTransform = playerUIs[index];

        if (!ready[index]) {
            var joinText = uiTransform.FindChild("JoinText").gameObject;
            if (joinText.activeSelf) {
                joinText.SetActive(false);
            }

            var nameText = uiTransform.FindChild("NameText").gameObject;
            if (!nameText.activeSelf) {
                nameText.gameObject.SetActive(true);
            }
            nameText.GetComponent<Text>().text = allCousins[cousins[index]].Name;

            var readyText = uiTransform.FindChild("ReadyText").gameObject;
            if (!readyText.activeSelf) {
                readyText.SetActive(true);
            }
        } else {
            var nameText = uiTransform.FindChild("NameText").gameObject;

            var arrowLeft = nameText.transform.FindChild("ArrowLeftImage").gameObject;
            if (arrowLeft.activeSelf) {
                arrowLeft.SetActive(false);
            }

            var arrowRight = nameText.transform.FindChild("ArrowRightImage").gameObject;
            if (arrowRight.activeSelf) {
                arrowRight.SetActive(false);
            }

            var readyText = uiTransform.FindChild("ReadyText").gameObject;
            if (readyText.activeSelf) {
                readyText.SetActive(false);
            }

            var isReadyText = uiTransform.FindChild("IsReadyText").gameObject;
            if (!isReadyText.activeSelf) {
                isReadyText.SetActive(true);
            }
        }
    }
}
