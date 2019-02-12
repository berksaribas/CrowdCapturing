using UnityEngine;
using World;

namespace Simulation
{
	public class AgentData
	{
		public Door StartingDoor;
		public Door TargetDoor;
		public MaterialPropertyBlock MaterialPropertyBlock;
		public bool IsCreateData;

		public Agent agent;
		
		public AgentData(Door startingDoor, Door targetDoor, MaterialPropertyBlock materialPropertyBlock)
		{
			StartingDoor = startingDoor;
			TargetDoor = targetDoor;
			MaterialPropertyBlock = materialPropertyBlock;
			IsCreateData = true;
		}

		public AgentData(Agent agent)
		{
			this.agent = agent;
			IsCreateData = false;
		}
	}
}