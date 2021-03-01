using Simulation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.Panels
{
    public class SimulationTimeController : MonoBehaviour
    {
        public TextMeshProUGUI CurrentTime, RangeStart, RangeEnd;
        public Slider Slider;

        private void Awake()
        {
            var startTime = TimeHelper.ConvertSecondsToString(
                SimulationController.Instance.TimeManager.DataRangeStart
            );
            RangeStart.text = $"Start: <mspace=0.6em>{startTime}</mspace>";
            
            var endTime = TimeHelper.ConvertSecondsToString(
                SimulationController.Instance.TimeManager.DataRangeEnd
            );
            RangeEnd.text = $"End: <mspace=0.6em>{endTime}</mspace>";
        }

        private void OnGUI()
        {
            var timeManager = SimulationController.Instance.TimeManager;
            
            var currentTime = TimeHelper.ConvertSecondsToString(timeManager.Time);
            CurrentTime.text = $"Current Time: <mspace=0.6em>{currentTime}</mspace>";

            var start = timeManager.DataRangeStart;
            var end = timeManager.DataRangeEnd;
            
            Slider.value = (float) ((timeManager.Time - start) / (end - start));
        }
    }
}