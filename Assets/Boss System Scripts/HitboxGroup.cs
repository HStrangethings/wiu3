using UnityEngine;

public class HitboxGroup : MonoBehaviour
{
    public HitboxID id;

    private DamageDealer[] hitboxes;

    private void Start()
    {
        hitboxes = GetComponentsInChildren<DamageDealer>();
    }

    public void SetEnabled(bool enabled)
    {
        foreach(DamageDealer d in hitboxes) { d.isActive = enabled; }
    }
}
