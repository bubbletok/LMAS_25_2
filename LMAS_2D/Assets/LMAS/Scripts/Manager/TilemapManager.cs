using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

using LMAS.Scripts.TypedTiles;

namespace LMAS.Scripts.Manager
{
    public enum TilemapType { None, Ground, Wall, Props, Decoration, Item, Interactable };

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
        [HideInInspector] public TilemapData[] Tilemaps => m_Tilemaps;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnDestroy()
        {
            m_Tilemaps = null;
            base.OnDestroy();
        }

        public List<Collider2D> GetSurrondingColliders(Vector3 pos)
        {
            Vector3Int centerPos = GetPosOnTilemap(pos);
            int dirNum = 9; // Number of directions to check (8 surrounding + center)
            int[] dx = { -1, 0, 1, -1, 0, 1, -1, 0, 1 };
            int[] dy = { -1, -1, -1, 0, 0, 0, 1, 1, 1 };
            HashSet<Collider2D> colliders = new HashSet<Collider2D>();
            for (int i = 0; i < dirNum; i++)
            {
                Vector3Int newPos = new Vector3Int(centerPos.x + dx[i], centerPos.y + dy[i], centerPos.z);
                List<Collider2D> surroundingColliders = GetCollidersOnTile(newPos);
                if (surroundingColliders != null)
                {
                    foreach (var collider in surroundingColliders)
                    {
                        colliders.Add(collider);
                    }
                }
            }
            return colliders.ToList();
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

        public Vector3Int GetPosOnTilemap(Vector2 pos)
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

        public Vector3 GetWorldPosOnTilemap(Vector2 pos)
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

        public TypedTile GetTile(Vector3Int pos)
        {
            // if (m_TilemapMapping.TryGetValue(pos, out TilemapData tilemapData1))
            // {
            //     var tilemap = tilemapData1.tilemap;
            //     TileBase tileBase = tilemap.GetTile(pos);
            //     TypedTile typedTile = tileBase as TypedTile;
            //     return typedTile;
            // }

            foreach (var tilemapData in m_Tilemaps)
            {
                var tilemap = tilemapData.tilemap;
                if (tilemap.HasTile(pos))
                {
                    return tilemap.GetTile(pos) as TypedTile;
                }
            }

            Debug.LogError($"TileBase at position {pos} not found.");
            return null;
        }

        public List<TypedTile> GetTiles(Vector3Int pos)
        {
            List<TypedTile> tiles = new List<TypedTile>();
            foreach (var tilemapData in m_Tilemaps)
            {
                var tilemap = tilemapData.tilemap;
                if (tilemap.HasTile(pos))
                {
                    var tile = GetTile(pos);
                    if (tile != null)  // Ensure the tile is not null
                    {
                        tiles.Add(tile);
                    }
                }
            }
            return tiles;
        }

        public List<(TypedTile, Vector3)> GetSurroundingTiles(Vector3 pos)
        {
            Vector3Int centerPos = GetPosOnTilemap(pos);
            List<(TypedTile, Vector3)> surroundingTiles = new List<(TypedTile, Vector3)>();
            int dirNum = 8; // Number of directions to check (8 surrounding)
            int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
            int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };
            for (int i = 0; i < dirNum; i++)
            {
                Vector3Int newPos = new Vector3Int(centerPos.x + dx[i], centerPos.y + dy[i], centerPos.z);
                var tiles = GetTiles(newPos);
                int count = tiles.Count;
                for (int j = 0; j < count; j++)
                {
                    if (tiles[j] == null) continue; // Skip if tile is null
                    // Add the tile and its position to the list
                    surroundingTiles.Add((tiles[j], new Vector3(pos.x + dx[i], pos.y + dy[i], pos.z)));
                }
            }
            return surroundingTiles;
        }
    }
}