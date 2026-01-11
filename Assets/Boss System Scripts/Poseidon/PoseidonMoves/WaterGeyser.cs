using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaterGeyser : BossMove
{
    private PoseidonBoss boss;
    private float projSpeed;
    private float size;
    private float chargeUpTime;
    private int count;
    public WaterGeyser(PoseidonBoss boss, float projSpeed, float size, float chargeUpTime, int count) : base(boss)
    {
        this.boss = boss;
        this.projSpeed = projSpeed;
        this.size = size;
        this.chargeUpTime = chargeUpTime;
        this.count = count;
    }
    private float timer = 0;
    bool fired;
    bool comboChecked;
    private float comboCheckTime = 0.2f;

    private List<Vector3> geyserPositions;
    public override void Start()
    {
        Debug.Log("Starting WaterWave");
        geyserPositions = FindGeyserPosition();
        foreach (Vector3 pos in geyserPositions)
        {
            var warning = GameObject.Instantiate(boss.geyserWarning, pos, Quaternion.identity);
            warning.transform.localScale = new Vector3(size, 0.02f, size);
        }
    }
    public override void Execute()
    {

        timer += Time.deltaTime;

        if (!fired && timer >= chargeUpTime)
        {
            fired = true;
            foreach(Vector3 pos in geyserPositions)
            {
                var geyser = GameObject.Instantiate(boss.geyser, pos, Quaternion.identity);
                geyser.transform.localScale = new Vector3(size, 1, size);
                //SetupBossHitReporting(geyser);
            }
        }

        //call its own comboCheck instead of animation
        if (!comboChecked && timer >= chargeUpTime + comboCheckTime)
        {
            comboChecked = true;
            AnimEvent("comboCheck");
            isFinished = true;
        }
    }

    public List<Vector3> FindGeyserPosition()
    {
        var list = new List<Vector3>();

        float max = 45f;
        float minDist = 6f;

        while (list.Count < count)
        {
            float x = Random.Range(-max, max);
            float z = Random.Range(-max, max);
            Vector3 newRand = new Vector3(x, 1f, z);

            bool valid = true;

            for (int i = 0; i < list.Count; i++)
            {
                // REAL distance check
                if ((list[i] - newRand).sqrMagnitude < minDist * minDist)
                {
                    valid = false;
                    break; // stop checking, reroll
                }
            }

            if (valid)
                list.Add(newRand);
        }

        return list;
    }

    public override void End()
    {
        Debug.Log("Ending WaterWave");
        base.End();
    }
    public override void AnimEvent(string evt)
    {
        switch (evt)
        {
            case "start":
                //create wave projectile and start its own animation
                break;

            case "end":
                isFinished = true;
                break;
            case "comboCheck":
                boss.BossMoveComboDetails(GetType(), out bool hit, out bool LOS, out float dist);

                bool close = dist < 11f;
                bool far = dist > 11f;
                string nextMoveId = "null";

                bool closeAttacking = boss.IsPlayerAttacking();
                if (closeAttacking && LOS) { nextMoveId = boss.mm.Choose("quickPosMelee", "null"); }
                else if (hit && !LOS) { nextMoveId = boss.mm.Choose("wideWaterBlast", "null"); }
                else if (hit && close) { nextMoveId = boss.mm.Choose("posMelee", "wideWaterBlast", "posMelee"); }
                else if (hit && far) { nextMoveId = boss.mm.Choose("waterBlast", "boatShield", "null"); }
                else if (!hit && LOS) { nextMoveId = boss.mm.Choose("wideWaterBlast", "waterWave"); }
                else { nextMoveId = boss.mm.Choose("waterBlast", "posMelee", "null"); }

                if (!string.IsNullOrEmpty(nextMoveId))
                {
                    boss.mm.PlayMove(nextMoveId);
                }
                isFinished = true;
                break;
        }
    }

    public override BossMove Clone()
    {
        return new WaterGeyser(this.boss, this.projSpeed, this.size, this.chargeUpTime, this.count);
    }
}
