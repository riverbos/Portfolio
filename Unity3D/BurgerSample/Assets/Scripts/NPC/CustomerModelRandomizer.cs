using UnityEngine;

public class CustomerModelRandomizer : MonoBehaviour
{
    [SerializeField] private GameObject[] modelPrefabs = new GameObject[0];
    [SerializeField] private Transform modelRoot;
    [SerializeField] private RuntimeAnimatorController animatorController;
    [SerializeField] private bool randomizeOnAwake = true;
    [SerializeField] private int selectedModelIndex = -1;
    [SerializeField] private float initialSpeed;

    private GameObject currentModel;
    private Animator currentAnimator;

    public Animator Animator => currentAnimator;
    public GameObject CurrentModel => currentModel;

    private void Awake()
    {
        if (randomizeOnAwake)
            RandomizeModel();
        else
            SetModel(selectedModelIndex);
    }

    [ContextMenu("Randomize Model")]
    public void RandomizeModel()
    {
        if (modelPrefabs == null || modelPrefabs.Length == 0)
            return;

        SetModel(Random.Range(0, modelPrefabs.Length));
    }

    public void SetModel(int modelIndex)
    {
        if (modelPrefabs == null || modelPrefabs.Length == 0)
            return;

        selectedModelIndex = Mathf.Clamp(modelIndex, 0, modelPrefabs.Length - 1);
        GameObject prefab = modelPrefabs[selectedModelIndex];
        if (prefab == null)
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
