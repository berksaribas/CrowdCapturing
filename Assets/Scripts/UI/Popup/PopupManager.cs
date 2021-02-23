using UnityEngine;

namespace UI.Popup
{
    public class PopupManager : MonoBehaviour
    {
        public GameObject PopupPrefab;

        private Popup popup;

        void SetPopup(Popup.Data popupData)
        {
            if (popupData == null)
            {
                Destroy(popup.gameObject);
                popup = null;
            }
            else
            {
                if (popup == null)
                    popup = Instantiate(PopupPrefab, transform).GetComponent<Popup>();
                
                popup.SetData(popupData);
            }
        }

        private void Awake()
        {
            UIState.Popup.OnChange += SetPopup;
        }
    }
}