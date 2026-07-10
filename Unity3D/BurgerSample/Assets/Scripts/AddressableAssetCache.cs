using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public sealed class AddressableAssetCache : MonoBehaviour
{
    private interface ICacheEntry
    {
        void Release();
    }

    private readonly struct CacheKey : IEquatable<CacheKey>
    {
        private readonly object key;
        private readonly Type assetType;
        private readonly bool loadMultiple;

        public CacheKey(object key, Type assetType, bool loadMultiple)
        {
            this.key = key;
            this.assetType = assetType;
            this.loadMultiple = loadMultiple;
        }

        public bool Equals(CacheKey other)
        {
            return Equals(key, other.key) && assetType == other.assetType &&
                loadMultiple == other.loadMultiple;
        }

        public override bool Equals(object obj)
        {
            return obj is CacheKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = ((key != null ? key.GetHashCode() : 0) * 397) ^ assetType.GetHashCode();
                return (hashCode * 397) ^ loadMultiple.GetHashCode();
            }
        }
    }

    private sealed class AssetCacheEntry<T> : ICacheEntry
    {
        private readonly AsyncOperationHandle<T> handle;

        public AssetCacheEntry(object key)
        {
            handle = Addressables.LoadAssetAsync<T>(key);
        }

        public void GetAsset(Action<T> onCompleted)
        {
            if (handle.IsDone)
            {
                Complete(onCompleted);
                return;
            }

            handle.Completed += _ => Complete(onCompleted);
        }

        public void Release()
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }

        private void Complete(Action<T> onCompleted)
        {
            T asset = handle.Status == AsyncOperationStatus.Succeeded
                ? handle.Result
                : default;
            onCompleted?.Invoke(asset);
        }
    }

    private sealed class AssetsCacheEntry<T> : ICacheEntry
    {
        private readonly AsyncOperationHandle<IList<T>> handle;

        public AssetsCacheEntry(object key)
        {
            handle = Addressables.LoadAssetsAsync<T>(key, null);
        }

        public void GetAssets(Action<IList<T>> onCompleted)
        {
            if (handle.IsDone)
            {
                Complete(onCompleted);
                return;
            }

            handle.Completed += _ => Complete(onCompleted);
        }

        public void Release()
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }

        private void Complete(Action<IList<T>> onCompleted)
        {
            IList<T> assets = handle.Status == AsyncOperationStatus.Succeeded
                ? handle.Result
                : null;
            onCompleted?.Invoke(assets);
        }
    }

    private static AddressableAssetCache instance;

    private readonly Dictionary<CacheKey, ICacheEntry> entries =
        new Dictionary<CacheKey, ICacheEntry>();

    public static AddressableAssetCache Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject cacheObject = new GameObject(nameof(AddressableAssetCache));
                instance = cacheObject.AddComponent<AddressableAssetCache>();
                DontDestroyOnLoad(cacheObject);
            }

            return instance;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        instance = null;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadAssets<T>(object key, Action<IList<T>> onCompleted)
    {
        if (key == null)
        {
            Debug.LogError("An Addressables key is required.", this);
            onCompleted?.Invoke(null);
            return;
        }

        CacheKey cacheKey = new CacheKey(key, typeof(T), true);
        if (!entries.TryGetValue(cacheKey, out ICacheEntry entry))
        {
            entry = new AssetsCacheEntry<T>(key);
            entries.Add(cacheKey, entry);
        }

        ((AssetsCacheEntry<T>)entry).GetAssets(onCompleted);
    }

    public void LoadAsset<T>(object key, Action<T> onCompleted)
    {
        if (key == null)
        {
            Debug.LogError("An Addressables key is required.", this);
            onCompleted?.Invoke(default);
            return;
        }

        CacheKey cacheKey = new CacheKey(key, typeof(T), false);
        if (!entries.TryGetValue(cacheKey, out ICacheEntry entry))
        {
            entry = new AssetCacheEntry<T>(key);
            entries.Add(cacheKey, entry);
        }

        ((AssetCacheEntry<T>)entry).GetAsset(onCompleted);
    }

    public bool ReleaseAsset<T>(object key)
    {
        return Release(new CacheKey(key, typeof(T), false));
    }

    public bool ReleaseAssets<T>(object key)
    {
        return Release(new CacheKey(key, typeof(T), true));
    }

    private bool Release(CacheKey cacheKey)
    {
        if (!entries.TryGetValue(cacheKey, out ICacheEntry entry))
            return false;

        entry.Release();
        entries.Remove(cacheKey);
        return true;
    }

    public void ReleaseAll()
    {
        foreach (ICacheEntry entry in entries.Values)
            entry.Release();

        entries.Clear();
    }

    private void OnDestroy()
    {
        if (instance != this)
            return;

        ReleaseAll();
        instance = null;
    }
}
