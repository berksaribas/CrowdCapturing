using Simulation;
using UnityEngine;
using UnityEngine.UI;

namespace UI
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
			SimulationManager.Instance.WorldSpeed = value;
		}
	}
}