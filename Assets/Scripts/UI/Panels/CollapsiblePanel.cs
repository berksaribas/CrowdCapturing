using System;
using JetBrains.Annotations;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace UI.Panels
{
    public class CollapsiblePanel : MonoBehaviour
    {
        [NotNull]
        public Button CollapseButton;
        [NotNull]
        public GameObject ContentToCollapse;

        private void Awake()
        {
            CollapseButton.onClick.AddListener( () =>
            {
                print("Budona bazdın, bıravo");
            });
        }
    }
}