using UnityEngine;
using UnityEngine.Events;

namespace RKit.ActionSpot
{
    [RequireComponent(typeof(Collider))]
    public class ActionSpot : MonoBehaviour
    {
        [SerializeField] private ActionSpotConfig config;
        [SerializeField] private UI_ActionSpot ui;

        [Header("이벤트")]
        public UnityEvent OnCompleted;
        public UnityEvent OnCharged;  // 자원 1단위 소모 시 (사운드 등 연동용)

        private float accumulator;
        private long paid;
        private bool playerInside;
        private bool completed;
        private IActionSpotResourceProvider resourceProvider;

        public ActionSpotConfig Config => config;

        private void Start()
        {
            ui.Initialize(config.resourceType, config.requiredAmount);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            resourceProvider = other.GetComponent<IActionSpotResourceProvider>();
            playerInside = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player")) playerInside = false;
        }

        private void Update()
        {
            if (!playerInside || completed || resourceProvider == null) return;

            accumulator += Time.deltaTime;
            if (accumulator >= 1f / config.chargeRatePerSecond)
            {
                accumulator = 0f;
                if (resourceProvider.ConsumeResource(config.resourceType))
                {
                    paid++;
                    OnCharged.Invoke();
                }
            }

            ui.UpdateGauge((float)paid / config.requiredAmount, paid);

            if (paid >= config.requiredAmount)
            {
                completed = true;
                ui.SetCompleted();
                OnCompleted.Invoke();
                Destroy(gameObject, 0.2f);
            }
        }
    }
}
