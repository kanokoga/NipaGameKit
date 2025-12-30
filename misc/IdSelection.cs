using System;
using System.Collections.Generic;

namespace NipaGameKit
{
    public class IdSelection<T>
    {
        public const int InvalidId = -1;
        public static int MouseOveredId { get; private set; } = -1;
        public static int SelectedId { get; private set; } = -1;

        public static event Action OnMouseOverIdChanged = delegate { };
        public static event Action OnSelectedIdChanged = delegate { };

        private readonly MouseOnObject mouseOnObject;
        private readonly int identityId;
        private bool isDisposed;
        private bool allowMouseOver = true;
        private bool allowClick = true;


        public static void Deselect()
        {
            SelectedId = -1;
            OnSelectedIdChanged.Invoke();
        }

        public static void Dispose()
        {
            MouseOveredId = -1;
            SelectedId = -1;
            OnMouseOverIdChanged = delegate { };
            OnSelectedIdChanged = delegate { };
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
