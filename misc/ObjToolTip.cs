using NipaGameKit;
using UnityEngine;

namespace NipaGameKit
{
    [RequireComponent(typeof(MouseOnObject))]
    public class ObjToolTip : MonoBehaviour
    {
        public string tooltipText;
        private MouseOnObject mouseOnUI;


        private void Awake()
        {
            this.mouseOnUI = this.GetComponent<MouseOnObject>();
            this.mouseOnUI.OnMouseAction += (a) =>
            {
                if(a == MouseActionType.MouseEnter)
                {
                    Tooltip.Instance.SetText(this.tooltipText);
                    Tooltip.Instance.SetActive(true);
                }
                else if(a == MouseActionType.MouseExit)
                {
                    Tooltip.Instance.SetActive(false);
                }
            };
        }
    }
}
