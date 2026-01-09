using TMPro;
using UnityEngine;

public class textUI : MonoBehaviour
{
    public PlayerController playerController;
    TextMeshProUGUI stateUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stateUI = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
