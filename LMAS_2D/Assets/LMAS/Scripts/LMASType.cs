using UnityEngine;

namespace LMAS.Scripts
{
    public enum LMASType
    {
        None,
        Agent,
        Ground,
        Wall,
        Door,
        Item
    }

    public class LMASObject : MonoBehaviour
    {
        public LMASType Type = LMASType.None;
    }
}