using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : BasePowerUp
{
    public int healingAmount = 20;

    protected override bool ApplyToPlayer(Player thePickerUpper)
    {
        if (thePickerUpper != null)
        {
            thePickerUpper.Heal(healingAmount);
            return true;
        }
        return false;
    }
}
