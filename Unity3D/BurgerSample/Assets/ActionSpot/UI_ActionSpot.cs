using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RKit.ActionSpot
{
    public class UI_ActionSpot : MonoBehaviour
    {
        [Header("UI 참조")]
        [SerializeField] private Image backgroundFill;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI amountText;

        [Header("아이콘")]
        [SerializeField] private Sprite jewelIcon;
        [SerializeField] private Sprite moneyIcon;

        private long requiredAmount;
        private float targetFill;
        private float currentFill;

        private const float GaugeLerpSpeed = 8f;

        public void Initialize(ResourceType resourceType, long required)
        {
            requiredAmount = required;
            iconImage.sprite = resourceType == ResourceType.Jewel ? jewelIcon : moneyIcon;
            backgroundFill.fillAmount = 0f;
            currentFill = 0f;
            targetFill = 0f;
            amountText.text = $"{requiredAmount}";
        }

        public void UpdateGauge(float progress, long paid)
        {
            targetFill = progress;
            amountText.text = $"{requiredAmount - paid}";
        }

        public void SetCompleted()
        {
            backgroundFill.fillAmount = 1f;
        }

        private void Update()
        {
            currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * GaugeLerpSpeed);
            backgroundFill.fillAmount = currentFill;
        }
    }
}
