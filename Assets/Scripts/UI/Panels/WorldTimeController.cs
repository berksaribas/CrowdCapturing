using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.Panels
{
    public class WorldTimeController : MonoBehaviour
    {
        public Text WorldTime;

        private void Update()
        {
            WorldTime.text = "Time: " + TimeHelper.ConvertSecondsToString(
                                 SimulationController.Instance.SimulationTime.TimeInSeconds);
        }
    }
}