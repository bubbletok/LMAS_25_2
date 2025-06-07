using UnityEngine;

namespace LMAS.Scripts.Interfaces
{
    /// <summary>
    /// 아이템을 집을 수 있는 인터페이스입니다.
    /// </summary>
    public interface IPickable
    {
        /// <summary>
        /// 아이템을 집는 메소드입니다.
        /// </summary>
        /// <param name="interactor">아이템을 집는 객체</param>
        /// <param name="callback">아이템을 집은 후 호출될 콜백 함수</param>
        void Pick(GameObject interactor, System.Action callback);
    }
}