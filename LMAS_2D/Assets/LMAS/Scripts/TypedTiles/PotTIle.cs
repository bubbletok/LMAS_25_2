using UnityEngine;

namespace LMAS.Scripts.TypedTiles
{
    public class PotTile : PickableTile
    {
        public override void Pick(GameObject interactor, System.Action callback)
        {
            // 여기서 PotTile에 대한 구체적인 동작을 구현합니다.
            // 예를 들어, 아이템을 획득하거나 애니메이션을 재생할 수 있습니다.
            Debug.Log($"{interactor.name} has picked up a pot tile.");

            // 콜백 호출
            callback?.Invoke();
        }
    }
}