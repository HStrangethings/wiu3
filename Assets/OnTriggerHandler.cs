using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerHandler : MonoBehaviour
{
    [Header("Put scripts in execute order when on trigger event")]
    [SerializeField] private List<MonoBehaviour> scripts = new();

    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < scripts.Count; i++)
        {
            var mb = scripts[i];
            if (mb == null) continue;

            if (mb is ITriggerReceiver receiver)
            {
                receiver.OnTrigger(other);
            }
            // else: ignore scripts that don't use this interface
        }
    }
}
