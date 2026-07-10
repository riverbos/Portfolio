using System.Collections.Generic;
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
        private bool completed;
        private IActionSpotResourceProvider resourceProvider;
        private readonly HashSet<Collider> playerColliders = new HashSet<Collider>();

        public ActionSpotConfig Config => config;

        private void Start()
        {
            if (config == null || ui == null || config.requiredAmount < 1 || config.chargeRatePerSecond <= 0f)
            {
                Debug.LogError($"{nameof(ActionSpot)} requires valid config values and a UI reference.", this);
                enabled = false;
                return;
            }

            ui.Initialize(config.resourceType, config.requiredAmount);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            IActionSpotResourceProvider provider = other.GetComponentInParent<IActionSpotResourceProvider>();
            if (provider == null)
                return;

            playerColliders.Add(other);
            resourceProvider = provider;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            playerColliders.Remove(other);
            if (playerColliders.Count == 0)
            {
                resourceProvider = null;
                accumulator = 0f;
            }
        }

        private void OnDisable()
        {
            playerColliders.Clear();
            resourceProvider = null;
            accumulator = 0f;
        }

        private void Update()
        {
            if (playerColliders.Count == 0 || completed || resourceProvider == null)
                return;

            accumulator += Time.deltaTime;
            if (accumulator >= 1f / config.chargeRatePerSecond)
            {
                accumulator = 0f;
                if (resourceProvider.ConsumeResource(config.resourceType))
                {
                    paid++;
                    OnCharged?.Invoke();
                }
            }

            ui.UpdateGauge((float)paid / config.requiredAmount, paid);

            if (paid >= config.requiredAmount)
            {
                completed = true;
                ui.SetCompleted();
                OnCompleted?.Invoke();
                Destroy(gameObject, 0.2f);
            }
        }
    }
}
