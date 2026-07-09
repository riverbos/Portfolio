using RKit.ActionSpot;
using UnityEngine;

public class BuildActionSpotBuilder : MonoBehaviour
{
    [SerializeField] private ActionSpot actionSpot;
    [SerializeField] private GameObject buildPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject existingBuildTarget;
    [SerializeField] private bool hideExistingTargetOnStart = true;
    [SerializeField] private bool buildOnce = true;

    private GameObject builtObject;

    private void Awake()
    {
        if (actionSpot == null)
            actionSpot = GetComponent<ActionSpot>();

        if (hideExistingTargetOnStart && existingBuildTarget != null)
            existingBuildTarget.SetActive(false);
    }

    private void OnEnable()
    {
        if (actionSpot != null)
            actionSpot.OnCompleted.AddListener(Build);
    }

    private void OnDisable()
    {
        if (actionSpot != null)
            actionSpot.OnCompleted.RemoveListener(Build);
    }

    public void Build()
    {
        if (buildOnce && builtObject != null)
            return;

        if (existingBuildTarget != null)
        {
            existingBuildTarget.SetActive(true);
            builtObject = existingBuildTarget;
            return;
        }

        if (buildPrefab == null)
        {
            Debug.LogWarning($"{nameof(BuildActionSpotBuilder)} requires a build prefab or existing build target.", this);
            return;
        }

        Transform target = spawnPoint != null ? spawnPoint : transform;
        builtObject = Instantiate(buildPrefab, target.position, target.rotation);
    }
}
