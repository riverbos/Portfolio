using RKit.ActionSpot;
using UnityEngine;

public class PlayerActionSpotResourceProvider : MonoBehaviour, IActionSpotResourceProvider
{
    public bool ConsumeResource(ResourceType resourceType)
    {
        return PlayerData.TryConsumeResource(resourceType);
    }
}
