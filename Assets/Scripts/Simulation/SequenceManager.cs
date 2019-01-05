using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
    public class SequenceManager : MonoBehaviour
    {
        private List<Sequence> sequences;
        private List<Sequence> activeSequences;

        private void Awake()
        {
            sequences = new List<Sequence>();
            activeSequences = new List<Sequence>();

            InvokeRepeating("UpdateSequences", 1.0f, 1.0f);
        }

        public void InsertSequence(Sequence sequence)
        {
            sequences.Add(sequence);
        }

        public void ActivateSequences()
        {
            var removedSequences = new List<Sequence>();

            foreach (var sequence in sequences)
            {
                if (SimulationController.Instance.SimulationManager.WorldTimeSeconds >= sequence.StartTime)
                {
                    removedSequences.Add(sequence);

                    if (SimulationController.Instance.SimulationManager.WorldTimeSeconds < sequence.StartTime + sequence.Duration)
                    {
                        activeSequences.Add(sequence);
                    }
                }
            }

            foreach (var removedSequence in removedSequences)
            {
                sequences.Remove(removedSequence);
            }
        }

        public void ProcessActiveSequences()
        {
            var removedSequences = new List<Sequence>();

            foreach (var sequence in activeSequences)
            {
                if (SimulationController.Instance.SimulationManager.WorldTimeSeconds > sequence.StartTime + sequence.Duration)
                {
                    removedSequences.Add(sequence);
                    continue;
                }

                SimulationController.Instance.SimulationManager.GenerateCrowds(sequence);
            }

            foreach (var removedSequence in removedSequences)
            {
                activeSequences.Remove(removedSequence);
            }
        }

        private void UpdateSequences()
        {
            if (sequences.Count != 0)
            {
                ActivateSequences();
            }

            if (activeSequences.Count != 0)
            {
                ProcessActiveSequences();
            }
        }
    }
}