using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Panels
{
    [RequireComponent(typeof(LayoutElement))]
    [ExecuteAlways]
    public class VerticalTextButton : UIBehaviour, ILayoutGroup, ILayoutSelfController
    {
        public TextMeshProUGUI Text;
        public LayoutElement LayoutElement;

        public void SetLayoutHorizontal()
        {
            LayoutElement.preferredWidth = Text.preferredHeight;
        }

        public void SetLayoutVertical()
        {
            LayoutElement.preferredHeight = Text.preferredWidth;
        }
    }
}