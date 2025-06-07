using UnityEngine;
using UnityEngine.Tilemaps;
using LMAS.Scripts.Types;

namespace LMAS.Scripts.TypedTiles
{
    [CreateAssetMenu(fileName = "TypedTile", menuName = "LMAS/TypedTile/DefaultTypedTile", order = 0)]
    public class TypedTile : Tile
    {
        public string TileName = "";
        public LMASType Type = LMASType.None;
    }
}