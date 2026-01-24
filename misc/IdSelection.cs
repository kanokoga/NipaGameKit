using System;
using System.Collections.Generic;

namespace NipaGameKit
{
    public static class IdSelectionParam
    {
        public const int InvalidId = -1;
    }


    public class IdSelection<T>
    {
        public static int MouseOveredId { get; private set; } = -1;
        public static int SelectedId { get; private set; } = -1;
        private static int InvalidId => IdSelectionParam.InvalidId;

        public static event Action OnMouseOverIdChanged = delegate { };
        public static event Action OnSelectedIdChanged = delegate { };

        private static bool IsInteractable = true;
        private readonly MouseOnObject mouseOnObject;
        private readonly int identityId;
        private bool isDisposed;
        private bool allowMouseOver = true;
        private bool allowClick = true;


        public static void Deselect()
        {
            SelectedId = InvalidId;
            OnSelectedIdChanged.Invoke();
        }

        public static void Dispose()
        {
            MouseOveredId = InvalidId;
            SelectedId = InvalidId;
            OnMouseOverIdChanged = delegate { };
            OnSelectedIdChanged = delegate { };
        }

        public static bool HasSelection()
            => SelectedId != InvalidId;

        public static bool HasMouseOvered()
            => MouseOveredId != InvalidId;

        public static void SetInteractable(bool interactable)
        {
            IsInteractable = interactable;
            if(IsInteractable == false)
            {
                MouseOveredId = InvalidId;
                OnMouseOverIdChanged.Invoke();
            }
        }

        public IdSelection(MouseOnObject mouseOnObject, int identityId)
        {
            this.mouseOnObject = mouseOnObject;
            this.identityId = identityId;
            this.mouseOnObject.OnMouseAction += this.OnMouseActionHandler;
        }

        public void SetAllowMouseOver(bool allow)
        {
            this.allowMouseOver = allow;
        }

        public void SetAllowClick(bool allow)
        {
            this.allowClick = allow;
        }

        private void OnMouseActionHandler(MouseActionType actionType)
        {
            if(IsInteractable == false)
            {
                return;
            }

            switch(actionType)
            {
                case MouseActionType.MouseEnter:
                    if(this.allowMouseOver == true)
                    {
                        MouseOveredId = this.identityId;
                        OnMouseOverIdChanged.Invoke();
                    }

                    break;
                case MouseActionType.MouseExit:
                    if(MouseOveredId == this.identityId)
                    {
                        MouseOveredId = -1;
                        OnMouseOverIdChanged.Invoke();
                    }

                    break;
                case MouseActionType.MouseClick:
                    if(this.allowClick == true)
                    {
                        SelectedId = this.identityId;
                        OnSelectedIdChanged.Invoke();
                    }

                    break;
            }
        }
    }
}
