using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI
{
	public class WorldTimeController : MonoBehaviour
	{
		public Text WorldTime;

		private void Update()
		{
			WorldTime.text =
				"Time: " + TimeHelper.ConvertSecondsToString(SimulationController.Instance.SimulationManager
					.WorldTimeSeconds);
		}
	}
}