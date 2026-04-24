# Portfolio


# Unity3D 포트폴리오

# BurgerSample

*Burger Please!* 스타일의 캐주얼 모바일 게임 프로토타입입니다.  
스토브에서 버거를 집어 카운터에 전달하면 자동으로 판매되는 게임 루프를 구현했습니다.

![게임스샷](Screenshots/burger_screenshot.jpg)

---

## 게임 플레이

- **스토브**에 접근하면 자동으로 버거를 집습니다
- **카운터**에 접근하면 자동으로 버거를 내려놓습니다
- **[S 키]** 일정 시간 동안 이동 속도 증가
- **[H 키]** 일정 시간 동안 손에 들 수 있는 아이템 수 증가

---

## 설계 구조

### IInteractable
스토브, 카운터 등 상호작용 가능한 오브젝트의 공통 인터페이스입니다.  
`ProximityInteraction`은 `Physics.OverlapSphere`로 범위 내 오브젝트를 감지하고  
구체적인 타입을 알지 못한 채 `CanInteract` / `Interact`만 호출합니다.  
새로운 상호작용 오브젝트는 이 인터페이스만 구현하면 됩니다.

```csharp
public interface IInteractable
{
    bool CanInteract(PlayerHand hand);
    void Interact(PlayerHand hand);
}
```

### IHoldable
플레이어가 손에 들 수 있는 오브젝트의 마커 인터페이스입니다.  
`PlayerHand`는 `CanAccept(Type)`으로 타입 일관성을 강제합니다.  
버거를 들고 있을 때 쓰레기는 집을 수 없습니다.

```csharp
public interface IHoldable { }
```

### Object Pool
`BurgerPool`이 버거 인스턴스를 관리합니다.  
런타임 `Instantiate` / `Destroy` 대신 `Get()` / `Return()`으로 재사용하여 GC 부하를 줄입니다.

### ScriptableObject Config
게임 밸런스 수치를 `StoveConfig`, `CounterConfig`, `PlayerConfig`로 분리했습니다.  
여러 스토브 또는 카운터가 하나의 Config 에셋을 공유할 수 있습니다.

---

## 프로젝트 구조

```
Assets/Scripts/
├── IInteractable.cs
├── ProximityInteraction.cs
├── BurgerStove.cs
├── Counter.cs
├── FollowCamera.cs
├── FullscreenJoystick.cs
├── GameConfig/
│   ├── StoveConfig.cs
│   ├── CounterConfig.cs
│   └── PlayerConfig.cs
├── HoldItem/
│   ├── IHoldable.cs
│   ├── BurgerItem.cs
│   └── BurgerPool.cs
├── Player/
│   ├── PlayerController.cs
│   ├── PlayerHand.cs
│   ├── CharacterAnimationController.cs
│   └── PowerUpController.cs
└── UI_Panel/
    └── HUD_Panel.cs
```

---

## 개발 환경

- **Unity** 6000.3.8f1
- **Input System** 패키지
- **TextMeshPro**



  1. 이미지 파일을 프로젝트에 추가
  Burger/
  ├── Screenshots/
  │   ├── gameplay.png
  │   └── ui.png
  ├── README.md
  ...

  2. README에 이미지 삽입
  ## 스크린샷

  