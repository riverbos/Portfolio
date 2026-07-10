using UnityEngine;

// 캐릭터의 애니메이션을 조이스틱 입력과 실제 이동 속도에 따라 제어하는 클래스입니다.
public class CharacterAnimationController : MonoBehaviour
{
    [Header("조이스틱 참조")]
    [SerializeField] private FullscreenJoystick joystick;

    [Header("애니메이션 설정")]
    [SerializeField] private float idleSpeedThreshold = 0.1f;
    [SerializeField] private float walkRunThreshold = 0.5f;
    [SerializeField] private float animationSmoothTime = 0.1f;

    [Header("애니메이션 디버깅")]
    [SerializeField] private bool showDebugInfo = false;

    // 컴포넌트 참조
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController playerController;

    // 애니메이션 파라미터 이름 (Animator에서 사용)
    private readonly string speedParameterName = "Speed";
    private readonly string isRunningParameterName = "IsRunning";
    private readonly string turnParameterName = "TurnDirection";

    // 내부 상태 변수
    private float currentSpeed = 0f;
    private float targetSpeed = 0f;
    private float speedSmoothVelocity = 0f;
    private Vector3 previousPosition;
    private float movementDirection = 0f;

    private void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Animator 컴포넌트가 없습니다! 캐릭터에 Animator를 추가해주세요.");
            enabled = false;
            return;
        }

        if (joystick == null)
        {
            Debug.LogError("VirtualJoystick 참조가 없습니다! Inspector에서 설정해주세요.");
            enabled = false;
            return;
        }

        previousPosition = transform.position;
    }

    private void Update()
    {
        UpdateAnimationParameters();
    }

    private void UpdateAnimationParameters()
    {
        // 실제 이동 속도 계산 (이동 거리 / 시간)
        Vector3 movement = transform.position - previousPosition;
        float actualSpeed = movement.magnitude / Time.deltaTime;
        previousPosition = transform.position;

        if (joystick != null)
        {
            // 타겟 스피드 계산 (조이스틱 입력 기반)
            targetSpeed = joystick.inputDirection.magnitude;

            // 스피드 값을 부드럽게 변경
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, animationSmoothTime);

            // 애니메이터에 스피드 값 전달
            animator.SetFloat(speedParameterName, currentSpeed);

            // 걷기/뛰기 전환을 위한 파라미터 설정
            bool isRunning = currentSpeed > walkRunThreshold;
            animator.SetBool(isRunningParameterName, isRunning);

            // 회전 애니메이션 파라미터 설정 (왼쪽 -1, 가운데 0, 오른쪽 1)
            if (targetSpeed > idleSpeedThreshold)
            {
                // 조이스틱의 좌우 입력값 (x축)에 따라 회전 방향 결정
                movementDirection = Mathf.Clamp(joystick.inputDirection.x, -1f, 1f);
                animator.SetFloat(turnParameterName, movementDirection);
            }
            else
            {
                // 정지 상태에서는 회전 방향을 0으로 리셋
                animator.SetFloat(turnParameterName, 0f);
            }

            // 디버깅 정보 표시
            if (showDebugInfo) {
                Debug.Log($"Speed: {currentSpeed:F2}, IsRunning: {isRunning}, TurnDirection: {movementDirection:F2}");
            }
        }
    }
}
