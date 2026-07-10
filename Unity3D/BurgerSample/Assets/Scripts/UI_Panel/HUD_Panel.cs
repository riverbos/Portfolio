using TMPro;
using RKit.ActionSpot;
using UnityEngine;
using UnityEngine.UI;

// HUD 패널 클래스.
public class HUD_Panel : MonoBehaviour
{
    public TextMeshProUGUI CoinText;
    public Image SpeedUp;
    public Image CarryUp;

    public static HUD_Panel Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        if (Instance != this)
            return;

        PlayerData.ResourceChanged += HandleResourceChanged;
        UpdateCoinText(PlayerData.Money);
    }

    private void OnDisable()
    {
        if (Instance != this)
            return;

        PlayerData.ResourceChanged -= HandleResourceChanged;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void HandleResourceChanged(ResourceType resourceType, long amount)
    {
        if (resourceType == ResourceType.Money)
            UpdateCoinText(amount);
    }

    public void UpdateCoinText(long coinCount)
    {
        if (CoinText != null)
            CoinText.text = coinCount.ToString("N0");
    }

    public void ShowSpeedUp(bool show)
    {
        if (SpeedUp != null)
            SpeedUp.gameObject.SetActive(show);
    }

    public void ShowCarryUp(bool show)
    {
        if (CarryUp != null)
            CarryUp.gameObject.SetActive(show);
    }
}
