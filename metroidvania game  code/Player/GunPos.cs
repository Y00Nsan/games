using UnityEngine;

public class FollowMouseFromCenter : MonoBehaviour
{
    public Transform center; // 중심점
    public float speed = 5f; // 이동 속도
    public float rotationSpeed = 200f; // 회전 속도
    public float radius = 1f; // 반경

    void Update()
    {
        // 마우스 위치를 화면 좌표에서 월드 좌표로 변환
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePosition.z = 0; // 2D 게임의 경우 Z축을 0으로 고정

        // 중심점과 마우스 위치 사이의 방향 벡터 계산
        Vector2 direction = (mousePosition - center.position).normalized;

        // 목표 위치 계산 (중심점에서 반경만큼 떨어진 위치)
        Vector3 targetPosition = center.position + (Vector3)direction * radius;

        // 현재 위치에서 목표 위치로 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // 마우스를 향하도록 회전
        Vector3 lookDirection = mousePosition - transform.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
}
