using Simulation;
using TMPro;
using UnityEngine;
using Util;

namespace UI.Panels
{
    public class WorldTimeController : MonoBehaviour
    {
        public TextMeshProUGUI WorldTime;

        private string dailyDataDate;
        
        private void Start()
        {
            var fileName = SimulationController.Instance.DailyData.name;
            dailyDataDate = $"{fileName.Substring(0, 2)}/{fileName.Substring(2, 2)}/{fileName.Substring(4, 4)}";
        }

        private void OnGUI()
        {
            var timeInSeconds = TimeHelper.ConvertSecondsToString(
                SimulationController.Instance.TimeManager.TimeInSeconds
            );

            WorldTime.text = $"Date: <mspace=0.6em>{dailyDataDate}</mspace>  Time: <mspace=0.6em>{timeInSeconds}</mspace>";
        }
    }
}