using UnityEngine;

namespace LMAS
{
    /// <summary>
    /// 2D 사이드 스크롤 카메라를 구현하는 컴포넌트입니다.
    /// 플레이어를 따라가며 데드존과 스무딩 기능을 제공합니다.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class SimulationCamera : MonoBehaviour
    {
        [Header("타겟(플레이어) 설정")]
        [Tooltip("카메라가 따라갈 플레이어 Transform")]
        public Transform target;

        [Header("데드존(Dead Zone) 설정")]
        [Tooltip("카메라 중심으로부터 X 축으로 얼마나 허용할지 (단위: 유니티 월드 좌표)")]
        public float deadZoneWidth = 1.0f;
        [Tooltip("카메라 중심으로부터 Y 축으로 얼마나 허용할지 (단위: 유니티 월드 좌표)")]
        public float deadZoneHeight = 0.5f;

        [Header("스무딩(Smoothing) 설정")]
        [Tooltip("카메라가 목표 위치로 이동할 때 걸리는 시간 (작을수록 빠르게 따라감)")]
        public float smoothTime = 0.2f;
        private Vector3 currentVelocity = Vector3.zero;

        [Header("카메라 월드 경계(Bounds) 설정")]
        [Tooltip("카메라가 이동 가능한 최소 X/Y 범위")]
        public Vector2 minBounds = new Vector2(-Mathf.Infinity, -Mathf.Infinity);
        [Tooltip("카메라가 이동 가능한 최대 X/Y 범위")]
        public Vector2 maxBounds = new Vector2(Mathf.Infinity, Mathf.Infinity);

        [Header("오프셋(Offset) 설정")]
        [Tooltip("플레이어 기준 카메라가 위치할 기본 오프셋 (예: y축을 살짝 위로)")]
        public Vector3 offset = new Vector3(0f, 1f, -10f);

        private Camera cam;
        public Camera Cam => cam;

        void Awake()
        {
            cam = GetComponent<Camera>();

            if (target == null)
            {
                Debug.LogError("SideScroll2DCamera: target이 설정되지 않았습니다.");
                enabled = false;
                return;
            }
        }

        void LateUpdate()
        {
            Vector3 desiredPosition = CalculateDesiredPosition();
            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);
            transform.position = ClampToBounds(smoothedPosition);
        }

        /// <summary>
        /// 현재 플레이어 위치와 데드존을 기준으로 카메라가 이동해야 할 목표 위치를 계산
        /// </summary>
        private Vector3 CalculateDesiredPosition()
        {
            // 카메라 기준 플레이어 상대 좌표
            Vector3 cameraCenter = transform.position;
            Vector3 targetPos = target.position + offset;

            float deltaX = targetPos.x - cameraCenter.x;
            float deltaY = targetPos.y - cameraCenter.y;

            Vector3 newCamPos = cameraCenter;

            // X축 데드존 검사
            if (Mathf.Abs(deltaX) > deadZoneWidth)
            {
                float directionX = Mathf.Sign(deltaX);
                newCamPos.x = cameraCenter.x + (Mathf.Abs(deltaX) - deadZoneWidth) * directionX;
            }

            // Y축 데드존 검사
            if (Mathf.Abs(deltaY) > deadZoneHeight)
            {
                float directionY = Mathf.Sign(deltaY);
                newCamPos.y = cameraCenter.y + (Mathf.Abs(deltaY) - deadZoneHeight) * directionY;
            }

            // Z축은 항상 offset.z 유지
            newCamPos.z = targetPos.z;

            return newCamPos;
        }

        /// <summary>
        /// 계산된 카메라 위치를 minBounds/maxBounds 사이로 제한
        /// </summary>
        private Vector3 ClampToBounds(Vector3 pos)
        {
            float vertExtent = cam.orthographicSize;
            float horzExtent = vertExtent * Screen.width / Screen.height;

            // 화면 절반 크기를 고려해서 bounds와 클램핑
            float leftBound = minBounds.x + horzExtent;
            float rightBound = maxBounds.x - horzExtent;
            float bottomBound = minBounds.y + vertExtent;
            float topBound = maxBounds.y - vertExtent;

            pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
            pos.y = Mathf.Clamp(pos.y, bottomBound, topBound);

            return pos;
        }

        // (선택) 에디터 상에서 Bounds 시각화하기
        private void OnDrawGizmosSelected()
        {
            if (cam == null) cam = GetComponent<Camera>();

            // 카메라 뷰포트 너비/높이
            float vert = cam != null ? cam.orthographicSize : 5f;
            float horz = vert * (cam != null ? (float)Screen.width / Screen.height : (float)Screen.width / Screen.height);

            // Bounds 사각형 그리기
            Vector3 bottomLeft = new Vector3(minBounds.x + horz, minBounds.y + vert, 0f);
            Vector3 topLeft = new Vector3(minBounds.x + horz, maxBounds.y - vert, 0f);
            Vector3 topRight = new Vector3(maxBounds.x - horz, maxBounds.y - vert, 0f);
            Vector3 bottomRight = new Vector3(maxBounds.x - horz, minBounds.y + vert, 0f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(bottomLeft, topLeft);
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);

            // Dead Zone 사각형 그리기 (카메라 중심 기준)
            Gizmos.color = Color.yellow;
            Vector3 camCenter = transform.position;
            Vector3 dzBL = new Vector3(camCenter.x - deadZoneWidth, camCenter.y - deadZoneHeight, camCenter.z);
            Vector3 dzTR = new Vector3(camCenter.x + deadZoneWidth, camCenter.y + deadZoneHeight, camCenter.z);
            Vector3 dzTL = new Vector3(camCenter.x - deadZoneWidth, camCenter.y + deadZoneHeight, camCenter.z);
            Vector3 dzBR = new Vector3(camCenter.x + deadZoneWidth, camCenter.y - deadZoneHeight, camCenter.z);

            Gizmos.DrawLine(dzBL, dzTL);
            Gizmos.DrawLine(dzTL, dzTR);
            Gizmos.DrawLine(dzTR, dzBR);
            Gizmos.DrawLine(dzBR, dzBL);
        }
    }
}