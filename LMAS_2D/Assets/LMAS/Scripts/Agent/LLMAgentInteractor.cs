using LMAS.Scripts.Interfaces;
using LMAS.Scripts.TypedTiles;
using UnityEngine;

namespace LMAS.Scripts.Agent
{
    public class LLMAgentInteractor : MonoBehaviour
    {
        public void Interact(LLMAgent agent, GameObject target, System.Action callback)
        {
            if (agent == null || target == null)
            {
                Debug.LogWarning("Agent or target is null. Please ensure both are set.");
                return;
            }

            IInteractable interactable = target.GetComponent<IInteractable>();
            if (interactable == null)
            {
                Debug.LogWarning("Target does not have an interactable component.");
                return;
            }

            interactable.Interact(agent.gameObject, callback);
        }

        public void InteractTile(LLMAgent agent, InteractableTile tile, System.Action callback)
        {
            if (agent == null || tile == null)
            {
                Debug.LogWarning("Agent or tile is null. Please ensure both are set.");
                callback?.Invoke();
                return;
            }

            tile.Interact(agent.gameObject, callback);
        }

        public void PickTile(LLMAgent agent, PickableTile tile, System.Action callback)
        {
            if (agent == null || tile == null)
            {
                Debug.LogWarning("Agent or tile is null. Please ensure both are set.");
                callback?.Invoke();
                return;
            }

            tile.Pick(agent.gameObject, callback);
        }
    }
}