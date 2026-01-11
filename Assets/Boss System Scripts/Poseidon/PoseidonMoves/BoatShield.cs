using System.Collections;
using System.Threading;
using UnityEngine;

public class BoatShield : BossMove
{
    private PoseidonBoss boss;
    private int projHealth;
    private GameObject[] boats = new GameObject[3];
    private GameObject shield;
    public BoatShield(PoseidonBoss boss, int projHealth) : base(boss)
    {
        this.boss = boss;
        this.projHealth = projHealth;
    }
    float timer = 0;
    bool comboChecked = false;
    public override void Start()
    {
        if (boss.shielded)
        {
            Debug.Log("Already shielded");
            comboChecked = true;
            AnimEvent("comboCheck");
            return;
        }

        Debug.Log("Starting BoatShield");

        Vector3 center = boss.transform.position;
        center.y += 0;

        // 3 boats evenly spaced: 360/3 = 120 degrees
        for (int i = 0; i < boats.Length; i++)
        {
            float angleDeg = 0 + (360f / boats.Length) * i;
            float angleRad = angleDeg * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(Mathf.Cos(angleRad), 0f, Mathf.Sin(angleRad)) * 9;
            Vector3 spawnPos = center + offset;

            // Face outward from center (boat forward points away from boss)
            Quaternion rot = Quaternion.LookRotation(offset.normalized, Vector3.up);

            boats[i] = Object.Instantiate(boss.boatShieldProj, spawnPos, rot);

            boats[i].transform.SetParent(boss.transform, true);

            // Optional: if each boat is a projectile with health
            // var dmg = boats[i].GetComponent<Damageable>();
            // if (dmg) dmg.SetHealth(projHealth); // depends on your system
        }

        Vector3 spawn = new Vector3(center.x, 0, center.z);
        shield = Object.Instantiate(boss.actualShield, spawn, Quaternion.identity);
        shield.transform.SetParent(boss.transform, true);

        if (shield.TryGetComponent<BoatDamageable>(out BoatDamageable damageable))
        { 
            damageable.health = projHealth;
            damageable.boats = boats;
        }
        boss.shielded = true;
        boss.StartCoroutine(SpinBoats());
    }
    public override void Execute()
    {
        if (!comboChecked && timer >= 0.5f)
        {
            comboChecked = true;
            AnimEvent("comboCheck");
            isFinished = true;
        }
    }
    public override void End()
    {
        Debug.Log("Ending BoatShield");
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

                if (!LOS) { nextMoveId = boss.mm.Choose("wideWaterBlast", "null"); }
                else if (close) { nextMoveId = boss.mm.Choose("posMelee", "wideWaterBlast", "posMelee"); }
                else if (far) { nextMoveId = boss.mm.Choose("waterBlast", "boatShield", "null"); }
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
        return new BoatShield(this.boss, this.projHealth);
    }

    private IEnumerator SpinBoats()
    {
        int n = boats.Length;

        // Use the initial offset as a base. (If you want a random start, set angleOffsetDeg randomly)
        float baseAngleDeg = 0;

        while (boss != null) // optionally also check boss health/alive
        {
            baseAngleDeg += 30 * Time.deltaTime;

            Vector3 center = boss.transform.position;
            center.y += 0;

            for (int i = 0; i < n; i++)
            {
                GameObject boat = boats[i];
                if (boat == null) continue; // leave a gap if missing/destroyed

                float angleDeg = baseAngleDeg + (360f / n) * i;
                float angleRad = angleDeg * Mathf.Deg2Rad;

                Vector3 offset = new Vector3(Mathf.Cos(angleRad), 0f, Mathf.Sin(angleRad)) * 9;
                Vector3 pos = center + offset;

                boat.transform.position = pos;

                // Keep facing boss
                Vector3 toBoss = (center - pos);
                if (toBoss.sqrMagnitude > 1e-6f)
                    boat.transform.rotation = Quaternion.LookRotation(toBoss.normalized, Vector3.up);
            }

            yield return null;
        }
    }
}
