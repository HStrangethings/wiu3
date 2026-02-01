using System.Collections;
using UnityEngine;

public class BossDamageable : Damageable
{
    private Animator animator;
    private Coroutine stunCoroutine;
    private BossBehaviour boss;
    private BossStats stats;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        boss = GetComponent<BossBehaviour>();
        stats = boss.boss;
    }

    public override void TryTakeDamage(DamageInfo info)
    {
        stats.health -= 50;
        stats.RecalcHealth01();
        if (stats.health <= 0)
        {
            Death();
        }
    }

    public override void Death()
    {
    }
}
