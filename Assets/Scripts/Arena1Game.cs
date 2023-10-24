using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class Arena1Game : NetworkBehaviour
{
    public Player playerPrefab;
    public Player hostPrefab;
    public Camera arenaCamera;
    private NetworkedPlayers networkedPlayers;

    private int positionIndex = 0;
    private Vector3[] startPositions = new Vector3[]
    {
        new Vector3(4, 2, 0),
        new Vector3(-4, 2, 0),
        new Vector3(0, 2, 4),
        new Vector3(0, 2, -4)
    };
    

    // Start is called before the first frame
    void Start()
    {
        arenaCamera.enabled = !IsClient;
        arenaCamera.GetComponent<AudioListener>().enabled = !IsClient;

        networkedPlayers = GameObject.Find("NetworkedPlayers").GetComponent<NetworkedPlayers>();
        NetworkHelper.Log($"Players = {networkedPlayers.allNetPlayers.Count}");

        if (IsServer)
        {
            SpawnPlayers();
        }
    }

    private Vector3 NextPosition()
    {
        Vector3 pos = startPositions[positionIndex];
        positionIndex += 1;
        if (positionIndex > startPositions.Length - 1)
        {
            positionIndex = 0;
        }
        return pos;
    }


    private void SpawnPlayers()
    {
        foreach(NetworkPlayerInfo info in networkedPlayers.allNetPlayers)
        {
            Player prefab = playerPrefab;
            //if (clientId == 0)
            //{
            //    prefab = hostPrefab;
            //}
            //else
            //{
            //    prefab = playerPrefab;
            //}

            Player playerSpawn = Instantiate(prefab, NextPosition(), Quaternion.identity);
            playerSpawn.GetComponent<NetworkObject>().SpawnAsPlayerObject(info.clientId);
            playerSpawn.playerColor.Value = info.color;
        }
    }
}
