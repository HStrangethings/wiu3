using UnityEngine;

public class PhoenixAttackMelee : BossState
{
    private bool started;

    public PhoenixAttackMelee(BossStateMachine sm, BossBehaviour boss) : base(sm, boss) { }

    public override void Enter()
    {
        started = false;
        boss.rb.linearVelocity = Vector3.zero;
    }

    public static void PushCCSideways(CharacterController cc, Vector3 bossCenter, float bossRadius, float pushSpeed)
    {
        Vector3 p = cc.transform.position;

        // horizontal delta only
        Vector3 d = p - bossCenter;
        d.y = 0f;

        float dist = d.magnitude;
        if (dist < 0.0001f) d = Vector3.forward;

        // if inside radius, push out
        if (dist < bossRadius)
        {
            Vector3 dir = d.normalized;
            float penetration = bossRadius - dist;

            Vector3 push = dir * (penetration * pushSpeed) * Time.deltaTime;

            // tiny down bias prevents "ride up"
            push.y = -0.2f * Time.deltaTime;

            cc.Move(push);
        }
    }

    public override void Execute()
    {
        boss.rb.linearVelocity = Vector3.zero;
        boss.transform.rotation = boss.RotateToPlayer();

        if (!started)
        {
            started = true;
            boss.mm.PlayMove("EnergySlash");
        }
    }

    public override void ComboFin()
    {
        sm.ChangeState<PhoenixIdle>();
    }

    public override void Exit() { }
}