using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 햄버거 스토브 클래스.
public class BurgerStove : MonoBehaviour, IInteractable
{
    [Header("설정")]
    [SerializeField] private StoveConfig config;
    [SerializeField] private Transform spawnPoint;

    [Header("시각적 피드백")]
    [SerializeField] private GameObject cookingEffectPrefab;
    [SerializeField] private Transform progressBarTransform;
    [SerializeField] private GameObject availableIndicator;

    private float currentCookingTime = 0f;
    private bool isCooking = true;
    private List<GameObject> burgersOnStove = new List<GameObject>();
    private GameObject currentCookingEffect;

    private void Start()
    {
        if (config == null)
        {
            Debug.LogError($"{nameof(BurgerStove)} requires a stove config.", this);
            enabled = false;
            return;
        }

        if (BurgerPool.Instance == null)
        {
            Debug.LogError("BurgerPool이 씬에 없습니다!");
            enabled = false;
            return;
        }

        if (spawnPoint == null)
            spawnPoint = transform;

        // 초기 상태에서는 요리 효과를 켜고, 햄버거가 있는지 표시합니다.
        ShowCookingEffect(true);
        UpdateAvailableIndicator();
    }

    private void Update()
    {
        // 스토브 위에 최대 햄버거 수가 초과되면 요리를 멈춥니다.
        if (burgersOnStove.Count >= config.maxBurgersOnStove)
        {
            if (isCooking)
            {
                isCooking = false;
                ShowCookingEffect(false);
            }
            return;
        }
        else if (!isCooking)
        {
            isCooking = true;
            ShowCookingEffect(true);
        }

        // 요리 중이라면 시간을 누적하고, 진행 상황을 업데이트합니다.
        if (isCooking)
        {
            currentCookingTime += Time.deltaTime;
            UpdateProgressBar();

            if (currentCookingTime >= config.cookingTime)
            {
                SpawnBurger();
                currentCookingTime = 0f;
                UpdateAvailableIndicator();
            }
        }
    }

    // IInteractable 구현 : 플레이어가 상호작용할 때 햄버거를 집을 수 있는지 체크합니다.
    public bool CanInteract(PlayerHand hand) => HasBurgerAvailable() && hand.CanAccept(typeof(BurgerItem));

    // IInteractable 구현 : 플레이어가 상호작용할 때 햄버거를 집도록 합니다.
    public void Interact(PlayerHand hand)
    {
        GameObject burger = TakeBurger();
        if (burger != null)
            hand.AddItem(burger);
    }

    // 스토브 위에 햄버거가 있는지 여부를 반환하는 메서드입니다.
    public bool HasBurgerAvailable() => burgersOnStove.Count > 0;

    // 햄버거를 하나 집어가는 메서드입니다.
    // 가장 아래에 있는 햄버거를 반환하고 리스트에서 제거합니다.
    public GameObject TakeBurger()
    {
        if (burgersOnStove.Count == 0)
            return null;

        GameObject burger = burgersOnStove[0];
        burgersOnStove.RemoveAt(0);
        burger.transform.SetParent(null);

        for (int i = 0; i < burgersOnStove.Count; i++)
        {
            Vector3 newPosition = spawnPoint.position + Vector3.up * config.burgerHeight * i;
            StartCoroutine(MoveBurger(burgersOnStove[i], newPosition, 0.2f));
        }

        return burger;
    }

    // 새로운 햄버거를 생성하여 스토브 위에 올려놓는 메서드입니다.
    private void SpawnBurger()
    {
        Vector3 spawnPosition = spawnPoint.position + Vector3.up * (burgersOnStove.Count * config.burgerHeight);
        GameObject newBurger = BurgerPool.Instance.Get();
        if (newBurger == null)
            return;

        newBurger.transform.position = spawnPosition;
        newBurger.transform.SetParent(transform);
        burgersOnStove.Add(newBurger);
    }

    // 햄버거가 스토브 위에서 부드럽게 이동하도록 하는 코루틴입니다. (PlayerHand로 이동합니다.)
    private IEnumerator MoveBurger(GameObject burger, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = burger.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            burger.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        burger.transform.position = targetPosition;
    }

    // 요리 효과를 켜거나 끄는 메서드입니다.
    private void ShowCookingEffect(bool show)
    {
        if (cookingEffectPrefab == null) return;

        if (show && currentCookingEffect == null)
        {
            currentCookingEffect = Instantiate(cookingEffectPrefab, spawnPoint.position, Quaternion.identity);
            currentCookingEffect.transform.SetParent(transform);
        }
        else if (!show && currentCookingEffect != null)
        {
            Destroy(currentCookingEffect);
            currentCookingEffect = null;
        }
    }

    // 요리 진행 상황을 시각적으로 표시하는 메서드입니다.
    private void UpdateProgressBar()
    {
        if (progressBarTransform != null)
        {
            float progress = currentCookingTime / config.cookingTime;
            progressBarTransform.localScale = new Vector3(progress, 1f, 1f);
        }
    }

    // 햄버거가 있는지 여부를 시각적으로 표시하는 메서드입니다.
    private void UpdateAvailableIndicator()
    {
        if (availableIndicator != null)
            availableIndicator.SetActive(burgersOnStove.Count > 0);
    }
}
