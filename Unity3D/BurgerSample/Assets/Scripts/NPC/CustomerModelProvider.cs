using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class CustomerModelProvider
{
    private const string CustomerModelLabel = "CustomerModel";

    public static CustomerModelProvider Instance { get; } = new CustomerModelProvider();

    private CustomerModelProvider()
    {
    }

    public void RequestRandomModel(Action<GameObject> onCompleted)
    {
        if (onCompleted == null)
            return;

        AddressableAssetCache.Instance.LoadAssets<GameObject>(
            CustomerModelLabel,
            models => onCompleted(GetRandomModel(models)));
    }

    private static GameObject GetRandomModel(IList<GameObject> models)
    {
        if (models == null || models.Count == 0)
        {
            Debug.LogError($"Addressables label '{CustomerModelLabel}' did not load any customer models.");
            return null;
        }

        return models[UnityEngine.Random.Range(0, models.Count)];
    }
}
