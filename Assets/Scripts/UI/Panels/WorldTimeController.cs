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
            var timeInSeconds = TimeHelper.ConvertSecondsToString(
                SimulationController.Instance.SimulationTime.TimeInSeconds
            );

            WorldTime.text =
                $"Date: <mspace=0.6em>26/09/2016</mspace>  Time: <mspace=0.6em>{timeInSeconds}</mspace>";
        }
    }
}