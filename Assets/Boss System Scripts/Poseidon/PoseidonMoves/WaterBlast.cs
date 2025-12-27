using System.Threading;
using UnityEngine;

public class WaterBlast : BossMove
{
    private PoseidonBoss boss;
    private float projSpeed;
    public WaterBlast(PoseidonBoss boss, float projSpeed) : base(boss)
    {
        this.boss = boss;
        this.projSpeed = projSpeed;
    }
    private float timer = 0;
    private GameObject proj;
    public override void Start()
    {
        Debug.Log("Starting WaterBlast");
        //s
        proj = Object.Instantiate(boss.waterBlastProj, boss.transform.position + boss.transform.forward * 9, Quaternion.LookRotation(boss.transform.forward));
    }
    public override void Execute()
    {
        if (timer < 3) { timer += Time.deltaTime; }
        else
        {
            if (proj != null)
            {
                var projRb = proj.GetComponent<Rigidbody>();
                projRb.AddForce(projRb.transform.forward * projSpeed, ForceMode.Impulse);
            }
            isFinished = true;
        }
    }
    public override void End()
    {
        Debug.Log("Ending WaterBlast");
        base.End();
    }
    public override void AnimEvent(string evt)
    {
        switch (evt)
        {
            case "start":
                //spawn projectile to start its own animation
                break;

            case "end":
                var projRb = proj.GetComponent<Rigidbody>();
                projRb.AddForce(projRb.transform.forward * projSpeed, ForceMode.Impulse);
                isFinished = true;
                break;
        }
    }

    public override BossMove Clone()
    {
        return new WaterBlast(this.boss, this.projSpeed);
    }

}
