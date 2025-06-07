using System;
using LMAS.Scripts.Interfaces;
using LMAS.Scripts.Types;
using UnityEngine;

namespace LMAS.Scripts.TypedTiles
{
    public class InteractableTile : TypedTile, IInteractable
    {
        public virtual void Interact(GameObject interactor, Action callback)
        {
            callback?.Invoke();
        }
    }
}