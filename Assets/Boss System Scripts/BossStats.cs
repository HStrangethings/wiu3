using UnityEngine;

[CreateAssetMenu(fileName = "BossStats", menuName = "Scriptable Objects/BossStats")]
public class BossStats : ScriptableObject
{
    public string bossName;
    public int health;
    public int maxHealth;
    public int atk;
    public int speed;
}
