using UnityEngine;
using UnityEngine.InputSystem;

public class PowerUpController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerHand playerHand;

    private void Update()
    {
        if (Keyboard.current.sKey.wasPressedThisFrame)
            playerController.ActivateSpeedBoost();

        if (Keyboard.current.hKey.wasPressedThisFrame)
            playerHand.ActivateCapacityBoost();
    }
}
