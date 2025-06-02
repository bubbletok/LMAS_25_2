using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LMAS.Scripts.PathFinder
{
    /// <summary>
    /// A* 알고리즘을 이용해 Tilemap 상의 시작 셀(start)에서 목표 셀(target)까지의 경로를 계산합니다.
    /// </summary>
    public class AStarTilePathFinder : MonoBehaviour
    {
        [Header("Tilemap References")]
        [Tooltip("이동 가능한 타일들이 있는 Tilemap")]
        public Tilemap groundTilemap;

        [Tooltip("장애물(이동 불가) 타일들이 있는 Tilemap")]
        public List<Tilemap> obstacleTilemaps;

        /// <summary>
        /// 내부에서 사용하는 A* 노드 클래스
        /// </summary>
        private class Node
        {
            public Vector3Int position;
            public Node parent;
            public int gCost;
            public int hCost;
            public int FCost => gCost + hCost;

            public Node(Vector3Int pos)
            {
                position = pos;
                parent = null;
                gCost = int.MaxValue;
                hCost = 0;
            }
        }

        /// <summary>
        /// 시작 셀(start)에서 목표 셀(target)까지의 경로를 계산하여
        /// 셀 좌표 리스트(List&lt;Vector3Int&gt;)로 반환합니다.
        /// 경로가 없으면 null을 반환합니다.
        /// </summary>
        public List<Vector3Int> FindPath(Vector3Int start, Vector3Int target)
        {
            // 1. 오픈 리스트, 클로즈드 셋, 전체 노드 딕셔너리 초기화
            List<Node> openList = new List<Node>();
            HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();
            Dictionary<Vector3Int, Node> allNodes = new Dictionary<Vector3Int, Node>();

            // 2. 시작 노드 세팅
            Node startNode = new Node(start);
            startNode.gCost = 0;
            startNode.hCost = Heuristic(start, target);
            openList.Add(startNode);
            allNodes[start] = startNode;

            // 3. A* 반복 탐색
            while (openList.Count > 0)
            {
                // 오픈리스트에서 FCost가 가장 작은 노드를 선택
                openList.Sort((a, b) => a.FCost.CompareTo(b.FCost));
                Node current = openList[0];
                openList.RemoveAt(0);
                closedSet.Add(current.position);

                // 목표에 도달한 경우: 역추적해서 경로 반환
                if (current.position == target)
                {
                    return RetracePath(current);
                }

                // 상하좌우 인접 노드 검사
                foreach (Vector3Int offset in GetNeighborsOffsets())
                {
                    Vector3Int neighborPos = current.position + offset;

                    // 이미 닫힌 노드거나, 이동 불가면 건너뜀
                    if (closedSet.Contains(neighborPos) || !IsWalkable(neighborPos))
                        continue;

                    int tentativeG = current.gCost + 1;

                    // 노드를 이미 생성했는지 확인
                    if (!allNodes.TryGetValue(neighborPos, out Node neighborNode))
                    {
                        neighborNode = new Node(neighborPos);
                        allNodes[neighborPos] = neighborNode;
                    }
                    else if (tentativeG >= neighborNode.gCost)
                    {
                        // 이전에 더 나은 경로가 있다면 무시
                        continue;
                    }

                    // G/H 비용 갱신 및 부모 설정
                    neighborNode.gCost = tentativeG;
                    neighborNode.hCost = Heuristic(neighborPos, target);
                    neighborNode.parent = current;

                    // 오픈리스트에 추가되지 않았다면 추가
                    if (!openList.Contains(neighborNode))
                        openList.Add(neighborNode);
                }
            }

            // 경로를 찾지 못했을 때
            return null;
        }

        /// <summary>
        /// 맨해튼 거리 휴리스틱
        /// </summary>
        private int Heuristic(Vector3Int a, Vector3Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        /// <summary>
        /// 상하좌우 인접 셀 오프셋
        /// </summary>
        private static IEnumerable<Vector3Int> GetNeighborsOffsets()
        {
            yield return Vector3Int.right;
            yield return Vector3Int.left;
            yield return Vector3Int.up;
            yield return Vector3Int.down;
        }

        /// <summary>
        /// 해당 셀(cellPos)이 이동 가능한지 체크
        /// - groundTilemap에 타일이 있고,
        /// - obstacleTilemap에는 타일이 없어야 함.
        /// </summary>
        private bool IsWalkable(Vector3Int cellPos)
        {
            if (!groundTilemap.HasTile(cellPos)) return false;
            foreach (var obstacleTilemap in obstacleTilemaps)
            {
                if (obstacleTilemap.HasTile(cellPos)) return false; // 장애물 타일이 있으면 이동 불가
            }
            return true;
        }

        /// <summary>
        /// 목표 노드에 도달한 후, 부모 노드를 거슬러 올라가며 역추적해서 경로 리스트를 생성
        /// </summary>
        private List<Vector3Int> RetracePath(Node endNode)
        {
            List<Vector3Int> path = new List<Vector3Int>();
            Node current = endNode;

            while (current != null)
            {
                path.Add(current.position);
                current = current.parent;
            }
            path.Reverse();
            return path;
        }
    }
}
