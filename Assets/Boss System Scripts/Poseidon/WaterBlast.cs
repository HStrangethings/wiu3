using System.Threading;
using UnityEngine;

public class WaterBlast : BossMove
{
    private float timer = 0;
    public override void Start()
    {
        Debug.Log("Starting WaterBlast");
    }
    public override void Execute()
    {
        Debug.Log("Executing WaterBlast");
        if (timer <= 5f)
        {
            timer += Time.deltaTime;
        }
        else
        {
            isFinished = true;
        }
    }
    public override void End()
    {
        Debug.Log("Ending WaterBlast");
        base.End();
    }
}
