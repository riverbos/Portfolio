using System.Collections;
using System.Collections.Generic;
using RKit.ActionSpot;
using UnityEngine;

public class Counter : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private CounterConfig config;
    [SerializeField] private Transform servingPoint;

    [Header("Visual Feedback")]
    [SerializeField] private bool autoSellBurgers = true;
    [SerializeField] private GameObject sellEffectPrefab;

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;

    private readonly List<GameObject> burgersOnCounter = new List<GameObject>();
    private float nextSellTime;

    private void Start()
    {
        if (servingPoint == null)
        {
            GameObject servingObj = new GameObject("ServingPoint");
            servingObj.transform.SetParent(transform);
            servingObj.transform.localPosition = new Vector3(0, 1f, 0);
            servingPoint = servingObj.transform;
        }

        SetNextSellTime();
    }

    private void Update()
    {
        if (autoSellBurgers && Time.time >= nextSellTime)
        {
            if (burgersOnCounter.Count > 0)
                SellBurger();

            SetNextSellTime();
        }

#if UNITY_EDITOR
        if (debugMode && Input.GetKeyDown(KeyCode.F3))
            Debug.Log($"Counter state: burgers {burgersOnCounter.Count}, money {PlayerData.Money}, next sell in {nextSellTime - Time.time:F1}s");
#endif
    }

    public bool CanInteract(PlayerHand hand) => !hand.IsEmpty() && !IsFull();

    public void Interact(PlayerHand hand)
    {
        GameObject item = hand.TakeItem();
        if (item != null && !AddBurger(item))
            hand.AddItem(item);
    }

    public bool AddBurger(GameObject burger)
    {
        if (IsFull())
            return false;

        Vector3 burgerPosition = servingPoint.position + Vector3.up * (config.stackHeight * burgersOnCounter.Count);
        burger.transform.SetParent(transform);
        burgersOnCounter.Add(burger);
        StartCoroutine(MoveBurgerToPosition(burger, burgerPosition, 0.1f));
        return true;
    }

    public bool IsFull() => burgersOnCounter.Count >= config.maxBurgersOnCounter;

    public long GetCoin() => PlayerData.Money;

    private void SellBurger()
    {
        if (burgersOnCounter.Count == 0)
            return;

        GameObject burger = burgersOnCounter[0];
        burgersOnCounter.RemoveAt(0);

        if (burger != null)
        {
            ShowSellEffect(burger.transform.position);
            BurgerPool.Instance.Return(burger);
            PlayerData.AddResource(ResourceType.Money, config.pointsPerBurger);
        }

        for (int i = 0; i < burgersOnCounter.Count; i++)
        {
            if (burgersOnCounter[i] != null)
            {
                Vector3 newPosition = servingPoint.position + Vector3.up * (config.stackHeight * i);
                StartCoroutine(MoveBurgerToPosition(burgersOnCounter[i], newPosition, 0.1f));
            }
        }
    }

    private void ShowSellEffect(Vector3 position)
    {
        if (sellEffectPrefab == null)
            return;

        GameObject effect = Instantiate(sellEffectPrefab, position, Quaternion.identity);
        Destroy(effect, 2f);
    }

    private void SetNextSellTime()
    {
        float variation = Random.Range(-config.sellIntervalVariation, config.sellIntervalVariation);
        float interval = Mathf.Max(0.5f, config.sellInterval + variation);
        nextSellTime = Time.time + interval;
    }

    private IEnumerator MoveBurgerToPosition(GameObject burger, Vector3 targetPosition, float duration)
    {
        if (burger == null)
            yield break;

        Vector3 startPosition = burger.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (burger == null)
                yield break;

            burger.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (burger != null)
            burger.transform.position = targetPosition;
    }
}
