using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    public Button startbutton;
    public TMPro.TMP_Text statusLabel;

    // Start is called before the first frame update
    void Start()
    {
        startbutton.gameObject.SetActive(false);
        statusLabel.text = "Start something, like the server or the host or the client";

        startbutton.onClick.AddListener(OnStartButtonClicked);
        NetworkManager.OnClientStarted += OnClientStarted;
        NetworkManager.OnServerStarted += OnServerStarted;
    }

    private void OnServerStarted()
    {
        //StartGame();
        startbutton.gameObject.SetActive(true);
        statusLabel.text = "Press Start";
    }

    private void OnClientStarted()
    {
        if (!IsHost) 
        {
            statusLabel.text = "Waiting for game to start";
        }
        
    }

    private void OnStartButtonClicked()
    {
        StartGame();
    }

    public void StartGame()
    {
        NetworkManager.SceneManager.LoadScene(
            "Arena1Game",
            //"TestChat",
            UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
