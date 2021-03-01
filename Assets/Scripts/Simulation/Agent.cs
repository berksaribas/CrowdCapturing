using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Simulation
{
    public enum AgentState
    {
        Idling,
        Walking,
    }

    [RequireComponent(
        typeof(PathTraverser),
        typeof(MeshRenderer),
        typeof(Collider)
    )]
    public class Agent : MonoBehaviour
    {
        public static readonly Dictionary<AgentState, HashSet<Agent>> AgentsInStates =
            new Dictionary<AgentState, HashSet<Agent>>
            {
                {AgentState.Idling, new HashSet<Agent>()},
                {AgentState.Walking, new HashSet<Agent>()},
            };
        
        private Renderer meshRenderer;
        private static readonly int ColorId = Shader.PropertyToID("_BaseColor");
        private MaterialPropertyBlock materialPropertyBlock;
        private PathTraverser pathTraverser;
        private new Collider collider;

        public int Id;

        private AgentState state;
        public AgentState State
        {
            get => state;
            set
            {
                AgentsInStates[state].Remove(this);
                state = value;
                AgentsInStates[state].Add(this);
            }
        }

        private readonly Queue<Sequence> sequences = new Queue<Sequence>();
        [NonSerialized] public Sequence CurrentSequence;
        [CanBeNull] public Sequence NextSequence => sequences.Count == 0 ? null : sequences.Peek();

        [CanBeNull] private Meeting meeting;
        [CanBeNull] public Meeting CurrentMeeting
        {
            get => meeting;
            set
            {
                meeting = value;

                if (meeting == null)
                    // TODO: this should be Door.NavMeshPosition
                    pathTraverser.SetTarget(CurrentSequence.TargetDoor.transform.position);
                else
                    pathTraverser.SetTarget(CurrentMeeting.Position);
            }
        }

        public float Speed;
        public float MinSpeed, MaxSpeed;

        private void Awake()
        {
            pathTraverser = GetComponent<PathTraverser>();
            meshRenderer = GetComponent<MeshRenderer>();
            materialPropertyBlock = new MaterialPropertyBlock();
            collider = GetComponent<Collider>();

            Speed = Random.Range(MinSpeed, MaxSpeed);

            State = AgentState.Idling;
        }

        public void Initialize(int id, IEnumerable<Sequence> initialSequences)
        {
            Id = id;

            foreach (var sequence in initialSequences)
                sequences.Enqueue(sequence);

            sequences.Peek().StartingBuilding.AgentsInside.Add(this);

            // Debug purposed only
            gameObject.name = $"Agent#{id.ToString()}";
        }

        private void Update()
        {
            if (CurrentSequence == null)
            {
                // Check if the next sequence should start
                var nextSequence = sequences.Peek();
                if (SimulationController.Instance.TimeManager.Time >= nextSequence.StartTime)
                {
                    // TODO: Enable meetings
                    //SimulationController.Instance.MeetingManager.RegisterForMeeting(this, nextSequence);
                    
                    nextSequence.StartingDoor.WaitingAgents.Enqueue(this);
                }
            }
        }

        private void StartSequence()
        {
            State = AgentState.Walking;
            
            CurrentSequence = sequences.Dequeue();
            
            // Set starting position
            transform.position = CurrentSequence.StartingDoor.NavMeshPosition;
            // Also sets the PathTraverser
            CurrentMeeting = CurrentSequence.Meeting;

            CurrentSequence.StartingBuilding.AgentsInside.Remove(this);

            meshRenderer.enabled = true;
            materialPropertyBlock.SetColor(
                ColorId,
                SimulationController.Instance.BuildingManager.GetColor(CurrentSequence)
            );
            meshRenderer.SetPropertyBlock(materialPropertyBlock);

            collider.enabled = true;
        }

        private void EndSequence()
        {
            State = AgentState.Idling;

            CurrentSequence.TargetBuilding.AgentsInside.Add(this);

            CurrentSequence = null;
            
            meshRenderer.enabled = false;
            collider.enabled = false;

            if (sequences.Count == 0)
                gameObject.SetActive(false);
        }

        // This is only called by the Door component (== observer.update)
        public void PassTheDoor()
        {
            if (CurrentSequence == null)
                StartSequence();
            else
                EndSequence();
        }

        // This is only called by the PathTraverser component (== observer.update)
        public void PathTraverseFinished()
        {
            if (CurrentMeeting != null)
                CurrentMeeting.Arrived();
            else
                // TODO: Handle NullReferenceException :C
                CurrentSequence.TargetDoor.WaitingAgents.Enqueue(this);
        }
    }
}