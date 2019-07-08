using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	public class SequenceManager : MonoBehaviour
	{
		private List<Sequence> sequences = new List<Sequence>();

		public void InsertSequence(Sequence sequence)
		{
			sequences.Add(sequence);
			sequences.Sort();
		}

		private void Update()
		{
			ActivateSequences();
		}

		private void ActivateSequences()
		{
			var removedSequences = new List<Sequence>();
			
			foreach (var sequence in sequences)
			{
				if (sequence.disabled)
				{
					removedSequences.Add(sequence);
				}
				else if (SimulationController.Instance.SimulationManager.WorldTimeSeconds - 0.2f * SimulationController.Instance.SimulationManager.WorldSpeed>= sequence.StartTime)
				{
					removedSequences.Add(sequence);
					var agent = SimulationController.Instance.AgentManager.GetAgentById(sequence.AgentId);
					SimulationController.Instance.BuildingManager.UnregisterAgent(
						sequence.StartingBuildingId,
						agent
					);
					agent.EndSequence();
				}
				else if (SimulationController.Instance.SimulationManager.WorldTimeSeconds >= sequence.StartTime)
				{
					removedSequences.Add(sequence);
					SimulationController.Instance.AgentManager.ActivateAgent(sequence.AgentId,
						sequence.ActorMaterialProperty);
				}
				else
				{
					break;
				}
			}

			foreach (var removedSequence in removedSequences)
			{
				sequences.Remove(removedSequence);
			}
		}
	}
}