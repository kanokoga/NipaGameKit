using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace NipaGameKit
{
    public class Tooltip : MonoBehaviour
    {
        public static Tooltip Instance { get; private set; }
        [SerializeField] private float loadDelay = 0.5f;
        [SerializeField] private float loadDuration = 0.5f;
        [SerializeField] private TMP_Text tooltipText;
        [SerializeField] private Vector3 offset;
        [SerializeField] private Vector3 loadingOffset;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private RectTransform windowRectTransform;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform loadingRectTransform;
        [SerializeField] private Image loadingImage;
        private float startTime;
        private float startPosition;
        private bool isTipShown = false;

        public void SetActive(bool active)
        {
            this.isTipShown = false;
            this.gameObject.SetActive(active);
            if(active == true)
            {
                this.loadingImage.gameObject.SetActive(true);
                this.loadingImage.fillAmount = 1f;
                this.startPosition = Input.mousePosition.x;
                this.startTime = Time.time;
                this.canvasGroup.alpha = 0f;
                this.StartCoroutine(this.LazyFitImageToText());
            }
        }

        public void SetText(string text)
        {
            this.tooltipText.text = text;
            this.FitImageToText();
        }

        private void Awake()
        {
            Instance = this;
            this.gameObject.SetActive(false);
        }

        private void Update()
        {
            if(this.isTipShown == false)
            {
                if(Mathf.Abs(this.startPosition - Input.mousePosition.x) > 5f)
                {
                    this.startTime = Time.time;
                    this.startPosition = Input.mousePosition.x;
                    this.canvasGroup.alpha = 0f;
                    this.loadingImage.fillAmount = 1f;
                }

                var elapsed = Time.time - this.startTime;

                if(elapsed > this.loadDelay + this.loadDuration)
                {
                    this.canvasGroup.alpha = 1f;
                    this.loadingImage.gameObject.SetActive(false);
                    this.isTipShown = true;
                }
                else if(elapsed > this.loadDelay)
                {

                    var t = Mathf.Clamp01((elapsed - this.loadDelay) / this.loadDuration);
                    this.loadingImage.fillAmount = t;
                }
            }

            var cursorPos = Input.mousePosition;
            // カーソル位置にオフセットを加えた位置を初期位置とする
            var targetPos = cursorPos + this.offset;

            // 画面サイズを取得
            var screenWidth = Screen.width;
            var screenHeight = Screen.height;

            // Tooltipのサイズを取得
            var tooltipSize = this.windowRectTransform.sizeDelta;

            // RectTransformのpivotを考慮して左上・右下座標を計算
            var pivot = this.rectTransform.pivot;
            var left = targetPos.x - tooltipSize.x * pivot.x;
            var right = targetPos.x + tooltipSize.x * (1 - pivot.x);
            var bottom = targetPos.y - tooltipSize.y * pivot.y;
            var top = targetPos.y + tooltipSize.y * (1 - pivot.y);

            // 画面内に収まるようにオフセットを調整
            if (left < 0)
            {
                targetPos.x += -left;
            }

            if (right > screenWidth)
            {
                targetPos.x -= (right - screenWidth);
            }

            if (bottom < 0)
            {
                targetPos.y += -bottom;
            }

            if (top > screenHeight)
            {
                targetPos.y -= (top - screenHeight);
            }

            this.rectTransform.position = targetPos;
            this.loadingRectTransform.position = cursorPos + this.loadingOffset;
        }

        private IEnumerator LazyFitImageToText()
        {
            yield return null;
            this.FitImageToText();
        }

        private void FitImageToText()
        {
            // Textの推奨サイズを取得
            var preferredWidth = this.tooltipText.preferredWidth;
            var preferredHeight = this.tooltipText.preferredHeight;

            // パディングを追加
            var paddingX = 10f;
            var paddingY = 10f;

            // RectTransformのサイズを設定
            this.windowRectTransform.sizeDelta = new Vector2(
                preferredWidth + paddingX,
                preferredHeight + paddingY
            );
        }
    }
}
