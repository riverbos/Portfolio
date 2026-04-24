using TMPro;
using UnityEngine;
using UnityEngine.UI;

// HUD 패널 클래스.
public class HUD_Panel : MonoBehaviour
{
    public TextMeshProUGUI CoinText;
    public Image SpeedUp;
    public Image CarryUp;

    public static HUD_Panel Instance { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateCoinText(int coinCount)
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
