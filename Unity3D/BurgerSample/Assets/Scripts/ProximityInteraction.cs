using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 주변의 IInteractable 오브젝트를 감지하고 자동으로 상호작용합니다.
/// IInteractable을 구현한 오브젝트라면 이 클래스를 수정하지 않아도 동작합니다.
/// </summary>
public class ProximityInteraction : MonoBehaviour
{
    [Header("상호작용 설정")]
    [SerializeField] private float interactionDistance = 1.5f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("쿨다운 설정")]
    [SerializeField] private float interactionCooldown = 0.5f;

    [Header("디버깅")]
    [SerializeField] private bool showDebugSphere = false;

    private PlayerHand playerHand;
    private float lastInteractionTime;
    private List<IInteractable> interactablesInRange = new List<IInteractable>();

    private void Start()
    {
        playerHand = GetComponent<PlayerHand>();
        if (playerHand == null)
            playerHand = gameObject.AddComponent<PlayerHand>();
    }

    private void Update()
    {
        CheckInteractablesInRange();

        if (Time.time - lastInteractionTime < interactionCooldown)
            return;

        foreach (IInteractable interactable in interactablesInRange)
        {
            if (interactable.CanInteract(playerHand))
            {
                interactable.Interact(playerHand);
                lastInteractionTime = Time.time;
                break;
            }
        }

#if UNITY_EDITOR
        if (showDebugSphere)
        {
            Debug.DrawLine(transform.position, transform.position + transform.forward * interactionDistance, Color.blue);
            DebugExtension.DebugWireSphere(transform.position, Color.yellow, interactionDistance);
        }
#endif
    }

    private void CheckInteractablesInRange()
    {
        interactablesInRange.Clear();

        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionDistance, interactableLayer);
        foreach (Collider col in colliders)
        {
            IInteractable interactable = col.GetComponent<IInteractable>();
            if (interactable != null)
                interactablesInRange.Add(interactable);
        }
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
            Vector3 forward = Vector3.Slerp(normal, -normal, 0.5f);
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
