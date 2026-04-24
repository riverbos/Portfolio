using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 카운터 클래스.
public class Counter : MonoBehaviour, IInteractable
{
    [Header("설정")]
    [SerializeField] private CounterConfig config;
    [SerializeField] private Transform servingPoint;

    [Header("시각적 피드백")]
    [SerializeField] private bool autoSellBurgers = true;
    [SerializeField] private GameObject sellEffectPrefab;

    [Header("디버그")]
    [SerializeField] private bool debugMode = false;

    private List<GameObject> burgersOnCounter = new List<GameObject>();
    private float nextSellTime;
    private int totalCoin = 0;

    private void Start()
    {
        // 버거를 올리는 지점
        if (servingPoint == null)
        {
            GameObject servingObj = new GameObject("ServingPoint");
            servingObj.transform.SetParent(transform);
            servingObj.transform.localPosition = new Vector3(0, 1f, 0);
            servingPoint = servingObj.transform;
        }

        // 다음 판매 시간 초기화
        SetNextSellTime();
    }

    private void Update()
    {
        // 자동 판매 기능
        if (autoSellBurgers && Time.time >= nextSellTime)
        {
            if (burgersOnCounter.Count > 0)
                SellBurger();

            SetNextSellTime();
        }

#if UNITY_EDITOR
        if (debugMode && Input.GetKeyDown(KeyCode.F3))
            Debug.Log($"카운터 상태: 버거 {burgersOnCounter.Count}개, 점수 {totalCoin}, 다음 판매까지 {nextSellTime - Time.time:F1}초");
#endif
    }

    // IInteractable 구현 : 플레이어가 상호작용힐 떼 버거를 놓을 수 있는지 체크
    public bool CanInteract(PlayerHand hand) => !hand.IsEmpty() && !IsFull();
    // IInteractable 구현 : 플레이어가 상호작용할 때 버거를 놓습니다. (실패하면 버거는 PlayerHand에 돌아갑니다.)
    public void Interact(PlayerHand hand)
    {
        GameObject item = hand.TakeItem();
        if (item != null && !AddBurger(item))
            hand.AddItem(item);
    }

    // 버거를 카운터에 추가하는 메서드.
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

    // 카운터가 가득 찼는지 여부를 반환
    public bool IsFull() => burgersOnCounter.Count >= config.maxBurgersOnCounter;

    // 현재 코인을 반환하는 메서드
    public int GetCoin() => totalCoin;

    // 버거를 판매하는 메서드입니다.
    private void SellBurger()
    {
        if (burgersOnCounter.Count == 0)
            return;

        // 맨 밑에 있는 버거를 판매합니다.
        GameObject burger = burgersOnCounter[0];
        burgersOnCounter.RemoveAt(0);
        if (burger != null)
        {
            ShowSellEffect(burger.transform.position);
            BurgerPool.Instance.Return(burger);

            totalCoin += config.pointsPerBurger;
            HUD_Panel.Instance.UpdateCoinText(GetCoin());
        }

        // 남은 버거들을 아래로 이동시킵니다.
        for (int i = 0; i < burgersOnCounter.Count; i++)
        {
            if (burgersOnCounter[i] != null)
            {
                Vector3 newPosition = servingPoint.position + Vector3.up * (config.stackHeight * i);
                StartCoroutine(MoveBurgerToPosition(burgersOnCounter[i], newPosition, 0.1f));
            }
        }
    }

    // 판매 효과를 보여주는 메서드입니다.
    private void ShowSellEffect(Vector3 position)
    {
        if (sellEffectPrefab == null) return;
        GameObject effect = Instantiate(sellEffectPrefab, position, Quaternion.identity);
        Destroy(effect, 2f);
    }

    // 다음 판매 시간을 설정하는 메서드입니다.
    private void SetNextSellTime()
    {
        float variation = Random.Range(-config.sellIntervalVariation, config.sellIntervalVariation);
        float interval = Mathf.Max(0.5f, config.sellInterval + variation);
        nextSellTime = Time.time + interval;
    }

    // 버거를 부드럽게 이동시키는 코루틴입니다.
    private IEnumerator MoveBurgerToPosition(GameObject burger, Vector3 targetPosition, float duration)
    {
        if (burger == null) yield break;

        Vector3 startPosition = burger.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (burger == null) yield break;
            burger.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (burger != null)
            burger.transform.position = targetPosition;
    }
}
