using System;
using RKit.ActionSpot;
using UnityEngine;

public static class PlayerData
{
    public const long DefaultMoney = 170;
    public const long DefaultJewel = 0;

    public static event Action<ResourceType, long> ResourceChanged;

    public static long Money { get; private set; } = DefaultMoney;
    public static long Jewel { get; private set; } = DefaultJewel;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState()
    {
        ResourceChanged = null;
        Money = DefaultMoney;
        Jewel = DefaultJewel;
    }

    public static void ResetToDefaults()
    {
        SetResource(ResourceType.Money, DefaultMoney);
        SetResource(ResourceType.Jewel, DefaultJewel);
    }

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

        long currentAmount = GetResource(resourceType);
        long newAmount = amount > long.MaxValue - currentAmount
            ? long.MaxValue
            : currentAmount + amount;
        SetResource(resourceType, newAmount);
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
