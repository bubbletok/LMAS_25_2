namespace LMAS.Scripts.PathFinder
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using LMAS.Scripts.Manager;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    [RequireComponent(typeof(AStarTilePathFinder))]
    public class TilePathFollow : MonoBehaviour
    {
        [Header("이동 설정")]
        [Tooltip("각 타일 사이를 이동할 때 소요되는 시간 (초)")]
        public float moveDuration = 0.2f;

        private bool isMoving = false;
        private AStarTilePathFinder pathfinder;
        private Tilemap groundTilemap;

        private void Awake()
        {
            pathfinder = GetComponent<AStarTilePathFinder>();
            groundTilemap = pathfinder.groundTilemap;
        }

        /// <summary>
        /// 명령 문자열에서 x, y 값을 실수로 파싱하고, 경로를 찾아 이동을 시작합니다.
        /// 이제 "Move x: -0.5 y: -0.5", "Move x: -0.50, y: -0.50", "Move x=-0.5 y=-0.5", "Move x=0.50, y=0.50" 형태 모두 정상 파싱됩니다.
        /// </summary>
        /// <param name="command">예: "Move x: -0.5 y: -0.5" 또는 "Move x=0.50, y=0.50"</param>
        public void ParseAndMove(string command, Action callback = null)
        {
            // 정규식:
            //  1) 'x' 이후 ':' 또는 '=' 중 하나, 그 뒤 공백 0개 이상, 부호 선택(-?), 숫자+소수부 선택
            //  2) x 뒤와 y 앞 사이에 (콤마+공백) 또는 (공백 1개 이상) 허용
            //  3) 'y' 이후 ':' 또는 '=' 중 하나, 그 뒤 공백 0개 이상, 부호 선택, 숫자+소수부 선택
            Regex regex = new Regex(@"
            x\s*[:=]\s*             # 'x' 뒤에 ':' 또는 '=' , 그 뒤 공백 0개 이상
            (-?\d+(?:\.\d+)?)       # 그룹1: 정수 또는 소수 (예: -0.5, 10, 3.1415 등)
            \s*                     # 숫자 뒤 공백 0개 이상
            (?:,\s*|\s+)            # (콤마 뒤 공백0개 이상) 또는 (공백1개 이상)
            y\s*[:=]\s*             # 'y' 뒤에 ':' 또는 '=' , 그 뒤 공백 0개 이상
            (-?\d+(?:\.\d+)?)       # 그룹2: 정수 또는 소수
        ", RegexOptions.IgnorePatternWhitespace);

            Match match = regex.Match(command);
            if (!match.Success)
            {
                Debug.LogWarning("Invalid move command format: " + command);
                callback?.Invoke();
                return;
            }

            // CultureInfo.InvariantCulture 사용: 소수점을 '.'로 파싱
            float deltaX = float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            float deltaY = float.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);

            // 현재 월드좌표에서 시작 셀 계산
            Vector3 worldStart = transform.position;
            Vector3Int startCell = TilemapManager.Instance.GetPosOnTilemap(worldStart);

            // 입력된 float 오프셋을 world좌표에 더한 뒤, 셀 좌표로 변환
            Vector3 worldTarget = new Vector3(deltaX, deltaY, 0f);
            Vector3Int targetCell = TilemapManager.Instance.GetPosOnTilemap(worldTarget);

            if (targetCell == new Vector3Int(int.MinValue, int.MinValue, int.MinValue))
            {
                Debug.LogWarning("Invalid target cell position: " + worldTarget);
                callback?.Invoke();
                return;
            }

            Debug.Log($"시작 좌표: {worldStart}, 목표 좌표: {worldTarget}");
            Debug.Log($"시작 셀: {startCell}, 목표 셀: {targetCell}");

            // AStarTilePathFinder를 이용해 경로 계산
            List<Vector3Int> path = pathfinder.FindPath(startCell, targetCell);
            if (path == null || path.Count == 0)
            {
                Debug.LogWarning($"경로를 찾을 수 없습니다: start={startCell}, target={targetCell}");
                callback?.Invoke();
                return;
            }

            // 이동 중이 아닐 때만 코루틴으로 실제 이동 시작
            if (!isMoving)
                StartCoroutine(FollowPath(path, callback));
        }


        private IEnumerator FollowPath(List<Vector3Int> path, Action callback = null)
        {
            isMoving = true;

            foreach (Vector3Int cell in path)
            {
                Vector3 worldTarget = groundTilemap.GetCellCenterWorld(cell);
                Vector3 startPos = transform.position;
                float elapsed = 0f;

                while (elapsed < moveDuration)
                {
                    transform.position = Vector3.Lerp(startPos, worldTarget, elapsed / moveDuration);
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                transform.position = worldTarget;
            }

            isMoving = false;

            callback?.Invoke();
        }
    }

}