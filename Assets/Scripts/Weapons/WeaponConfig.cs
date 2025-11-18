using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "Weapons/Weapon Config")]
public class WeaponConfig : ScriptableObject
{
    public string weaponName = "New Weapon";
    [TextArea] public string notes;

    [Serializable]
    public struct BindingMap
    {
        public Weapon_Systems.InputBinding binding; // What you press
        public AbilityBase ability;                 // What it triggers
    }

    [Header("Input ? Ability maps")]
    public List<BindingMap> bindings = new List<BindingMap>();
}
