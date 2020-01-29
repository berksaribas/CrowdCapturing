using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI.MeetingSelector
{
    public class TextsUI : MonoBehaviour
    {
        public AgentMeetingUIConfig Config;
    
        public GameObject TextPrefab;
        private List<TextMeshProUGUI> texts;
    
        private RectTransform rectTransform;
        private Rect rect;
    
        private void Awake()
        {
            texts = new List<TextMeshProUGUI>(Config.MaxCount);

            rectTransform = transform as RectTransform;
            Debug.Assert(rectTransform != null, nameof(rectTransform) + " != null");
            rect = rectTransform.rect;
        }

        public void UpdateUI(List<AgentMeetingUI.Space> spaces)
        {
            texts.ForEach(text => Destroy(text.gameObject));
            texts.Clear();

            foreach (var space in spaces)
            {
                var text = Instantiate(TextPrefab, transform).GetComponent<TextMeshProUGUI>();
                text.text = space.ChildCount.ToString();
                text.rectTransform.anchoredPosition = space.Position * rect.size;
                texts.Add(text);
            }
        }
    }
}