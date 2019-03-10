using UnityEngine;
using Util;

namespace Simulation
{
    public class SimulationManager : MonoBehaviour
    {
        public float WorldSpeed = 10f;
        public float WorldTimeSeconds = 0;

        private float lastRecordedTime = 0f;

        private void Update()
        {
            WorldTimeSeconds += (Time.realtimeSinceStartup - lastRecordedTime) * WorldSpeed;
            lastRecordedTime = Time.realtimeSinceStartup;
        }

        public void GenerateCrowds(Sequence sequence)
        {
            var totalToPrevTime = 0f;
            if (sequence.ProcessedTime > 0f)
            {
                totalToPrevTime = EasingFunction.GetEasingFunction(sequence.Ease)
                    .Invoke(0, sequence.CrowdSize, (sequence.ProcessedTime - 1) / sequence.Duration);
            }

            var totalToThisTime = EasingFunction.GetEasingFunction(sequence.Ease)
                .Invoke(0, sequence.CrowdSize, sequence.ProcessedTime / sequence.Duration);

            sequence.ProcessedTime += 1f;
            sequence.OutputCrowd += totalToThisTime - totalToPrevTime;

            if (sequence.OutputCrowd < 1.0)
            {
                return;
            }

            for (var i = 0; i < sequence.OutputCrowd; i++)
            {
                var startingDoor = sequence.StartingBuilding.GetDoorByTargetBuilding(sequence.TargetBuilding);
                var finishingDoor = sequence.StartingBuilding.GetFinishingDoorByTargetBuilding(startingDoor, sequence.TargetBuilding);

                SimulationController.Instance.CrowdManager.CreateAgent(startingDoor, finishingDoor, sequence.ActorMaterialProperty);
            }

            sequence.TotalOutput += (int) sequence.OutputCrowd;
            sequence.OutputCrowd -= (int) sequence.OutputCrowd;
        }
    }
}