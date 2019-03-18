using UnityEngine;
using World;

namespace Simulation
{
	public class AgentData
	{
		public MaterialPropertyBlock MaterialPropertyBlock;
		public bool IsCreateData;
		public Agent Agent;
		public Door StartingDoor;
		
		public AgentData(Agent agent, MaterialPropertyBlock materialPropertyBlock, Door startingDoor)
		{
			MaterialPropertyBlock = materialPropertyBlock;
			IsCreateData = true;
			Agent = agent;
			StartingDoor = startingDoor;
		}

		public AgentData(Agent agent)
		{
			Agent = agent;
			IsCreateData = false;
		}
	}
}