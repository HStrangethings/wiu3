using UnityEngine;

[CreateAssetMenu(fileName = "NewGameEvent", menuName = "Game/Event")]
public class GameEventSO : ScriptableObject
{
    [Header("Event Info")]
    public string eventName;
    [TextArea(2, 5)]
    public string description;

    [Header("Spawn Settings")]
    [Range(0f, 1f)]
    public float chance = 0.3f;   
}
