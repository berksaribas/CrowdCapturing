using UnityEngine;
using World;

namespace Simulation
{
	public class AgentData
	{
		public enum DataType
		{
			IndividualMove,
			GroupMoveBeforeMeet,
			FinishSequence
		}
		
		public readonly MaterialPropertyBlock MaterialPropertyBlock;
		public readonly Agent Agent;
		public readonly Door StartingDoor;
		public readonly DataType Type;
		
		public AgentData(Agent agent, MaterialPropertyBlock materialPropertyBlock, Door startingDoor, DataType type)
		{
			MaterialPropertyBlock = materialPropertyBlock;
			Type = type;
			Agent = agent;
			StartingDoor = startingDoor;
		}

		public AgentData(Agent agent)
		{
			Agent = agent;
			Type = DataType.FinishSequence;
		}
	}
}