#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteInEditMode]
    public class VerticalText : MonoBehaviour
    {
        private RectTransform rectT, parentRectT;

        private void Awake()
        {
            rectT = transform as RectTransform;
            parentRectT = rectT.parent as RectTransform;
        }

        void Update()
        {
            rectT.sizeDelta = new Vector2(parentRectT.sizeDelta.y, parentRectT.sizeDelta.x);
        }
    }
}

#endif