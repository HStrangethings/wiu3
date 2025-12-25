using System.Collections;
using UnityEngine;

public class PlayerDamageable : Damageable
{
    Renderer objectRenderer;
    private Color originalColor;
    public Color damageColor = Color.red;
    public float damageEffectDuration = 0.5f;
    private Coroutine damageCoroutine;

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
        health -= info.damageAmt;
        if (health <= 0)
        {
            Death();
        }
    }

    public override void Death()
    {
    }
}
