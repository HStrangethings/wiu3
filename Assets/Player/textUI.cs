using TMPro;
using UnityEngine;

public class textUI : MonoBehaviour
{
     public playerController playerController;
    TextMeshProUGUI stateUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stateUI = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        stateUI.text = playerController.currentState.ToString() +
            "\n onGround: " + playerController.onGround.ToString() +
            "\n vel: " + playerController.vel.ToString("F1");
    }
}
