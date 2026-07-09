using System;
using RKit.ActionSpot;

public static class PlayerData
{
    public static event Action<ResourceType, long> ResourceChanged;

    public static long Money { get; private set; } = 100;
    public static long Jewel { get; private set; } = 0;

    public static long GetResource(ResourceType resourceType)
    {
        return resourceType switch
        {
            ResourceType.Money => Money,
            ResourceType.Jewel => Jewel,
            _ => 0
        };
    }

    public static void SetResource(ResourceType resourceType, long amount)
    {
        amount = Math.Max(0, amount);

        switch (resourceType)
        {
            case ResourceType.Money:
                Money = amount;
                break;
            case ResourceType.Jewel:
                Jewel = amount;
                break;
            default:
                return;
        }

        ResourceChanged?.Invoke(resourceType, amount);
    }

    public static void AddResource(ResourceType resourceType, long amount)
    {
        if (amount <= 0)
            return;

        SetResource(resourceType, GetResource(resourceType) + amount);
    }

    public static bool TryConsumeResource(ResourceType resourceType, long amount = 1)
    {
        if (amount <= 0)
            return true;

        long currentAmount = GetResource(resourceType);
        if (currentAmount < amount)
            return false;

        SetResource(resourceType, currentAmount - amount);
        return true;
    }
}
