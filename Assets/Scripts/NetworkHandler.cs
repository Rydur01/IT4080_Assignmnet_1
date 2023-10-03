using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEditor.PackageManager;

public class NetworkHandler : NetworkBehaviour
{
    private bool OnServerStopped;

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.OnClientStarted += OnClientStarted;
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private void PrintMe()
    {
        if (IsServer)
        {
            NetworkHelper.Log($" I AM a Server! {NetworkManager.ServerClientId}");
        }
        if (IsHost)
        {
            NetworkHelper.Log($" I AM a Host!{NetworkManager.ServerClientId}/{NetworkManager.LocalClientId}");
        }
        if (IsClient)
        {
            NetworkHelper.Log($" I AM a Client! {NetworkManager.LocalClientId}");
        }
        if (!IsServer && !IsClient)
        {
            NetworkHelper.Log(" I Am Nothing yet!");
        }
    }




    //-------------------
    //Client Actions
    //-------------------

    private void OnClientStarted()
    {
        NetworkHelper.Log("!! Client Started !!");
        NetworkManager.OnClientConnectedCallback += ClientOnClientConnected;
        NetworkManager.OnClientDisconnectCallback += ClientOnClientDisconnected;
        NetworkManager.OnServerStopped += ClientOnClientStopped;
        PrintMe();
    }

    private void ClientOnClientStopped(bool indicator)
    {
        NetworkHelper.Log("!! Client Stopped !!");
        NetworkManager.OnClientConnectedCallback -= ClientOnClientConnected;
        NetworkManager.OnClientDisconnectCallback -= ClientOnClientDisconnected;
        NetworkManager.OnClientStopped -= ClientOnClientStopped;
        PrintMe();
    }

    private void ClientOnClientConnected(ulong clientId)
    {
        // IsClient IsServer IsHost NetworkManager.ClientId NetworkManager.LocalId NetworkManager.ServerId
        if (IsClient && !IsServer)
        {
            NetworkHelper.Log($"I have connected {clientId}");
        }
        if(IsHost)
        {
            if (clientId == 0)
            {
                NetworkHelper.Log($"I have connected {clientId}");
            }
        }
    }
    private void ClientOnClientDisconnected(ulong clientId)
    {
        if (OnServerStopped)
        {
            NetworkHelper.Log($"I have disonnected 0");
        }

        if (IsClient && !IsServer)
        {
            NetworkHelper.Log("!! Client Stopped !!");
            NetworkHelper.Log($"I am Client! {NetworkManager.LocalClientId}");   
        }
    }

    //-------------------
    //Server Actions
    //-------------------
    private void OnServerStarted()
    {
        NetworkHelper.Log("!! Server Started !!");
        NetworkManager.OnClientConnectedCallback += ServerOnClientConnected;
        NetworkManager.OnClientDisconnectCallback += ServerOnClientDisconnected;
        NetworkManager.OnServerStopped += ServerOnServerStopped;
        PrintMe();
    }
    private void ServerOnServerStopped(bool indicator)
    {
        OnServerStopped = true;
        NetworkHelper.Log("!! Server Stopped !!");
        NetworkManager.OnClientConnectedCallback -= ServerOnClientConnected;
        NetworkManager.OnClientDisconnectCallback -= ServerOnClientDisconnected;
        NetworkManager.OnServerStopped -= ServerOnServerStopped;
        PrintMe();
    }

    private void ServerOnClientConnected(ulong clientId)
    {
        NetworkHelper.Log($"Client {clientId} connected to the server");
    }

    private void ServerOnClientDisconnected(ulong clientId)
    {
        NetworkHelper.Log($"Client {clientId} disconnected from the server");
    }

    

}
