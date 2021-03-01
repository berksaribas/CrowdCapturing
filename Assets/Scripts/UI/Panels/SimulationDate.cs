using Simulation;
using TMPro;
using UnityEngine;

namespace UI.Panels
{
    public class SimulationDate : MonoBehaviour
    {
        public TextMeshProUGUI Date;

        private string dailyDataDate;

        private void Awake()
        {
            var fileName = SimulationController.Instance.DailyData.name;
            var day = fileName.Substring(0, 2);
            var month = fileName.Substring(2, 2);
            var year = fileName.Substring(4, 4);
            dailyDataDate = $"{day}/{month}/{year}";
        }

        private void OnGUI()
        {
            Date.text = $"Date: <mspace=0.6em>{dailyDataDate}</mspace>";
        }
    }
}