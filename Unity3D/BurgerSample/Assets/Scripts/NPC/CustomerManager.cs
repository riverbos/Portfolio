using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CustomerAgent customerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform queueFront;
    [SerializeField] private Counter counter;

    [Header("Spawn")]
    [SerializeField] private float minSpawnInterval = 3f;
    [SerializeField] private float maxSpawnInterval = 7f;
    [SerializeField] private int maxQueueSize = 6;

    [Header("Queue")]
    [SerializeField] private float queueSpacing = 1.2f;

    [Header("Table")]
    [SerializeField] private Transform[] tablePositions = new Transform[0];
    [SerializeField] private float minStayDuration = 5f;
    [SerializeField] private float maxStayDuration = 10f;

    private readonly List<CustomerAgent> customers = new List<CustomerAgent>();
    private readonly HashSet<Transform> occupiedSeats = new HashSet<Transform>();
    private float nextSpawnTime;
    private bool spawningStarted;

    public bool HasWaitingCustomer => customers.Count > 0 && customers[0] != null && customers[0].HasArrived;
    public int CustomerCount => customers.Count;

    private void Awake()
    {
        if (counter != null)
            counter.SetCustomerManager(this);
    }

    private void Update()
    {
        bool isCounterActive = counter != null && counter.gameObject.activeInHierarchy;
        if (!isCounterActive)
        {
            spawningStarted = false;
            return;
        }

        if (!spawningStarted)
        {
            spawningStarted = true;
            ScheduleNextSpawn();
            return;
        }

        if (customers.Count >= maxQueueSize || Time.time < nextSpawnTime)
            return;

        SpawnCustomer();
        ScheduleNextSpawn();
    }

    public bool TryServeNextCustomer()
    {
        if (!HasWaitingCustomer)
            return false;

        if (!TryGetAvailableSeat(out Transform seat))
            return false;

        CustomerAgent customer = customers[0];
        customers.RemoveAt(0);

        if (customer != null)
        {
            occupiedSeats.Add(seat);
            float stayDuration = Random.Range(
                Mathf.Max(0f, minStayDuration),
                Mathf.Max(minStayDuration, maxStayDuration));
            customer.MoveToSeat(
                seat.position,
                seat.parent.position,
                stayDuration,
                () => LeaveTable(customer, seat));
        }

        RefreshQueuePositions();
        return true;
    }

    private void SpawnCustomer()
    {
        if (customerPrefab == null || spawnPoint == null || queueFront == null)
            return;

        CustomerAgent customer = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation, transform);
        customers.Add(customer);
        customer.MoveTo(GetQueuePosition(customers.Count - 1));
    }

    private void RefreshQueuePositions()
    {
        for (int i = 0; i < customers.Count; i++)
        {
            if (customers[i] != null)
                customers[i].MoveTo(GetQueuePosition(i));
        }
    }

    private Vector3 GetQueuePosition(int index)
    {
        Vector3 firstQueuePosition = queueFront.position;
        firstQueuePosition.x = 0f;

        Vector3 queueDirection = spawnPoint.position - firstQueuePosition;
        queueDirection.y = 0f;

        if (queueDirection.sqrMagnitude < 0.01f)
            queueDirection = -queueFront.forward;

        return firstQueuePosition + queueDirection.normalized * (queueSpacing * index);
    }

    private void ScheduleNextSpawn()
    {
        float minimum = Mathf.Max(0.1f, minSpawnInterval);
        float maximum = Mathf.Max(minimum, maxSpawnInterval);
        nextSpawnTime = Time.time + Random.Range(minimum, maximum);
    }

    private bool TryGetAvailableSeat(out Transform availableSeat)
    {
        foreach (Transform tablePosition in tablePositions)
        {
            if (tablePosition == null || !tablePosition.gameObject.activeInHierarchy)
                continue;

            for (int i = 0; i < tablePosition.childCount; i++)
            {
                Transform candidate = tablePosition.GetChild(i);
                bool isSeat = candidate.name == "Pos_1" || candidate.name == "Pos_2";
                if (isSeat && candidate.gameObject.activeInHierarchy && !occupiedSeats.Contains(candidate))
                {
                    availableSeat = candidate;
                    return true;
                }
            }
        }

        availableSeat = null;
        return false;
    }

    private void LeaveTable(CustomerAgent customer, Transform seat)
    {
        occupiedSeats.Remove(seat);

        if (customer != null)
            customer.Leave(spawnPoint.position);
    }
}
