using System;
using System.Collections.Generic;
using UnityEngine;

public class HitboxManager : MonoBehaviour
{
    private HitboxGroup[] groups;

    void Awake()
    {
        groups = GetComponentsInChildren<HitboxGroup>(true);
    }

    public void SetGroup(HitboxID id, bool on)
    {
        for (int i = 0; i < groups.Length; i++)
            if (groups[i].id == id)
                groups[i].SetEnabled(on);
    }

    public void SetGroupHitReports(HitboxID id, Type moveType)
    {
        for (int i = 0; i < groups.Length; i++)
            if (groups[i].id == id)
                groups[i].SetHitReports(moveType);
    }

    public void SetGroupMoveMachines(HitboxID id, BossMoveMachine mm)
    {
        for (int i = 0; i < groups.Length; i++)
            if (groups[i].id == id)
                groups[i].SetMoveMachines(mm);
    }
}
