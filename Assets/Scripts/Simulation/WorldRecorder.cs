using System;
using System.Globalization;
using System.IO;
using UnityEngine;
using Util;

namespace Simulation
{
    public class WorldRecorder : MonoBehaviour
    {
        private void Awake()
        {
            if (!enabled)
                Destroy(this);
        }

        private void Start()
        {
            var now = DateTime.Now.ToString("dd-MM-yyyy HH_mm_ss");
            var path = IOHelper.GetFullPath("", $"[{now}] Building & Door Positions", "csv");
            using (var recordFile = new StreamWriter(path))
            {
                // Header
                recordFile.WriteLine("BuildingID, BuildingX, BuildingY, DoorID, DoorX, DoorY");
                
                foreach (var building in SimulationController.Instance.BuildingManager.GetBuildings())
                {
                    var buildingPosition = building.transform.position;
                    var buildingX = buildingPosition.x.ToString("0.000", CultureInfo.InvariantCulture);
                    var buildingY = buildingPosition.z.ToString("0.000", CultureInfo.InvariantCulture);
                    var buildingId = building.DataAlias;

                    foreach (var door in building.Doors)
                    {
                        var doorPosition = door.NavMeshPosition;
                        var doorX = doorPosition.x.ToString("0.000", CultureInfo.InvariantCulture);
                        var doorY = doorPosition.z.ToString("0.000", CultureInfo.InvariantCulture);
                        var doorId = door.name;
                        
                        recordFile.WriteLine(string.Join(",",
                            buildingId, buildingX, buildingY,
                            doorId, doorX, doorY
                        ));
                    }
                }
            }
            
            enabled = false;
        }
    }
}