using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Burger/Player Config")]
public class PlayerConfig : ScriptableObject
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public int maxItemsInHand = 3;
    public float stackHeight = 0.15f;
    public float interactionDistance = 1.5f;
    public float interactionCooldown = 0.5f;

    [Header("파워업")]
    public float speedBoostMultiplier = 2f;
    public float speedBoostDuration = 5f;
    public int capacityBoostAmount = 2;
    public float capacityBoostDuration = 5f;
}
