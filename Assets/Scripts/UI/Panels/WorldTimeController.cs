using TMPro;
using UnityEngine;
using Util;

namespace UI.Panels
{
    public class WorldTimeController : MonoBehaviour
    {
        public TextMeshProUGUI WorldTime;

        private void OnGUI()
        {
            WorldTime.text =
                $"Time: {TimeHelper.ConvertSecondsToString(SimulationController.Instance.SimulationTime.TimeInSeconds)}";
        }
    }
}