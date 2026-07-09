namespace RKit.ActionSpot
{
    /// <summary>
    /// 플레이어(또는 자원 관리 컴포넌트)에 붙여서 ActionSpot의 자원 소모를 처리합니다.
    /// </summary>
    public interface IActionSpotResourceProvider
    {
        /// <summary>
        /// 자원 1단위를 소모합니다. 성공 시 true 반환.
        /// </summary>
        bool ConsumeResource(ResourceType resourceType);
    }
}
