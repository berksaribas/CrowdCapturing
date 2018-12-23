using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	public class SequenceManager : MonoBehaviour
	{
		private List<Sequence> Sequences;
		private List<Sequence> ActiveSequences;

		private void Awake()
		{
			Sequences = new List<Sequence>();
			ActiveSequences = new List<Sequence>();
			
			InvokeRepeating("UpdateSequences", 1.0f, 1.0f);
		}

		public void InsertSequence(Sequence sequence)
		{
			Sequences.Add(sequence);
		}

		public void ActivateSequences()
		{
			var removedSequences = new List<Sequence>();

			for (var i = 0; i < Sequences.Count; i++)
			{
				var sequence = Sequences[i];

				if (SimulationController.Instance.SimulationManager.WorldTimeSeconds >= sequence.StartTime)
				{
					removedSequences.Add(sequence);
					
					if (SimulationController.Instance.SimulationManager.WorldTimeSeconds < sequence.StartTime + sequence.Duration)
					{
						ActiveSequences.Add(sequence);
					}
				}
			}

			foreach (var removedSequence in removedSequences)
			{
				Sequences.Remove(removedSequence);
			}
		}

		public void ProcessActiveSequences()
		{
			var removedSequences = new List<Sequence>();

			for (var i = 0; i < ActiveSequences.Count; i++)
			{
				var sequence = ActiveSequences[i];
				if (SimulationController.Instance.SimulationManager.WorldTimeSeconds > sequence.StartTime + sequence.Duration)
				{
					removedSequences.Add(sequence);
					continue;
				}

				SimulationController.Instance.SimulationManager.GenerateCrowds(sequence);
			}
			
			foreach (var removedSequence in removedSequences)
			{
				ActiveSequences.Remove(removedSequence);
			}
		}

		private void UpdateSequences()
		{
			if (Sequences.Count != 0)
			{
				ActivateSequences();
			}
			
			if (ActiveSequences.Count != 0)
			{
				ProcessActiveSequences();
			}
		}
	}
}