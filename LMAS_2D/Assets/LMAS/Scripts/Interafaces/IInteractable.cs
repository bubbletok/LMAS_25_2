
using UnityEngine;

namespace LMAS.Scripts.Interfaces
{
    public interface IInteractable
    {
        /// <summary>
        /// 상호작용을 처리하는 메소드입니다.
        /// </summary>
        /// <param name="callback">상호작용 후 호출될 콜백 함수</param>
        void Interact(GameObject interactor, System.Action callback);
    }
}