using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	public class SequenceManager : MonoBehaviour
	{
		private List<Sequence> sequences = new List<Sequence>();

		public void InsertSequences(List<Sequence> sequenceList)
		{
			sequences.AddRange(sequenceList);
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
				else if (SimulationController.Instance.SimulationTime.TimeInSeconds - 0.2f * SimulationController.Instance.SimulationTime.Speed>= sequence.StartTime)
				{
					removedSequences.Add(sequence);
					var agent = SimulationController.Instance.AgentManager.GetAgentById(sequence.AgentId);
					SimulationController.Instance.BuildingManager.UnregisterAgent(
						sequence.StartingBuildingId,
						agent
					);
					agent.EndSequence();
				}
				else if (SimulationController.Instance.SimulationTime.TimeInSeconds >= sequence.StartTime)
				{
					removedSequences.Add(sequence);
					SimulationController.Instance
						.AgentManager.ActivateAgent(
							sequence.AgentId
						);
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