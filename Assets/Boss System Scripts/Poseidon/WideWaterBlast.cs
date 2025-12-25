using System.Threading;
using UnityEngine;

public class WideWaterBlast : BossMove
{
    private PoseidonBoss boss;
    private float projSpeed;
    public WideWaterBlast(PoseidonBoss boss, float projSpeed) : base(boss)
    {
        this.boss = boss;
        this.projSpeed = projSpeed;
    }
    private float timer = 0;
    private GameObject[] proj = new GameObject[3];
    private bool[] projCreated = new bool[3];
    public override void Start()
    {
        Debug.Log("Starting WideWaterBlast");
        //proj = Object.Instantiate(boss.waterBlastProj, boss.transform.position + boss.transform.forward * 1, Quaternion.LookRotation(boss.transform.forward));
    }
    public override void Execute()
    {
        if (timer < 5) { 
            timer += Time.deltaTime;
            float timeIntervals = 0.2f;
            if (timer > timeIntervals && !projCreated[0])
            {
                projCreated[0] = true;
                Vector3 localOffset = new Vector3(1, 0, 1);
                Vector3 spawn = boss.transform.TransformPoint(localOffset);
                proj[0] = Object.Instantiate(boss.waterBlastProj, spawn, boss.transform.rotation,boss.transform);
                proj[0].transform.localRotation *= Quaternion.Euler(0f, 25f, 0f);
            }
            if (timer > timeIntervals * 2 && !projCreated[1])
            {
                projCreated[1] = true;
                Vector3 localOffset = new Vector3(0, 0, 1);
                Vector3 spawn = boss.transform.TransformPoint(localOffset);
                proj[1] = Object.Instantiate(boss.waterBlastProj, spawn, boss.transform.rotation, boss.transform);
                proj[1].transform.localRotation *= Quaternion.Euler(0f, 0f, 0f);
            }
            if (timer > timeIntervals * 3 && !projCreated[2])
            {
                projCreated[2] = true;
                Vector3 localOffset = new Vector3(-1, 0, 1);
                Vector3 spawn = boss.transform.TransformPoint(localOffset);
                proj[2] = Object.Instantiate(boss.waterBlastProj, spawn, boss.transform.rotation, boss.transform);
                proj[2].transform.localRotation *= Quaternion.Euler(0f, -25f, 0f);
            }
        }
        else
        {
            foreach (GameObject p in proj)
            {
                var projRb = p.GetComponent<Rigidbody>();
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
}
