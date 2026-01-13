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
                this.loadingImage.gameObject.SetActive(false);
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
                    this.loadingImage.gameObject.SetActive(true);
                    var t = Mathf.Clamp01((elapsed - this.loadDelay) / this.loadDuration);
                    this.loadingImage.fillAmount = t;
                }
            }

            var cursorPos = Input.mousePosition;
            this.rectTransform.position = cursorPos + this.offset;
            this.loadingRectTransform.position = cursorPos;
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
