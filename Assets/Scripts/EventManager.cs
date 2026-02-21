using UnityEngine;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    public List<GameEventSO> events;
    public EventPopupUI popupUI;

    [Header("Frequency Settings")]
    public float checkInterval = 10f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= checkInterval)
        {
            TryTriggerEvent();
            timer = 0f;
        }
    }

    void TryTriggerEvent()
    {
        foreach (GameEventSO e in events)
        {
            if (Random.value <= e.chance)
            {
                popupUI.ShowEvent(e);
                return; 
            }
        }
    }
}
