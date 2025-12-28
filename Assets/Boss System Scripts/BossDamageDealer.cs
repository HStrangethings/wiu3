using System;
using UnityEngine;

public class BossDamageDealer : DamageDealer, IBossHitReporter
{
    public Type sourceMove;
    public BossMoveMachine mm;

    //use interface so hitbox groups dont need to keep getting component, just check for interface
    public void SetMoveMachine(BossMoveMachine mm) => this.mm = mm;
    public void SetMoveType(Type moveType) => sourceMove = moveType;

    public override void OnHit(Vector3 hitPoint, Vector3 normal, Damageable dmg)
    {
        base.OnHit(hitPoint, normal, dmg);
        mm?.ReportHit(sourceMove);
    }
}
