/// <summary>
/// 플레이어가 손에 들 수 있는 오브젝트의 마커 인터페이스.
/// PlayerHand는 동일한 IHoldable 구현 타입만 함께 들 수 있습니다.
/// 예: BurgerItem을 들고 있을 때 TrashItem은 집을 수 없습니다.
/// </summary>
public interface IHoldable { }
