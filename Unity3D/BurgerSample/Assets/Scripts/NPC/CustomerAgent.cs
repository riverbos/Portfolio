using System;
using System.Collections;
using UnityEngine;

public class CustomerAgent : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float stoppingDistance = 0.05f;

    private CustomerModelRandomizer modelRandomizer;
    private Vector3 targetPosition;
    private bool isMoving;
    private bool leaveWhenArrived;
    private Action arrivedAction;

    public bool HasArrived => !isMoving;

    private void Awake()
    {
        modelRandomizer = GetComponent<CustomerModelRandomizer>();
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (!isMoving)
            return;

        Vector3 offset = targetPosition - transform.position;
        offset.y = 0f;

        if (offset.sqrMagnitude <= stoppingDistance * stoppingDistance)
        {
            transform.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            isMoving = false;
            modelRandomizer?.SetMoveSpeed(0f);

            if (leaveWhenArrived)
            {
                Destroy(gameObject);
                return;
            }

            Action action = arrivedAction;
            arrivedAction = null;
            action?.Invoke();

            return;
        }

        Vector3 direction = offset.normalized;
        transform.position += direction * Mathf.Min(moveSpeed * Time.deltaTime, offset.magnitude);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.LookRotation(direction),
            720f * Time.deltaTime);
    }

    public void MoveTo(Vector3 position)
    {
        targetPosition = position;
        leaveWhenArrived = false;
        arrivedAction = null;
        isMoving = true;
        modelRandomizer?.SetMoveSpeed(moveSpeed);
    }

    public void MoveToSeat(Vector3 position, Vector3 lookAtPosition, float stayDuration, Action onStayFinished)
    {
        targetPosition = position;
        leaveWhenArrived = false;
        arrivedAction = () =>
        {
            FacePosition(lookAtPosition);
            StartCoroutine(StayAtSeat(stayDuration, onStayFinished));
        };
        isMoving = true;
        modelRandomizer?.SetMoveSpeed(moveSpeed);
    }

    public void Leave(Vector3 exitPosition)
    {
        targetPosition = exitPosition;
        leaveWhenArrived = true;
        arrivedAction = null;
        isMoving = true;

        Collider customerCollider = GetComponent<Collider>();
        if (customerCollider != null)
            customerCollider.enabled = false;

        modelRandomizer?.SetMoveSpeed(moveSpeed);
    }

    private IEnumerator StayAtSeat(float duration, Action onStayFinished)
    {
        yield return new WaitForSeconds(Mathf.Max(0f, duration));
        onStayFinished?.Invoke();
    }

    private void FacePosition(Vector3 position)
    {
        Vector3 direction = position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(direction.normalized);
    }
}
