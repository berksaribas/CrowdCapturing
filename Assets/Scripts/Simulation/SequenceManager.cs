using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Simulation
{
	public class SequenceManager : MonoBehaviour
	{
		public SimulationController Simulation;
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

				if (Simulation.WorldTimeSeconds >= sequence.StartTime)
				{
					removedSequences.Add(sequence);
				}

				ActiveSequences.Add(sequence);
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
				var sequence = Sequences[i];
				if (Simulation.WorldTimeSeconds > sequence.StartTime + sequence.Duration &&
				    sequence.RemainingCrowdSize == 0)
				{
					removedSequences.Add(sequence);
					continue;
				}

				Simulation.GenerateCrowds(sequence);
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