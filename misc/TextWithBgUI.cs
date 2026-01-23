using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace NipaGameKit
{
    [RequireComponent(typeof(VerticalLayoutGroup))]
    [RequireComponent(typeof(ContentSizeFitter))]
    public class TextWithBgUI : MonoBehaviour
    {
        public RectTransform BackgroundRect => this.rectTransform;

        [SerializeField] private int padding = 5;
        private RectTransform rectTransform;
        private TMP_Text textComp;


        public void SetText(string text)
        {
            this.gameObject.SetActive(text.Length > 0);
            this.textComp.text = text;
        }

        private void Awake()
        {
            this.textComp = this.GetComponentInChildren<TMP_Text>();
            this.rectTransform = this.GetComponent<RectTransform>();
            var verticalLayoutGroup = this.GetComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.childControlHeight = true;
            verticalLayoutGroup.childControlWidth = true;
            verticalLayoutGroup.childForceExpandHeight = false;
            verticalLayoutGroup.childForceExpandWidth = false;
            verticalLayoutGroup.padding = new RectOffset(
                this.padding,
                this.padding,
                this.padding,
                this.padding);
            var contentSizeFitter = this.GetComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }
}
