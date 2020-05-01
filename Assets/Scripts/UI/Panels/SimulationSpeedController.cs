using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
	public class SimulationSpeedController : MonoBehaviour
	{
		public Slider SpeedSlider;
		public TextMeshProUGUI SpeedValue;

		private void Awake()
		{
			SpeedSlider.value = SimulationController.Instance.SimulationTime.Speed;
			SpeedValue.text = SpeedSlider.value.ToString();
			
			SpeedSlider.onValueChanged.AddListener(SliderValueChange);
		}

		private void SliderValueChange(float value)
		{
			SimulationController.Instance.SimulationTime.Speed = value;
			SpeedValue.text = value.ToString();
		}
	}
}