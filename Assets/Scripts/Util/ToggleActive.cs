using UnityEngine;

namespace Util
{
    public class ToggleActive : MonoBehaviour
    {
        public void Toggle()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}