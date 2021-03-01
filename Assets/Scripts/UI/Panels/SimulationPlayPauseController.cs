using Simulation;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    public class SimulationPlayPauseController : MonoBehaviour
    {
        public Button PlayButton, PauseButton;

        private void SetButtonStates()
        {
            var simulationPlaying = SimulationController.Instance.TimeManager.Playing;
            PlayButton.interactable = !simulationPlaying;
            PauseButton.interactable = simulationPlaying;
        }
        
        private void PressedPlay()
        {
            SimulationController.Instance.TimeManager.Playing = true;
            SetButtonStates();
        }
        
        private void PressedPause()
        {
            SimulationController.Instance.TimeManager.Playing = false;
            SetButtonStates();
        }
        
        private void Awake()
        {
            PlayButton.onClick.AddListener(PressedPlay);
            PauseButton.onClick.AddListener(PressedPause);
            SetButtonStates();
        }
    }
}