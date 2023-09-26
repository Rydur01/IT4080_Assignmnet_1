using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ChatServer : NetworkBehaviour
{
    public ChatUi chatUi;
    const ulong SYSTEM_ID = ulong.MaxValue;
    private ulong[] dmClientIds = new ulong[2];
    private ulong[] dmClientError = new ulong[1];

    // Start is called before the first frame update
    void Start()
    {
        chatUi.printEnteredText = false;
        chatUi.MessageEntered += OnChatUiMessageEntered;

        if (IsServer)
        {
            NetworkManager.OnClientConnectedCallback += ServerOnClientConnected;
            NetworkManager.OnClientDisconnectCallback += ServerOnClientDisconnected;
            if (IsHost)
            {
                DisplayMessageLocally(SYSTEM_ID, $"You are the host And client {NetworkManager.LocalClientId}");
            }
            else
            {
                DisplayMessageLocally(SYSTEM_ID, "You are the server");
            }
        }
        else
        {
            DisplayMessageLocally(SYSTEM_ID, $"You are the client {NetworkManager.LocalClientId}");
        }
    }

    private void ServerOnClientConnected(ulong clientId)
    {
        ServerSendDirectMessage(
            $"I ({NetworkManager.LocalClientId}) see you ({clientId}) have connected to the server, well done",
            NetworkManager.LocalClientId,
            clientId);

        RecieveChatMessageClientRpc($"Client {clientId} has connected.", SYSTEM_ID);
    }
    private void ServerOnClientDisconnected(ulong clientId)
    {
        RecieveChatMessageClientRpc($"Client {clientId} has disconnected.", SYSTEM_ID);
    }

    private void DisplayMessageLocally(ulong from, string message)
    {
        string fromStr = $"Player {from}";
        Color textColor = chatUi.defaultTextColor;

        if(from == NetworkManager.LocalClientId)
        {
            fromStr = "you";
            textColor = Color.magenta;
        }
        else if(from == SYSTEM_ID)
        {
            fromStr = "SYS";
            textColor = Color.green;
        }

        chatUi.addEntry(fromStr, message, textColor);
    }

    private void OnChatUiMessageEntered(string message)
    {
        SendChatMessageServerRpc(message);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendChatMessageServerRpc(string message, ServerRpcParams serverRpcParams = default)
    {
        // "@123 hello world this is a longer message"
        if (message.StartsWith("@"))
        {
            string[] parts = message.Split(" ");
            string clientIdStr = parts[0].Replace("@", "");
            //ulong toClientId = ulong.Parse(clientIdStr);

            if (ulong.TryParse(clientIdStr, out ulong toClientId) && NetworkManager.Singleton.ConnectedClients.ContainsKey(toClientId))
            {
                ServerSendDirectMessage(message, serverRpcParams.Receive.SenderClientId, toClientId);
            }
            else
            {
                string errorMessage = "Invalid recipient. Message not sent.";
                ServerSendDirectMessage(errorMessage, serverRpcParams.Receive.SenderClientId, ulong.MaxValue);
            }
        }
        else
        {
            RecieveChatMessageClientRpc(message, serverRpcParams.Receive.SenderClientId);
        }
        
    }

    [ClientRpc]
    public void RecieveChatMessageClientRpc(string message, ulong from, ClientRpcParams clientRpcParams = default)
    {
        DisplayMessageLocally(from, message);
    }

    private void ServerSendDirectMessage(string message, ulong from, ulong to)
    {
        dmClientIds[0] = from;
        dmClientIds[1] = to;
        dmClientError[0] = from;

        ClientRpcParams rpcParams = default;
        

        if (to == ulong.MaxValue)
        {
            rpcParams.Send.TargetClientIds = dmClientError;
            RecieveChatMessageClientRpc($"{message}", from, rpcParams);
        }
        else
        {
            rpcParams.Send.TargetClientIds = dmClientIds;
            RecieveChatMessageClientRpc($"<whisper> {message}", from, rpcParams);
        }
    }
}
