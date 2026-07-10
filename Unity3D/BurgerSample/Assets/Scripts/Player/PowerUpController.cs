using UnityEngine;
using UnityEngine.InputSystem;

public class PowerUpController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerHand playerHand;

    private InputAction speedBoostAction;
    private InputAction capacityBoostAction;

    private void Awake()
    {
        if (playerController == null)
            playerController = GetComponent<PlayerController>();

        if (playerHand == null)
            playerHand = GetComponent<PlayerHand>();

        if (playerController == null || playerHand == null)
        {
            Debug.LogError($"{nameof(PowerUpController)} requires PlayerController and PlayerHand references.", this);
            enabled = false;
            return;
        }

        speedBoostAction = new InputAction("Speed Boost", binding: "<Keyboard>/g");
        capacityBoostAction = new InputAction("Capacity Boost", binding: "<Keyboard>/h");
    }

    private void OnEnable()
    {
        if (speedBoostAction == null || capacityBoostAction == null)
            return;

        speedBoostAction.performed += HandleSpeedBoost;
        capacityBoostAction.performed += HandleCapacityBoost;
        speedBoostAction.Enable();
        capacityBoostAction.Enable();
    }

    private void OnDisable()
    {
        if (speedBoostAction == null || capacityBoostAction == null)
            return;

        speedBoostAction.Disable();
        capacityBoostAction.Disable();
        speedBoostAction.performed -= HandleSpeedBoost;
        capacityBoostAction.performed -= HandleCapacityBoost;
    }

    private void OnDestroy()
    {
        speedBoostAction?.Dispose();
        capacityBoostAction?.Dispose();
    }

    private void HandleSpeedBoost(InputAction.CallbackContext context)
    {
        playerController.ActivateSpeedBoost();
    }

    private void HandleCapacityBoost(InputAction.CallbackContext context)
    {
        playerHand.ActivateCapacityBoost();
    }
}
