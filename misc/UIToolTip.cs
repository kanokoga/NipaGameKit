using NipaGameKit;
using UnityEngine;

namespace NipaGameKit
{
    [RequireComponent(typeof(MouseOnUI))]
    public class UIToolTip : MonoBehaviour
    {
        private MouseOnUI mouseOnUI;
        [SerializeField, TextArea] private string tooltipText;

        private void Awake()
        {
            this.mouseOnUI = this.GetComponent<MouseOnUI>();
            this.mouseOnUI.OnMouseAction += (a) =>
            {
                if(this.tooltipText.Length == 0)
                {
                    return;
                }

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
