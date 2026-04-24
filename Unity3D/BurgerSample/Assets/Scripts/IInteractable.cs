/// <summary>
/// 플레이어가 근접 시 상호작용할 수 있는 오브젝트의 공통 인터페이스.
/// 새로운 상호작용 오브젝트(스토브, 카운터, 쓰레기통 등)는 이 인터페이스를 구현하면
/// ProximityInteraction이 별도 수정 없이 자동으로 인식합니다.
/// </summary>
public interface IInteractable
{
    /// <summary>현재 상호작용 가능한 상태인지 반환. Interact 호출 전 반드시 확인합니다.</summary>
    bool CanInteract(PlayerHand hand);
    void Interact(PlayerHand hand);
}
