using UnityEngine;

namespace RKit.ActionSpot
{
    [CreateAssetMenu(fileName = "ActionSpotConfig", menuName = "XPHero/Action Spot Config")]
    public class ActionSpotConfig : ScriptableObject
    {
        [Header("액션 식별")]
        public ActionSpotType actionType;
        public string actionName;
        public GameObject prefab;

        [Header("필요 자원")]
        public ResourceType resourceType;
        public long requiredAmount;

        [Header("충전 속도 (초당 소모량)")]
        [Min(0.1f)] public float chargeRatePerSecond = 10f;

        private void OnValidate()
        {
            if (requiredAmount < 1)
                requiredAmount = 1;

            chargeRatePerSecond = Mathf.Max(0.1f, chargeRatePerSecond);
        }
    }
}
