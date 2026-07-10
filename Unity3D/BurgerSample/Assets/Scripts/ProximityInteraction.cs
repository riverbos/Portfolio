using UnityEngine;

/// <summary>
/// 플레이어 주변의 IInteractable 오브젝트를 감지하고 자동으로 상호작용합니다.
/// IInteractable을 구현한 오브젝트라면 이 클래스를 수정하지 않아도 동작합니다.
/// </summary>
[RequireComponent(typeof(PlayerHand))]
public class ProximityInteraction : MonoBehaviour
{
    [Header("상호작용 설정")]
    [SerializeField] private float interactionDistance = 1.5f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("쿨다운 설정")]
    [SerializeField] private float interactionCooldown = 0.5f;
    [SerializeField, Min(1)] private int detectionCapacity = 32;

    [Header("디버깅")]
    [SerializeField] private bool showDebugSphere = false;

    private PlayerHand playerHand;
    private float lastInteractionTime;
    private Collider[] overlapResults;

    private void Awake()
    {
        playerHand = GetComponent<PlayerHand>();
        overlapResults = new Collider[Mathf.Max(1, detectionCapacity)];
    }

    private void Update()
    {
        if (Time.time - lastInteractionTime < interactionCooldown)
            return;

        IInteractable interactable = FindClosestInteractable();
        if (interactable != null && interactable.CanInteract(playerHand))
        {
            interactable.Interact(playerHand);
            lastInteractionTime = Time.time;
        }

#if UNITY_EDITOR
        if (showDebugSphere)
        {
            Debug.DrawLine(transform.position, transform.position + transform.forward * interactionDistance, Color.blue);
            DebugExtension.DebugWireSphere(transform.position, Color.yellow, interactionDistance);
        }
#endif
    }

    private IInteractable FindClosestInteractable()
    {
        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            interactionDistance,
            overlapResults,
            interactableLayer,
            QueryTriggerInteraction.Collide);

        IInteractable closest = null;
        float closestSqrDistance = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Collider candidateCollider = overlapResults[i];
            IInteractable candidate = candidateCollider.GetComponentInParent<IInteractable>();
            if (candidate == null || !candidate.CanInteract(playerHand))
                continue;

            Vector3 closestPoint = candidateCollider.ClosestPoint(transform.position);
            float sqrDistance = (closestPoint - transform.position).sqrMagnitude;
            if (sqrDistance >= closestSqrDistance)
                continue;

            closest = candidate;
            closestSqrDistance = sqrDistance;
        }

        return closest;
    }

    private void OnValidate()
    {
        interactionDistance = Mathf.Max(0.1f, interactionDistance);
        interactionCooldown = Mathf.Max(0f, interactionCooldown);
        detectionCapacity = Mathf.Max(1, detectionCapacity);
    }

    public static class DebugExtension
    {
        public static void DebugWireSphere(Vector3 position, Color color, float radius = 1.0f)
        {
            DrawCircle(position, Vector3.right, color, radius);
            DrawCircle(position, Vector3.up, color, radius);
            DrawCircle(position, Vector3.forward, color, radius);
        }

        private static void DrawCircle(Vector3 position, Vector3 normal, Color color, float radius = 1.0f)
        {
            int segments = 36;
            Vector3 forward = Vector3.Cross(normal, Vector3.up);
            if (forward.sqrMagnitude < 0.01f)
                forward = Vector3.Cross(normal, Vector3.right);

            forward.Normalize();
            Vector3 right = Vector3.Cross(normal, forward).normalized;

            Vector3 lastPoint = position + right * radius;
            for (int i = 0; i <= segments; i++)
            {
                float angle = (float)i / segments * 360.0f * Mathf.Deg2Rad;
                Vector3 nextPoint = position + (right * Mathf.Cos(angle) + forward * Mathf.Sin(angle)) * radius;
                Debug.DrawLine(lastPoint, nextPoint, color);
                lastPoint = nextPoint;
            }
        }
    }
}
