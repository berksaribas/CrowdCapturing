using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteInEditMode]
    public class VerticalTextButton : MonoBehaviour, ILayoutGroup
    {
        private RectTransform rectT, childRectT;

        private void Awake()
        {
            rectT = transform as RectTransform;
            childRectT = transform.GetChild(0) as RectTransform;
        }

        public void SetLayoutHorizontal()
        {
            var size = childRectT.sizeDelta;
            size.y = rectT.sizeDelta.x;
            childRectT.sizeDelta = size;
        }

        public void SetLayoutVertical()
        {
            var size = childRectT.sizeDelta;
            size.x = rectT.sizeDelta.y;
            childRectT.sizeDelta = size;
        }
    }
}