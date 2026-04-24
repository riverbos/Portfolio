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

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < initialSize; i++)
            pool.Enqueue(CreateBurger());
    }

    public GameObject Get()
    {
        GameObject obj = pool.Count > 0 ? pool.Dequeue() : CreateBurger();
        obj.SetActive(true);
        return obj;
    }

    public void Return(GameObject burger)
    {
        burger.SetActive(false);
        burger.transform.SetParent(transform);
        pool.Enqueue(burger);
    }

    private GameObject CreateBurger()
    {
        GameObject obj = Instantiate(burgerPrefab, transform);
        obj.SetActive(false);
        return obj;
    }
}
