using System;
using UnityEngine;

public interface IBossHitReporter
{
    void SetMoveMachine(BossMoveMachine mm);
    void SetMoveType(Type moveType);
}
