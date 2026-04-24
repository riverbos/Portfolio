using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target; // 따라다닐 대상 (캐릭터)
    [SerializeField] private Vector3 offset; // 카메라와 대상 사이의 거리

    private void LateUpdate()
    {
        if (target != null)
        {
            // 대상의 위치에 오프셋을 더하여 카메라 위치 설정
            transform.position = target.position + offset;
        }
    }
}
