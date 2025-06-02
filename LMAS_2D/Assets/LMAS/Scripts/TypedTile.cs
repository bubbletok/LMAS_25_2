using UnityEngine;
using UnityEngine.Tilemaps;


namespace LMAS.Scripts
{
    [CreateAssetMenu(fileName = "TypedTile", menuName = "LMAS/TypedTile", order = 0)]
    public class TypedTile : Tile
    {
        public LMASType Type = LMASType.None;
    }
}