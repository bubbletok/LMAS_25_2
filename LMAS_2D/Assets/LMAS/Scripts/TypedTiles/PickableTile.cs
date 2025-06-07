using UnityEngine;

using LMAS.Scripts.Interfaces;

namespace LMAS.Scripts.TypedTiles
{
    public class PickableTile : TypedTile, IPickable
    {
        public virtual void Pick(GameObject interactor, System.Action callback)
        {
            // 기본 구현은 아무 동작도 하지 않음
            // 자식 클래스에서 오버라이드하여 구현할 수 있음
            callback?.Invoke();
        }
    }
}