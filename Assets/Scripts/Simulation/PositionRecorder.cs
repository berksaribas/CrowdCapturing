using System;
using System.Globalization;
using System.IO;
using UnityEngine;
using Util;

namespace Simulation
{
    public class PositionRecorder : MonoBehaviour
    {
        private StreamWriter recordFile;

        private void Awake()
        {
            if (!enabled)
            {
                Destroy(this);
                return;
            }
            
            var now = DateTime.Now.ToString("dd-MM-yyyy HH_mm_ss");
            var path = IOHelper.GetFullPath("", $"[{now}] Agent Positions", "csv");
            recordFile = new StreamWriter(path);

            // Header
            recordFile.WriteLine("AgentID, AgentX, AgentY, TimeInSeconds");
        }

        private void LateUpdate()
        {
            var currentTimeInSeconds = SimulationController.Instance
                .TimeManager.TimeInSeconds.ToString("0.0", CultureInfo.InvariantCulture);

            foreach (var agent in Agent.AgentsInStates[AgentState.Walking])
            {
                var position = agent.transform.position;
                var x = position.x.ToString("0.000", CultureInfo.InvariantCulture);
                var y = position.z.ToString("0.000", CultureInfo.InvariantCulture);
                var id = agent.Id.ToString(CultureInfo.InvariantCulture);

                recordFile.WriteLine(string.Join(",", id, x, y, currentTimeInSeconds));
            }
        }

        private void OnDestroy()
        {
            if (enabled)
                recordFile.Dispose();
        }
    }
}