using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
	public class WorldSpeedController : MonoBehaviour
	{
		public Slider SpeedSlider;

		private void Awake()
		{
			SpeedSlider.onValueChanged.AddListener(SliderValueChange);
		}

		private void SliderValueChange(float value)
		{
			SimulationController.Instance.SimulationManager.WorldSpeed = value;
		}
	}
}