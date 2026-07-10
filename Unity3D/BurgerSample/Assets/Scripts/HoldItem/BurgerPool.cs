using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 버거 오브젝트 풀. Instantiate/Destroy 대신 재사용하여 GC 부하를 줄입니다.
/// BurgerStove에서 Get(), Counter 판매 시 Return()으로 반납합니다.
/// </summary>
public class BurgerPool : MonoBehaviour
{
    public static BurgerPool Instance { get; private set; }

    [SerializeField] private GameObject burgerPrefab;
    [SerializeField] private int initialSize = 10;

    private readonly Queue<GameObject> pool = new Queue<GameObject>();
    private readonly HashSet<GameObject> pooledObjects = new HashSet<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"Duplicate {nameof(BurgerPool)} was destroyed.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (burgerPrefab == null)
        {
            Debug.LogError($"{nameof(BurgerPool)} requires a burger prefab.", this);
            enabled = false;
            return;
        }

        for (int i = 0; i < Mathf.Max(0, initialSize); i++)
            Enqueue(CreateBurger());
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public GameObject Get()
    {
        if (burgerPrefab == null)
            return null;

        GameObject obj = pool.Count > 0 ? pool.Dequeue() : CreateBurger();
        pooledObjects.Remove(obj);
        ResetBurger(obj);
        obj.SetActive(true);
        return obj;
    }

    public void Return(GameObject burger)
    {
        if (burger == null || pooledObjects.Contains(burger))
            return;

        burger.SetActive(false);
        ResetBurger(burger);
        Enqueue(burger);
    }

    private GameObject CreateBurger()
    {
        GameObject obj = Instantiate(burgerPrefab, transform);
        obj.SetActive(false);
        return obj;
    }

    private void Enqueue(GameObject burger)
    {
        burger.transform.SetParent(transform);
        pool.Enqueue(burger);
        pooledObjects.Add(burger);
    }

    private void ResetBurger(GameObject burger)
    {
        burger.transform.SetParent(transform);
        burger.transform.localPosition = Vector3.zero;
        burger.transform.localRotation = Quaternion.identity;

        Rigidbody body = burger.GetComponent<Rigidbody>();
        if (body != null)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
    }
}
