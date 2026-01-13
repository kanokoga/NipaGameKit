using NipaGameKit;
using UnityEngine;

namespace NipaGameKit
{
    [RequireComponent(typeof(MouseOnUI))]
    public class UIToolTip : MonoBehaviour
    {
        private MouseOnUI mouseOnUI;
        [SerializeField] private string tooltipText;

        private void Awake()
        {
            this.mouseOnUI = this.GetComponent<MouseOnUI>();
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
