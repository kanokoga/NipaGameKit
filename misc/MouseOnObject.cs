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
        public bool IsActive { get; set; } = true;
        public event Action<MouseActionType> OnMouseAction = delegate { };


        public void SetActive(bool active)
            => this.IsActive = active;

        private void OnMouseEnter()
        {
            if(this.IsActive == false)
            {
                return;
            }
            this.OnMouseAction(MouseActionType.MouseEnter);
        }

        private void OnMouseExit()
        {
            if(this.IsActive == false)
            {
                return;
            }
            this.OnMouseAction(MouseActionType.MouseExit);
        }

        private void OnMouseUp()
        {
            if(this.IsActive == false)
            {
                return;
            }
            this.OnMouseAction(MouseActionType.MouseClick);
        }

        private void OnDestroy()
        {
            if(this.IsActive == false)
            {
                return;
            }
            this.OnMouseAction = delegate { };
        }
    }
}
