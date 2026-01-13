using System;
using UnityEngine;
using UnityEngine.EventSystems; // UGUIのイベントシステムに必要

namespace NipaGameKit
{

// UGUIのImage等にアタッチして使用するクラス
    public class MouseOnUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public bool IsActive { get; set; } = true;
        public event Action<MouseActionType> OnMouseAction = delegate { };
        private RectTransform _rectTransform;
        private bool _isMouseOver = false;

        public void SetActive(bool active)
            => this.IsActive = active;

        private void Awake()
        {
            this._rectTransform = this.GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (!this.IsActive)
            {
                return;
            }

            // マウス位置がRect内にあるかチェック
            // 第2引数にInput.mousePosition、第3引数にCamera(Overlayの場合はnull)を渡す
            bool isInside = RectTransformUtility.RectangleContainsScreenPoint(this._rectTransform, Input.mousePosition, null);

            // Enterの判定
            if (isInside && !this._isMouseOver)
            {
                this._isMouseOver = true;
                this.OnMouseAction(MouseActionType.MouseEnter);
            }
            // Exitの判定
            else if (!isInside && this._isMouseOver)
            {
                this._isMouseOver = false;
                this.OnMouseAction(MouseActionType.MouseExit);
            }

            // Clickの判定 (マウスが上で、かつ左クリックが押された瞬間)
            // if (isInside && Input.GetMouseButtonDown(0))
            // {
            //     this.OnMouseAction(MouseActionType.MouseClick);
            // }
        }

        // マウスが入った時 (旧 OnMouseEnter)
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(!this.IsActive)
            {
                return;
            }

        //    this.OnMouseAction(MouseActionType.MouseEnter);
        }

        // マウスが出た時 (旧 OnMouseExit)
        public void OnPointerExit(PointerEventData eventData)
        {
            if(!this.IsActive)
            {
                return;
            }

         //   this.OnMouseAction(MouseActionType.MouseExit);
        }

        // クリック（押して離した）時 (旧 OnMouseUp に近い挙動)
        public void OnPointerClick(PointerEventData eventData)
        {
            if(!this.IsActive)
            {
                return;
            }

            this.OnMouseAction(MouseActionType.MouseClick);
        }

        private void OnDestroy()
        {
            // イベントの購読解除を忘れずに行うためクリア
            this.OnMouseAction = delegate { };
        }
    }
}
