using UnityEngine;

namespace LMAS.Scripts.Types
{
    public enum LMASType
    {
        None,
        Agent,
        Ground,
        Wall,
        Furniture,
        Door,
        Props,
        Item,
        Decoration,
        Interactable,
    }

    public class LMASObject : MonoBehaviour
    {
        public LMASType Type = LMASType.None;
    }
}