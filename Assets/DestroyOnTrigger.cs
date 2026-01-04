using UnityEngine;

public class DestroyOnTrigger : MonoBehaviour, ITriggerReceiver
{
    public void OnTrigger(Collider other)
    {
        Destroy(this.gameObject);
    }
}
