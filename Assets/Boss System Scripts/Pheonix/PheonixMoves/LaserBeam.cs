using UnityEngine;

public class LaserBeam : BossMove
{
    private PhoenixBoss boss;
    private float chargeUpTime;

    private GameObject proj;

    public LaserBeam(PhoenixBoss boss, float chargeUpTime) : base(boss)
    {
        this.boss = boss;
        this.chargeUpTime = chargeUpTime;
    }

    public override void Start()
    {
        isFinished = false;
        proj = null;

        // Laser animation will call AE_LaserFreeze() which spawns laser during freeze
        boss.animator.Play("LaserBeamAnim");
    }

    public override void Execute()
    {
        // no timers; freeze routine controls start/end
    }

    public override void AnimEvent(string evt)
    {
        switch (evt)
        {
            case "start":
                {
                    if (proj != null) return;

                    Transform sp = boss.laserSpawnPoint != null ? boss.laserSpawnPoint : boss.transform;

                    proj = Object.Instantiate(
                        boss.laserBeam,
                        sp.position,
                        sp.rotation
                    );

                    SetupBossHitReporting(proj);
                    break;
                }

            case "end":
                {
                    if (proj != null)
                    {
                        Object.Destroy(proj);
                        proj = null;
                    }

                    isFinished = true;
                    break;
                }
        }
    }

    public override BossMove Clone()
    {
        return new LaserBeam(this.boss, this.chargeUpTime);
    }
}