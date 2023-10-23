using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BasePowerUp : NetworkBehaviour
{
    public void ServerPickUp(Player thePickerUpper)
    {
        if(IsServer)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
