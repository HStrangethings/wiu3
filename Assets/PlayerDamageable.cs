using System.Collections;
using UnityEngine;

public class PlayerDamageable : Damageable
{
    Renderer objectRenderer;
    private Color originalColor;
    public Color damageColor = Color.red;
    public float damageEffectDuration = 0.5f;
    private Coroutine damageCoroutine;

    private PlayerController playerController;
    private PlayerWeaponController weaponController;
    private Animator animator;
    private Coroutine stunCoroutine;

    private IEnumerator DamageEffect()
    {
        objectRenderer.material.color = damageColor;
        float elapsedTime = 0f;
        while (elapsedTime < damageEffectDuration)
        {
            objectRenderer.material.color = Color.Lerp(damageColor, originalColor, elapsedTime / damageEffectDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        objectRenderer.material.color = originalColor;
    }
    private void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }

        playerController = GetComponent<PlayerController>();
        weaponController = GetComponentInChildren<PlayerWeaponController>();
        animator = GetComponentInChildren<Animator>();
    }

    public override void TryTakeDamage(DamageInfo info)
    {
        if (objectRenderer != null)
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
            }
            damageCoroutine = StartCoroutine(DamageEffect());
        }
        Stunned();
        health -= info.damageAmt;
        if (health <= 0)
        {
            Death();
        }
    }

    public override void Death()
    {
    }

    public void Stunned()
    {
        playerController.GetStunned();
        weaponController.StunnedAttack();
        animator.Play("Rib Hit");
        if (stunCoroutine != null) StopCoroutine(stunCoroutine);
        stunCoroutine = StartCoroutine(WaitForStunAnimThenRecover("Rib Hit", 0));
    }

    public void RecoverStunned()
    {
        weaponController.RecoverAttack();
        playerController.ToIdleState();
        animator.Play("Sword And Shield Idle 1");
    }

    private IEnumerator WaitForStunAnimThenRecover(string stateName, int layer)
    {
        // 1) Wait until we are actually in the stun state (handles transitions)
        while (true)
        {
            var st = animator.GetCurrentAnimatorStateInfo(layer);
            if (st.IsName(stateName)) break;
            yield return null;
        }

        // 2) Wait until the stun state finishes (and we're not transitioning out)
        while (true)
        {
            var st = animator.GetCurrentAnimatorStateInfo(layer);

            // If your stun clip loops, this will never end — ensure Loop Time is OFF for "Rib Hit".
            if (st.IsName(stateName) && st.normalizedTime >= 1f && !animator.IsInTransition(layer))
                break;

            yield return null;
        }

        RecoverStunned();
        stunCoroutine = null;
    }

}
