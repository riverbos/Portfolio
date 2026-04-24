using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FullscreenJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("조이스틱 참조")]
    [SerializeField] private RectTransform joystickBackground;  // 조이스틱 배경
    [SerializeField] private RectTransform joystickHandle;      // 조이스틱 핸들

    [Header("조이스틱 설정")]
    [SerializeField] private float joystickRadius = 50f;        // 조이스틱 반경
    [SerializeField] private bool fixedPosition = true;         // 고정 위치 사용 여부
    [SerializeField] private bool hideWhenReleased = false;     // 놓았을 때 숨길지 여부
    [SerializeField] private bool followTouchPosition = false;  // 터치 위치로 이동 여부

    [Header("화면 위치 설정")]
    [SerializeField][Range(0, 1)] private float horizontalPosition = 0.15f;  // 화면 가로 위치 (0~1)
    [SerializeField][Range(0, 1)] private float verticalPosition = 0.15f;    // 화면 세로 위치 (0~1)
    [SerializeField] private float edgePadding = 20f;           // 화면 가장자리 여백

    [HideInInspector] public Vector2 inputDirection;            // 입력 방향 벡터

    // 내부 변수
    private Vector2 joystickCenter;
    private bool isDragging = false;
    private Canvas parentCanvas;
    private CanvasScaler canvasScaler;
    private RectTransform canvasRectTransform;
    private Vector2 screenSize;
    private RectTransform touchArea; // 전체 화면 터치 영역

    private void Awake()
    {
        // 부모 캔버스 및 관련 컴포넌트 찾기
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            Debug.LogError("조이스틱의 부모로 Canvas가 필요합니다!");
            return;
        }

        canvasScaler = parentCanvas.GetComponent<CanvasScaler>();
        canvasRectTransform = parentCanvas.GetComponent<RectTransform>();

        // 조이스틱이 UI 레이어에 있는지 확인
        if (gameObject.layer != LayerMask.NameToLayer("UI"))
        {
            Debug.LogWarning("조이스틱이 UI 레이어에 있지 않습니다. 터치 입력이 정상 작동하지 않을 수 있습니다.");
        }

        // 터치 영역 확인 또는 생성
        SetupTouchArea();

        // 숨김 설정이 활성화되어 있으면 시작 시 숨김
        if (hideWhenReleased)
        {
            joystickBackground.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        // 화면 크기와 캔버스 스케일 모드에 따라 위치 설정
        SetupJoystickPosition();

        // 화면 크기가 변경될 때 대응 (에디터에서 유용)
#if UNITY_EDITOR
        screenSize = new Vector2(Screen.width, Screen.height);
#endif
    }

    // 전체 화면 터치 영역 설정
    private void SetupTouchArea()
    {
        // 이미 있는 터치 영역 확인
        touchArea = GetComponent<RectTransform>();

        if (touchArea == null)
        {
            Debug.LogError("FullscreenJoystick는 RectTransform이 필요합니다!");
            return;
        }

        // 전체 화면을 커버하도록 설정
        touchArea.anchorMin = Vector2.zero;
        touchArea.anchorMax = Vector2.one;
        touchArea.offsetMin = Vector2.zero;
        touchArea.offsetMax = Vector2.zero;

        // 확인용 로그
        Debug.Log("터치 영역이 전체 화면으로 설정되었습니다.");
    }

    private void Update()
    {
        // 에디터에서 화면 크기가 변경되면 조이스틱 위치 업데이트
#if UNITY_EDITOR
        if (screenSize.x != Screen.width || screenSize.y != Screen.height)
        {
            screenSize = new Vector2(Screen.width, Screen.height);
            SetupJoystickPosition();
        }
#endif
    }

    // 조이스틱 위치 설정
    private void SetupJoystickPosition()
    {
        if (!fixedPosition) return;

        // 캔버스 크기 가져오기
        Vector2 canvasSize = canvasRectTransform.sizeDelta;

        // 위치 계산 (왼쪽 하단 기준)
        float posX = canvasSize.x * horizontalPosition;
        float posY = canvasSize.y * verticalPosition;

        // 가장자리에 너무 가깝지 않도록 조정
        posX = Mathf.Clamp(posX, joystickBackground.sizeDelta.x / 2 + edgePadding,
                           canvasSize.x - joystickBackground.sizeDelta.x / 2 - edgePadding);
        posY = Mathf.Clamp(posY, joystickBackground.sizeDelta.y / 2 + edgePadding,
                           canvasSize.y - joystickBackground.sizeDelta.y / 2 - edgePadding);

        // 앵커 및 피벗 설정 (왼쪽 하단 기준점)
        joystickBackground.anchorMin = new Vector2(0, 0);
        joystickBackground.anchorMax = new Vector2(0, 0);
        joystickBackground.pivot = new Vector2(0.5f, 0.5f);

        // 위치 설정
        joystickBackground.anchoredPosition = new Vector2(posX, posY);

        // 초기 위치 저장
        joystickCenter = joystickBackground.position;

        // 핸들 중앙 위치
        joystickHandle.position = joystickCenter;
    }

    // 터치 시작 (IPointerDownHandler 구현)
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;

        // 숨김 설정이 활성화된 경우 보이게 함
        if (hideWhenReleased && !joystickBackground.gameObject.activeSelf)
        {
            joystickBackground.gameObject.SetActive(true);
        }

        // 터치 위치 따라가기가 활성화되었거나 고정 위치가 아닌 경우
        if (followTouchPosition || !fixedPosition)
        {
            joystickBackground.position = eventData.position;
            joystickCenter = joystickBackground.position;
        }

        // 핸들 위치 업데이트
        OnDrag(eventData);
    }

    // 드래그 중 (IDragHandler 구현)
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        // 조이스틱 중심으로부터의 벡터 계산
        Vector2 direction = eventData.position - (Vector2)joystickCenter;

        // 반경 제한
        if (direction.magnitude > joystickRadius)
        {
            direction = direction.normalized * joystickRadius;
        }

        // 핸들 위치 업데이트
        joystickHandle.position = joystickCenter + direction;

        // 정규화된 입력 방향 계산 (-1 ~ 1)
        inputDirection = direction / joystickRadius;
    }

    // 터치 종료 (IPointerUpHandler 구현)
    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;

        // 핸들 위치 리셋
        joystickHandle.position = joystickCenter;

        // 입력 방향 초기화
        inputDirection = Vector2.zero;

        // 숨김 설정이 활성화된 경우 숨김
        if (hideWhenReleased)
        {
            joystickBackground.gameObject.SetActive(false);
        }

        // 고정 위치가 설정된 경우 원래 위치로 되돌림
        if (fixedPosition)
        {
            SetupJoystickPosition();
        }
    }
}