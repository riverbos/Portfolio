using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Burger/Player Config")]
public class PlayerConfig : ScriptableObject
{
    [Min(0.1f)]
    public float moveSpeed = 5f;
    [Min(0.1f)]
    public float rotationSpeed = 10f;
    [Min(1)]
    public int maxItemsInHand = 3;
    [Min(0f)]
    public float stackHeight = 0.15f;
    [Min(0.1f)]
    public float interactionDistance = 1.5f;
    [Min(0f)]
    public float interactionCooldown = 0.5f;

    [Header("파워업")]
    [Min(1f)]
    public float speedBoostMultiplier = 2f;
    [Min(0f)]
    public float speedBoostDuration = 5f;
    [Min(0)]
    public int capacityBoostAmount = 2;
    [Min(0f)]
    public float capacityBoostDuration = 5f;

    private void OnValidate()
    {
        moveSpeed = Mathf.Max(0.1f, moveSpeed);
        rotationSpeed = Mathf.Max(0.1f, rotationSpeed);
        maxItemsInHand = Mathf.Max(1, maxItemsInHand);
        stackHeight = Mathf.Max(0f, stackHeight);
        interactionDistance = Mathf.Max(0.1f, interactionDistance);
        interactionCooldown = Mathf.Max(0f, interactionCooldown);
        speedBoostMultiplier = Mathf.Max(1f, speedBoostMultiplier);
        speedBoostDuration = Mathf.Max(0f, speedBoostDuration);
        capacityBoostAmount = Mathf.Max(0, capacityBoostAmount);
        capacityBoostDuration = Mathf.Max(0f, capacityBoostDuration);
    }
}
