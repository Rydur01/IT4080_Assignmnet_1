using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    public Button startbutton;
    public TMPro.TMP_Text statusLabel;

    void Start()
    {
        startbutton.onClick.AddListener(OnStartButtonClicked);
        NetworkManager.OnClientStarted += OnClientStarted;
        NetworkManager.OnServerStarted += OnServerStarted;

        startbutton.gameObject.SetActive(false);
        statusLabel.text = "Start something, like the server or the host or the client";
    }

    private void OnClientStarted()
    {
        if (!IsHost) 
        {
            statusLabel.text = "Waiting for game to start";
        }
        
    }

    private void OnServerStarted()
    {
        //StartGame();
        //startbutton.gameObject.SetActive(true);
        //statusLabel.text = "Press Start";
        GotoLobby();
    }

    private void OnStartButtonClicked()
    {
        StartGame();
    }

    public void GotoLobby()
    {
        NetworkManager.SceneManager.LoadScene(
            "Lobby", 
            UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void StartGame()
    {
        NetworkManager.SceneManager.LoadScene(
            "Arena1Game",
            UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
