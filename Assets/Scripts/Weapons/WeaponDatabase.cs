using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Game/Weapons/Weapon Database", fileName = "WeaponDatabase")]
public class WeaponDatabase : ScriptableObject
{
    public WeaponPreset[] all;

    public WeaponPreset GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id) || all == null) return null;
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i] && string.Equals(all[i].id, id, StringComparison.Ordinal))
                return all[i];
        }
        return null;
    }
}