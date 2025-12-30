using UnityEngine;
using System;

namespace NipaGameKit
{
    public enum MouseActionType
    {
        None,
        MouseEnter,
        MouseExit,
        MouseClick
    }

    public class MouseOnObject : MonoBehaviour
    {
        public event Action<MouseActionType> OnMouseAction = delegate { };

        private void OnMouseEnter()
        {
            this.OnMouseAction(MouseActionType.MouseEnter);
        }

        private void OnMouseExit()
        {
            this.OnMouseAction(MouseActionType.MouseExit);
        }

        private void OnMouseUp()
        {
            this.OnMouseAction(MouseActionType.MouseClick);
        }

        private void OnDestroy()
        {
            this.OnMouseAction = delegate { };
        }
    }
}
