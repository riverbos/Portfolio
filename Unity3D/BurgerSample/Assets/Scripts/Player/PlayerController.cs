using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private PlayerConfig config;
    [SerializeField] private FullscreenJoystick joystick;
    [SerializeField] private Transform cameraTransform;

    private CharacterController characterController;
    private float speedMultiplier = 1f;
    private Coroutine speedBoostCoroutine;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (config == null || joystick == null || cameraTransform == null)
        {
            Debug.LogError($"{nameof(PlayerController)} requires config, joystick, and camera references.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        MoveCharacter();
        RotateCharacter();
    }

    public float GetSpeed() => config.moveSpeed * speedMultiplier;

    // 외부에서 호출하여 스피드 부스트를 활성화하는 메서드
    public void ActivateSpeedBoost()
    {
        if (speedBoostCoroutine != null) return;
        speedBoostCoroutine = StartCoroutine(SpeedBoostRoutine());
    }

    private IEnumerator SpeedBoostRoutine()
    {
        if (HUD_Panel.Instance != null)
            HUD_Panel.Instance.ShowSpeedUp(true);

        speedMultiplier = config.speedBoostMultiplier;
        yield return new WaitForSeconds(config.speedBoostDuration);
        speedMultiplier = 1f;

        if (HUD_Panel.Instance != null)
            HUD_Panel.Instance.ShowSpeedUp(false);

        speedBoostCoroutine = null;
    }

    // 캐릭터 이동 처리
    private void MoveCharacter()
    {
        Vector3 movementDirection = new Vector3(joystick.inputDirection.x, 0, joystick.inputDirection.y);
        movementDirection = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * movementDirection;

        if (!characterController.isGrounded)
            movementDirection.y -= 9.8f * Time.deltaTime;

        if (movementDirection.magnitude > 0.1f)
            characterController.Move(movementDirection * GetSpeed() * Time.deltaTime);
    }

    // 캐릭터 회전 처리
    private void RotateCharacter()
    {
        if (joystick.inputDirection.magnitude <= 0.1f) return;

        Vector3 lookDirection = new Vector3(joystick.inputDirection.x, 0, joystick.inputDirection.y);
        lookDirection = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * lookDirection;

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, config.rotationSpeed * Time.deltaTime);
    }
}
