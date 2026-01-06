using Unity.VisualScripting;
using UnityEngine;

public class feetHandler : MonoBehaviour
{
    private Collider _collider;

    private playerController _playerController;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _collider = GetComponent<Collider>();
        _playerController = GetComponentInParent<playerController>();
    }

    // Update is called once per frame
    void Update()   
    {
        
    }

    private void OnTriggerStay(Collider collider)
    {
        Debug.Log("collided with " +  collider.gameObject.name + "on layer " + collider.gameObject.layer );
        if (collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _playerController.onGround = true;
        }
    }

}
