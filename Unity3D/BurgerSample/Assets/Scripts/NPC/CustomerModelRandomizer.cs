using UnityEngine;

public class CustomerModelRandomizer : MonoBehaviour
{
    [SerializeField] private Transform modelRoot;
    [SerializeField] private RuntimeAnimatorController animatorController;
    [SerializeField] private bool randomizeOnAwake = true;
    [SerializeField] private float initialSpeed;

    private GameObject currentModel;
    private Animator currentAnimator;
    private int requestVersion;

    public Animator Animator => currentAnimator;
    public GameObject CurrentModel => currentModel;

    private void Awake()
    {
        if (randomizeOnAwake)
            RandomizeModel();
    }

    [ContextMenu("Randomize Model")]
    public void RandomizeModel()
    {
        int currentRequestVersion = ++requestVersion;
        CustomerModelProvider.Instance.RequestRandomModel(
            prefab => OnModelLoaded(prefab, currentRequestVersion));
    }

    private void OnModelLoaded(GameObject prefab, int completedRequestVersion)
    {
        if (this == null || completedRequestVersion != requestVersion || prefab == null)
            return;

        ClearCurrentModel();

        Transform parent = modelRoot != null ? modelRoot : transform;
        currentModel = Instantiate(prefab, parent);
        currentModel.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        currentModel.transform.localRotation = Quaternion.identity;
        currentModel.transform.localScale = Vector3.one;

        currentAnimator = currentModel.GetComponent<Animator>();
        if (currentAnimator == null)
            currentAnimator = currentModel.AddComponent<Animator>();

        if (animatorController != null)
            currentAnimator.runtimeAnimatorController = animatorController;

        SetMoveSpeed(initialSpeed);
    }

    private void OnDestroy()
    {
        requestVersion++;
    }

    private void ClearCurrentModel()
    {
        if (currentModel == null)
            return;

        if (Application.isPlaying)
            Destroy(currentModel);
        else
            DestroyImmediate(currentModel);

        currentModel = null;
        currentAnimator = null;
    }

    public void SetMoveSpeed(float speed)
    {
        initialSpeed = Mathf.Max(0f, speed);

        if (currentAnimator == null)
            return;

        currentAnimator.SetFloat("Speed", initialSpeed);
        currentAnimator.SetBool("IsRunning", initialSpeed > 0.5f);
    }
}
