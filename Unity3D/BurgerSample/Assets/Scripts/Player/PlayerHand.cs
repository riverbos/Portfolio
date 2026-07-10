using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 들고 있는 아이템을 관리합니다.
/// IHoldable 타입 단위로 혼적을 방지합니다. (버거와 쓰레기를 동시에 들 수 없음)
/// 아이템 추가 전 CanAccept()로 수용 가능 여부를 확인해야 합니다.
/// </summary>
public class PlayerHand : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private PlayerConfig config;
    [SerializeField] private Transform handPoint;

    [Header("시각적 피드백")]
    [SerializeField] private bool showStackUI = true;
    [SerializeField] private GameObject itemCountPrefab;

    [Header("디버깅")]
    [SerializeField] private bool debugMode = false;

    private List<GameObject> itemsInHand = new List<GameObject>();
    private Type heldItemType;
    private int bonusCapacity = 0;
    private Coroutine capacityBoostCoroutine;
    private GameObject itemCountUI;
    private TextMesh itemCountText;

    private void Awake()
    {
        if (config != null)
            return;

        Debug.LogError($"{nameof(PlayerHand)} requires a player config.", this);
        enabled = false;
    }

    private void Start()
    {
        if (handPoint == null)
        {
            GameObject handObj = new GameObject("HandPoint");
            handObj.transform.SetParent(transform);
            handObj.transform.localPosition = new Vector3(0, 0.5f, 0.5f);
            handPoint = handObj.transform;
        }

        if (showStackUI && itemCountPrefab != null)
        {
            itemCountUI = Instantiate(itemCountPrefab, handPoint.position + Vector3.up * 0.5f, Quaternion.identity);
            itemCountUI.transform.SetParent(transform);
            itemCountText = itemCountUI.GetComponentInChildren<TextMesh>();
            UpdateCountUI();
            itemCountUI.SetActive(false);
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (debugMode)
            DebugStackPositions();
#endif
        UpdateItemsPosition();
    }

    public bool AddItem(GameObject item)
    {
        if (IsFull()) return false;

        IHoldable holdable = item.GetComponent<IHoldable>();
        if (holdable == null) return false;

        // 다른 타입 혼적 방지
        if (heldItemType != null && holdable.GetType() != heldItemType)
            return false;

        heldItemType = holdable.GetType();
        item.transform.SetParent(handPoint);

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        StartCoroutine(MoveToPosition(item, CalculatePosition(itemsInHand.Count), 0.1f));
        itemsInHand.Add(item);
        UpdateCountUI();
        return true;
    }

    public GameObject TakeItem()
    {
        if (itemsInHand.Count == 0) return null;

        GameObject item = itemsInHand[itemsInHand.Count - 1];
        itemsInHand.RemoveAt(itemsInHand.Count - 1);
        item.transform.SetParent(null);

        if (itemsInHand.Count == 0)
            heldItemType = null;

        UpdateItemsPosition();
        UpdateCountUI();
        return item;
    }

    public int GetMaxCapacity() => config.maxItemsInHand + bonusCapacity;

    public void ActivateCapacityBoost()
    {
        if (capacityBoostCoroutine != null) return;
        capacityBoostCoroutine = StartCoroutine(CapacityBoostRoutine());
    }

    private IEnumerator CapacityBoostRoutine()
    {
        if (HUD_Panel.Instance != null)
            HUD_Panel.Instance.ShowCarryUp(true);

        bonusCapacity = config.capacityBoostAmount;
        yield return new WaitForSeconds(config.capacityBoostDuration);

        if (HUD_Panel.Instance != null)
            HUD_Panel.Instance.ShowCarryUp(false);

        bonusCapacity = 0;
        capacityBoostCoroutine = null;
    }

    public bool CanAccept(Type itemType)
    {
        if (IsFull()) return false;
        if (heldItemType == null) return true;
        return heldItemType == itemType;
    }

    public int GetItemCount() => itemsInHand.Count;
    public bool IsFull() => itemsInHand.Count >= GetMaxCapacity();
    public bool IsEmpty() => itemsInHand.Count == 0;

    public void ClearAll()
    {
        foreach (GameObject item in itemsInHand)
        {
            if (item == null)
                continue;

            if (item.TryGetComponent(out BurgerItem _) && BurgerPool.Instance != null)
                BurgerPool.Instance.Return(item);
            else
                Destroy(item);
        }
        itemsInHand.Clear();
        heldItemType = null;
        UpdateCountUI();
    }

    private Vector3 CalculatePosition(int index) => new Vector3(0, config.stackHeight * index, 0);

    private void UpdateItemsPosition()
    {
        for (int i = 0; i < itemsInHand.Count; i++)
        {
            if (itemsInHand[i] != null)
            {
                itemsInHand[i].transform.localPosition = Vector3.Lerp(
                    itemsInHand[i].transform.localPosition,
                    CalculatePosition(i),
                    Time.deltaTime * 10f);
            }
        }
    }

    private IEnumerator MoveToPosition(GameObject item, Vector3 targetLocalPosition, float duration)
    {
        float elapsed = 0f;
        Vector3 start = item.transform.localPosition;

        while (elapsed < duration)
        {
            item.transform.localPosition = Vector3.Lerp(start, targetLocalPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        item.transform.localPosition = targetLocalPosition;
    }

    private void UpdateCountUI()
    {
        if (itemCountText == null) return;

        itemCountText.text = itemsInHand.Count + "/" + GetMaxCapacity();
        itemCountText.color = IsFull() ? Color.red : Color.white;
        itemCountUI.SetActive(itemsInHand.Count > 0);
    }

    private void DebugStackPositions()
    {
        for (int i = 0; i < config.maxItemsInHand; i++)
        {
            Vector3 worldPos = handPoint.TransformPoint(CalculatePosition(i));
            Debug.DrawLine(worldPos - Vector3.right * 0.1f, worldPos + Vector3.right * 0.1f, Color.yellow);
            Debug.DrawLine(worldPos - Vector3.forward * 0.1f, worldPos + Vector3.forward * 0.1f, Color.yellow);
        }

        for (int i = 0; i < itemsInHand.Count; i++)
        {
            if (itemsInHand[i] != null)
                Debug.DrawLine(itemsInHand[i].transform.position, handPoint.TransformPoint(CalculatePosition(i)), Color.red);
        }
    }
}
