using System.Collections;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    public float seconds;

    private void Start()
    {
        StartCoroutine(WaitForDestroy());
    }

    public IEnumerator WaitForDestroy()
    {
        yield return new WaitForSeconds(seconds);
        Destroy(this.gameObject);
    }
}
