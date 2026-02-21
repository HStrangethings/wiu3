using UnityEngine;

public class AerialSlash : BossMove
{
    private PhoenixBoss boss;
    private float speed;

    private GameObject proj;

    public AerialSlash(PhoenixBoss boss, float speed) : base(boss)
    {
        this.boss = boss;
        this.speed = speed;
    }

    public override void Start()
    {
        isFinished = false;
        proj = null;

        boss.animator.Play("AerialSlashAnim");
    }

    public override void Execute()
    {
        // animation events control spawn and end
    }

    public override void AnimEvent(string evt)
    {
        switch (evt)
        {
            case "spawn":
                {
                    if (proj != null) return;

                    if (boss.aerialSlashPrefab == null)
                    {
                        Debug.LogWarning("AerialSlash: aerialSlashPrefab is not assigned on PhoenixBoss.");
                        return;
                    }

                    Transform sp = boss.aerialSlashSpawnPoint != null ? boss.aerialSlashSpawnPoint : boss.transform;

                    proj = Object.Instantiate(
                        boss.aerialSlashPrefab,
                        sp.position,
                        sp.rotation
                    );

                    // Initialize projectile motion
                    var p = proj.GetComponent<AerialSlashProjectile>();
                    if (p != null)
                        p.Init(sp.forward, speed);

                    SetupBossHitReporting(proj);
                    break;
                }

            case "end":
                // The projectile self-destroys on hit or after 5s.
                // Ending the move returns control back to state machine.
                isFinished = true;
                break;
        }
    }

    public override BossMove Clone()
    {
        return new AerialSlash(this.boss, this.speed);
    }
}