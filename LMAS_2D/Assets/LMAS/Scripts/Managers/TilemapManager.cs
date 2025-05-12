using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LMAS.Scripts.Manager
{
    public enum TilemapType { None, Ground, Wall, Water };

    [Serializable]
    public class TilemapData
    {
        public Tilemap tilemap; // The tilemap to manage
        public TilemapType type;
    }

    /// <summary>
    /// TilemapManager is a singleton class that manages tilemaps in the game.
    /// </summary>
    public class TilemapManager : MonoSingleton<TilemapManager>
    {
        [SerializeField] private TilemapData[] m_Tilemaps; // Array of tilemaps to manage
        private Dictionary<Vector3Int, TilemapData> m_TilemapMapping = new Dictionary<Vector3Int, TilemapData>(); // Mapping of tilemap positions to their types

        protected override void Awake()
        {
            base.Awake();
            foreach (var tilemapData in m_Tilemaps)
            {
                var tilemap = tilemapData.tilemap;
                var type = tilemapData.type;
                var bounds = tilemap.cellBounds;
                foreach (var pos in bounds.allPositionsWithin)
                {
                    if (tilemap.HasTile(pos))
                    {
                        m_TilemapMapping[pos] = tilemapData;
                    }
                }
            }
        }

        protected override void OnDestroy()
        {
            m_Tilemaps = null;
            m_TilemapMapping.Clear();
            m_TilemapMapping = null;

            base.OnDestroy();
        }

        public List<Collider2D> GetSurrondingColliders(Vector3Int centerPos)
        {
            int dirNum = 9; // Number of directions to check (8 surrounding + center)
            int[] dx = { -1, 0, 1, -1, 0, 1, -1, 0, 1 };
            int[] dy = { -1, -1, -1, 0, 0, 0, 1, 1, 1 };
            List<Collider2D> colliders = new List<Collider2D>();
            for (int i = 0; i < dirNum; i++)
            {
                Vector3Int newPos = new Vector3Int(centerPos.x + dx[i], centerPos.y + dy[i], centerPos.z);
                List<Collider2D> surroundingColliders = GetCollidersOnTile(newPos);
                colliders.AddRange(surroundingColliders);
            }
            return colliders;
        }

        public List<Collider2D> GetCollidersOnTile(Vector3Int centerPos)
        {
            // if (!m_TilemapMapping.ContainsKey(centerPos)) return null;
            // TilemapData tilemapData = m_TilemapMapping[centerPos];

            // var tilemap = tilemapData.tilemap;
            // var tile = tilemap.GetTile(centerPos);
            // if (tile == null) return null;

            Tilemap tilemap = null;
            foreach (var tilemapData in m_Tilemaps)
            {
                if (tilemapData.tilemap.HasTile(centerPos))
                {
                    tilemap = tilemapData.tilemap;
                    break;
                }
            }

            return Physics2D.OverlapBoxAll(tilemap.GetCellCenterWorld(centerPos), tilemap.cellSize, 0f).ToList();
        }

        public Vector3Int GetTilemapPos(Vector2 pos)
        {
            foreach (var tilemapData in m_Tilemaps)
            {
                var tilemap = tilemapData.tilemap;
                try
                {
                    var cellPos = tilemap.WorldToCell(pos);
                    if (tilemap.HasTile(cellPos))
                    {
                        // Check if the tile is not null and return the position
                        var tile = tilemap.GetTile(cellPos);
                        if (tile != null)  // Ensure the tile is not null
                        {
                            return cellPos;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error in GetTilemapPos: {e.Message}");
                }
            }
            return new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
        }

        public Vector3 GetTilemapWorldPos(Vector2 pos)
        {
            foreach (var tilemapData in m_Tilemaps)
            {
                var tilemap = tilemapData.tilemap;
                try
                {
                    var cellPos = tilemap.WorldToCell(pos);
                    if (tilemap.HasTile(cellPos))
                    {
                        // Check if the tile is not null and return the position
                        var tile = tilemap.GetTile(cellPos);
                        if (tile != null)  // Ensure the tile is not null
                        {
                            return tilemap.GetCellCenterWorld(cellPos);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error in GetTilemapWorldPos: {e.Message}");
                }
            }
            return Vector3.negativeInfinity;
        }

        public TileBase GetTile(Vector3Int pos)
        {
            if (m_TilemapMapping.TryGetValue(pos, out TilemapData tilemapData1))
            {
                var tilemap = tilemapData1.tilemap;
                return tilemap.GetTile(pos);
            }

            foreach (var tilemapData2 in m_Tilemaps)
            {
                var tilemap = tilemapData2.tilemap;
                if (tilemap.HasTile(pos))
                {
                    return tilemap.GetTile(pos);
                }
            }

            Debug.LogError($"TileBase at position {pos} not found.");
            return null;
        }

        public List<TileBase> GetTiles(Vector3Int pos)
        {
            List<TileBase> tiles = new List<TileBase>();
            foreach (var tilemapData in m_Tilemaps)
            {
                var tilemap = tilemapData.tilemap;
                if (tilemap.HasTile(pos))
                {
                    var tile = tilemap.GetTile(pos);
                    if (tile != null)  // Ensure the tile is not null
                    {
                        tiles.Add(tile);
                    }
                }
            }
            return tiles;
        }
    }
}