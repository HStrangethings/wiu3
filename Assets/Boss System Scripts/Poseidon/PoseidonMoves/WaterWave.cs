using UnityEngine;

public class WaterWave : BossMove
{
    private PoseidonBoss boss;
    private float projSpeed;
    private float xSize;
    private float chargeUpTime;
    public WaterWave(PoseidonBoss boss, float projSpeed,float xSize,float chargeUpTime) : base(boss)
    {
        this.boss = boss;
        this.projSpeed = projSpeed;
        this.xSize = xSize;
        this.chargeUpTime = chargeUpTime;
    }
    private float timer = 0;
    private GameObject proj;
    public override void Start()
    {
        Debug.Log("Starting WaterWave");
        proj = Object.Instantiate(boss.waterWaveProj, boss.transform.position + boss.transform.forward * 7, Quaternion.LookRotation(boss.transform.forward));
        proj.transform.localScale = new Vector3(xSize, 0, boss.waterWaveProj.transform.localScale.z);
        SetupBossHitReporting(proj);
    }
    public override void Execute()
    {
        if (timer < 3)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer);
            float scaleY = Mathf.Lerp(0, boss.waterWaveProj.transform.localScale.y, t);
            proj.transform.localScale = new Vector3(xSize, scaleY, boss.waterWaveProj.transform.localScale.z);
        }
        else
        {
            proj.transform.localScale = new Vector3(xSize, boss.waterWaveProj.transform.localScale.y, boss.waterWaveProj.transform.localScale.z);
            var projRb = proj.GetComponent<Rigidbody>();
            projRb.AddForce(projRb.transform.forward * projSpeed, ForceMode.Impulse);
            isFinished = true;
        }
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
            
            case"end":
                var projRb = proj.GetComponent<Rigidbody>();
                projRb.AddForce(projRb.transform.forward * projSpeed, ForceMode.Impulse);
                isFinished = true;
                break;
        }
    }

    public override BossMove Clone()
    {
        return new WaterWave(this.boss, this.projSpeed,this.xSize,this.chargeUpTime);
    }

}
