using UnityEngine;

[CreateAssetMenu(fileName = "BossStats", menuName = "Scriptable Objects/BossStats")]
public class BossStats : ScriptableObject
{
    public string bossName;
    public float baseHealth;
    public float baseAtk;
    public float baseSpeed;

    [Space]
    public float health01;
    public float health;
    public float atk;
    public float speed;

    //use this to offset spawn proj positions so it doesnt spawn inside boss
    public float bossRad;
    public float bossMeleeReach;

    public void RecalcHealth01()
    {
        health01 = baseHealth <= 0f ? 0f : health / baseHealth;
    }
}
