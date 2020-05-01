using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    public class Panel : MonoBehaviour
    {
        public Button Button;
        public GameObject Content;

        private void Awake()
        {
            Button.onClick.AddListener(Toggle);
        }

        public void Toggle()
        {
            Content.SetActive(!Content.activeSelf);
        }

        public void Open()
        {
            Content.SetActive(true);
        }

        public void Close()
        {
            Content.SetActive(false);
        }

        public void SetState(bool isOpen)
        {
            Content.SetActive(isOpen);
        }
    }
}