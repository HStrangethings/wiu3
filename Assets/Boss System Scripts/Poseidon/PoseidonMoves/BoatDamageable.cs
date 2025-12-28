using System.Collections;
using System.Linq;
using UnityEngine;

public class BoatDamageable : Damageable
{
    Renderer objectRenderer;
    private Color originalColor;
    public Color damageColor = Color.blue;
    public float damageEffectDuration = 0.5f;
    private Coroutine damageCoroutine;

    private float startHealth;
    [HideInInspector]
    public GameObject[] boats = new GameObject[3];

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
        startHealth = health;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) { var info = new DamageInfo(this.gameObject, "self", 10, Vector3.zero, Vector3.zero, false); TryTakeDamage(info); }
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
        if (health < (2/3f * startHealth))
        { Destroy(boats[2]); }
        if (health < (1/3f * startHealth))
        { Destroy(boats[1]); }
        if (health <= 0)
        {
            Death();
        }
    }

    public override void Death()
    {
        Destroy(boats[0]);
        Destroy(this.gameObject);
    }
}
