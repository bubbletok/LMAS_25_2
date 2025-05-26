using UnityEngine;

namespace LMAS.Scripts
{
    public enum LMASType
    {
        None,
        Agent,
        Ground,
        Wall,
        Water,
        Door,
        Window,
        Item
    }

    public class LMASObject : MonoBehaviour
    {
        public LMASType Type = LMASType.None;
    }
}