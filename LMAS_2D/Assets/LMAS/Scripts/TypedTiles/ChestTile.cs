using UnityEngine;

namespace LMAS.Scripts.TypedTiles
{
    /// <summary>
    /// 상호작용 가능한 상자 타일을 나타내는 클래스입니다.
    /// </summary>
    [CreateAssetMenu(fileName = "ChestTile", menuName = "LMAS/TypedTile/ChestTile", order = 1)]
    public class ChestTile : InteractableTile
    {
        /// <summary>
        /// 상자의 이름입니다.
        /// </summary>
        public string ChestName = "Default Chest";

        /// <summary>
        /// 상자의 설명입니다.
        /// </summary>
        public string Description = "A chest.";

        public override void Interact(GameObject interactor, System.Action callback)
        {
            // 콜백이 설정되어 있다면 호출합니다.
            Debug.Log($"{interactor.name} interacted with {ChestName}");
            callback?.Invoke();
        }
    }
}