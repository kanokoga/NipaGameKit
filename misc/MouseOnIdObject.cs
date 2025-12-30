using System;
using System.Collections.Generic;

namespace NipaGameKit
{
    public class MouseOnIdObject<T>
    {
        public static int MouseOveredId { get; private set; } = -1;
        public static int MouseClickedId { get; private set; } = -1;

        public static event Action OnMouseOverChanged = delegate { };
        public static event Action OnMouseClicked = delegate { };

        private readonly MouseOnObject mouseOnObject;
        private readonly int identityId;
        private bool isDisposed;
        private bool allowMouseOver = true;
        private bool allowClick = true;


        public static void Dispose()
        {
            MouseOveredId = -1;
            MouseClickedId = -1;
            OnMouseOverChanged = delegate { };
            OnMouseClicked = delegate { };
        }

        public MouseOnIdObject(MouseOnObject mouseOnObject, int identityId)
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
                        OnMouseOverChanged.Invoke();
                    }

                    break;
                case MouseActionType.MouseExit:
                    if(MouseOveredId == this.identityId)
                    {
                        MouseOveredId = -1;
                        OnMouseOverChanged.Invoke();
                    }

                    break;
                case MouseActionType.MouseClick:
                    if(this.allowClick == true)
                    {
                        MouseClickedId = this.identityId;
                        OnMouseClicked.Invoke();
                    }

                    break;
            }
        }
    }
}
