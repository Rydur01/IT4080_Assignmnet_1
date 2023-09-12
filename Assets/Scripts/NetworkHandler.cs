using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkHandler : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.OnClientStarted += OnClientStarted;
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private bool hasPrinted = false;
    private void PrintMe()
    {
        if (hasPrinted)
        {
            return;
        }
        Debug.Log("I AM");
        hasPrinted = true;
        if (IsServer)
        {
            Debug.Log($" The Server! {NetworkManager.ServerClientId}");
        }
        if (IsHost)
        {
            Debug.Log($" The Host!{NetworkManager.ServerClientId}/{NetworkManager.LocalClientId}");
        }
        if (IsClient)
        {
            Debug.Log($" The Client! {NetworkManager.LocalClientId}");
        }
        if (!IsServer && !IsClient)
        {
            Debug.Log(" Nothing yet!");
            hasPrinted = false;
        }
    }

    private void OnClientStarted()
    {
        Debug.Log("!! Client Started !!");
        NetworkManager.OnClientConnectedCallback += ClientOnClientConnected;
        NetworkManager.OnClientDisconnectCallback += ClientOnClientDisconnected;
        NetworkManager.OnServerStopped += ClientOnClientStopped;
        PrintMe();
    }


    //-------------------
    //Client Actions
    //-------------------

    private void ClientOnClientConnected(ulong clientId)
    {
        PrintMe();

        // Print I {clientId} have connected to the server
        if (IsClient && !IsServer) 
        {
            Debug.Log($"I {clientId} have connected to the server");
        }
        

        // handle the case when we are the client running on the host. Some other client connected.
        if (IsHost)
        {
            Debug.Log($"Client {clientId} has connected to the server");
        }
    }

    private void ClientOnClientDisconnected(ulong clientId)
    {
        // print I {clientID} have disconnected from the server.
        if (IsClient && !IsHost)
        {
            Debug.Log($"I {clientId} have disconnected from the server");
        }

        if (IsHost)
        {
            Debug.Log($"Client {clientId} has disconnected from the server");
        }

    }

    private void ClientOnClientStopped(bool indicator)
    {
        Debug.Log("!! Client Stopped !!");
        hasPrinted = false;
        NetworkManager.OnClientConnectedCallback -= ClientOnClientConnected;
        NetworkManager.OnClientDisconnectCallback -= ClientOnClientDisconnected;
        NetworkManager.OnClientStopped -= ClientOnClientStopped;
    }


    //-------------------
    //Server Actions
    //-------------------
    private void OnServerStarted()
    {
        Debug.Log("!! Server Started !!");
        NetworkManager.OnClientConnectedCallback += ServerOnClientConnected;
        NetworkManager.OnClientDisconnectCallback += ServerOnClientDisconnected;
        NetworkManager.OnServerStopped += ServerOnServerStopped;
        PrintMe();
    }

    private void ServerOnClientConnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} connected to the server");
    }

    private void ServerOnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} disconnected from the server");
    }

    private void ServerOnServerStopped(bool indicator)
    {
        Debug.Log("!! Server Stopped !!");
        hasPrinted = false;
        NetworkManager.OnClientConnectedCallback -= ServerOnClientConnected;
        NetworkManager.OnClientDisconnectCallback -= ServerOnClientDisconnected;
        NetworkManager.OnServerStopped -= ServerOnServerStopped;
    }
}
